import { Component, OnInit, ElementRef, NgZone } from '@angular/core';
import { NgForm, FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { Airline } from 'src/app/entities/airline/airline';
import { Polyline } from 'src/app/entities/user/traveller';
import { UserType } from 'src/app/entities/userType/userType';
import { Address } from 'src/app/entities/address/address';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { WebsiteAdminModel } from 'src/app/entities/user/websiteAdminModel';

@Component({
    selector: 'app-websiteAdmin',
    templateUrl: './websiteAdmin.component.html',
    styleUrls: ['./websiteAdmin.component.css']
})

export class WebsiteAdminComponent implements OnInit {
    user: WebsiteAdminModel;
    loaded: boolean;
    websitesAdministratorsHidden: boolean;
    rentACarHidden: boolean;
    airlineHidden: boolean;
    profileInfoHidden: boolean;
    hideNewAdminDiv: boolean;
    discountsHidden: boolean;

    hide = true;
    ChangePasswordGroup: FormGroup;
    ChangeUsernameGroup: FormGroup;
    ChangeInformationGroup: FormGroup;
    panelOpenState = false;
    wrongPassword: boolean;
    usernameExists: boolean;

    websiteAdministrators: Array<User>;
    newAdminGroup: FormGroup;

    carCompanyAdministrators: Array<User>;
    newCarCompanyGroup: FormGroup;

    airlines: Array<Airline>;
    airlineAdministrators: Array<User>;
    newAirlineGroup: FormGroup;

    DiscountChange: FormGroup;

    zoom: number;
    public polyline: Polyline;
    options : string[];
    marker : any = {url:"assets/carMarker.png", scaledSize: {width: 30, height: 30}};
    public clickedLat: number;
    public clickedLon: number;
    private geoCoder;
    clickedAddress:string;
    public searchElementRef: ElementRef;


    constructor(private mapsAPILoader: MapsAPILoader, private ngZone: NgZone, public route: Router, public router: ActivatedRoute, private fb: FormBuilder, private airlineService: AirlineService, private userService: UserService, private carRentalService: CarRentalService ) {

    }

    ngOnInit(): void {
      this.loaded = false;
      this.ChangePasswordGroup = this.fb.group({
        'newPassword' : new FormControl ('', Validators.required),
        'repeatNewPassword' : new FormControl ('', Validators.required),
        'oldPassword' : new FormControl ('', Validators.required),
      }, {validator: this.checkIfMatchingPasswords('newPassword', 'repeatNewPassword')});
      this.newAdminGroup = this.fb.group({
        'email' : new FormControl('', [Validators.required, Validators.email]),
        'pass' : new FormControl('', Validators.required),
        'confirmPass' : new FormControl ('', Validators.required),
        'username' : new FormControl ('', Validators.required),
      }, {validator: this.checkIfMatchingPasswords('pass', 'confirmPass')});
       this.newCarCompanyGroup = this.fb.group({
       'companyName': new FormControl('', Validators.required),
      });
      this.newAirlineGroup = this.fb.group({
        'companyName': new FormControl('', Validators.required),
        'address': new FormControl('', Validators.required),
      });
        this.userService.loadWebsiteAdmin().subscribe(
          (res: any) => {
            this.user = res;
            this.ChangeUsernameGroup = this.fb.group({
              'username' : new FormControl (this.user.username, Validators.required),
              'currentPass' : new FormControl ('', Validators.required),
            });
            var date = new Date(this.user.birthday);
            this.ChangeInformationGroup = this.fb.group({
              'fullname' : new FormControl (this.user.fullName, Validators.required),
              'address' : new FormControl (this.user.address, Validators.required),
              'email' : new FormControl (this.user.email, [Validators.required, Validators.email]),
              'birthday' : new FormControl (date, Validators.required),
              'passCurrent' : new FormControl ('', Validators.required),
            });
           
            this.DiscountChange = this.fb.group({
              'bronze' : new FormControl(this.user.discount.bronzeTier, Validators.required),
              'silver' : new FormControl(this.user.discount.silverTier, Validators.required),
              'gold' : new FormControl (this.user.discount.goldTier, Validators.required),
              'percent' : new FormControl (this.user.discount.discountPercent, Validators.required),
            });
            this.CloseAll();
            this.loaded = true;
            if(this.user.mainAdmin) {
              this.ShowWebAdministrators();
            }
            else {
              this.ShowRentACar();
            }
            this.carRentalService.getCarCompanies().subscribe(
              (res: any) => {
                this.user.rentAcars = res;
                this.userService.getAdmins(this.user.rentAcars).subscribe(
                  (res: any) => {
                    this.user.rentAcars = res;
                  },
                  err => {
                    document.getElementById('changeInfoError').style.color = 'red';
                    document.getElementById('changeInfoError').textContent = err.error.message;
                    console.log(err);
                    
                  }
                );
              },
              err => {
                document.getElementById('changeInfoError').style.color = 'red';
                document.getElementById('changeInfoError').textContent = err.error.message;
                console.log(err);
                
              }
            );
            this.airlineService.getAirCompanies().subscribe(
              (res: any) => {
                this.user.airlines = res;
                this.userService.getAirAdmins(this.user.airlines).subscribe(
                  (res: any) => {
                    this.user.airlines = res;
                  },
                  err => {
                    document.getElementById('changeInfoError').style.color = 'red';
                    document.getElementById('changeInfoError').textContent = err.error.message;
                    console.log(err);
                    
                  }
                );
              },
              err => {
                document.getElementById('changeInfoError').style.color = 'red';
                document.getElementById('changeInfoError').textContent = err.error.message;
                console.log(err);
                
              }
            );
          },
          err => {
            console.log(err);
          }
        );        

        this.hideNewAdminDiv = true;

        
        
        
        

       
        this.zoom = 4;
        this.polyline = new Polyline([], 'blue', { url:"assets/carMarker.png", scaledSize: {width: 30, height: 30}});
        this.clickedLat = 45;
        this.clickedLon = 20;
        this.clickedAddress = "";

        this.mapsAPILoader.load().then(() => {
          this.geoCoder = new google.maps.Geocoder;
        });
    }

    CloseAll() {
        this.websitesAdministratorsHidden = true;
        var webAdmins = document.getElementById("websiteAdministratorsRow");
        webAdmins.style.backgroundColor = "transparent";
        this.rentACarHidden = true;
        var rentAcar = document.getElementById("rentACarRow");
        rentAcar.style.backgroundColor = "transparent"; 
        this.airlineHidden = true;
        var airlines = document.getElementById("airlinesRow");
        airlines.style.backgroundColor = "transparent";  
        this.profileInfoHidden = true;
        var profile = document.getElementById("profileInfoRow");
        profile.style.backgroundColor = "transparent";  
        this.discountsHidden = true;
        var discounts = document.getElementById("discountsRow");
        discounts.style.backgroundColor = "transparent";  

        this.clickedAddress = "";
        this.clickedLat = 45;
        this.clickedLon = 20;
        this.zoom = 4;
    }

    ShowWebAdministrators() {
        this.CloseAll();
        document.getElementById("websiteAdministratorsRow").style.backgroundColor = "gainsboro";
        this.websitesAdministratorsHidden = false;
    }

    ShowRentACar() {
        this.CloseAll();
        document.getElementById("rentACarRow").style.backgroundColor = "gainsboro";
        this.rentACarHidden = false;
    }

    ShowAirlines() {
        this.CloseAll();
        document.getElementById("airlinesRow").style.backgroundColor = "gainsboro";
        this.airlineHidden = false;
    }

    ShowProfileInfo() {
        this.CloseAll();
        document.getElementById("profileInfoRow").style.backgroundColor = "gainsboro";
        this.profileInfoHidden = false;
    }

    ShowDiscounts() {
        this.CloseAll();
        document.getElementById("discountsRow").style.backgroundColor = "gainsboro";
        this.discountsHidden = false;
    }
    
    checkIfMatchingPasswords(passwordKey: string, passwordConfirmationKey: string) {
        return (group: FormGroup) => {
          let passwordInput = group.controls[passwordKey],
              passwordConfirmationInput = group.controls[passwordConfirmationKey];
          if (passwordInput.value !== passwordConfirmationInput.value) {
            return passwordConfirmationInput.setErrors({notEquivalent: true})
          }
          else {
              return passwordConfirmationInput.setErrors(null);
          }
        }
      }
    
      checkUsername() {
        
      }
    
      ChangePassword() {
        if(this.ChangePasswordGroup.valid) {
          if(this.ChangePasswordGroup.value.oldPassword == this.ChangePasswordGroup.value.newPassword) {
            document.getElementById('changePassError').style.color = 'red';
            document.getElementById('changePassError').textContent = 'New password can not be the same as old password';
            return;
          }

          var pass = this.ChangePasswordGroup.value.oldPassword;
          var newPass = this.ChangePasswordGroup.value.newPassword;
    
          this.userService.SaveNewPasswordWAdmin(pass, newPass).subscribe(
            (res: any) => {
              this.user.changedPassword = true;
              this.ChangeInformationGroup.value.passCurrent = '';
              document.getElementById('changePassError').style.color = 'green';
              document.getElementById('changePassError').textContent = 'Success!';
            },
            err => {
              document.getElementById('changePassError').style.color = 'red';
              document.getElementById('changePassError').textContent = err.error.message;
              console.log(err);
              
            }
          );
        }
      }
    
      changeUsername() {
        if(this.ChangeUsernameGroup.valid) {
          var usrnm = this.ChangeUsernameGroup.value.username;
          var pass = this.ChangeUsernameGroup.value.currentPass;
          
          this.userService.SaveNewUsernameWAdmin(usrnm, pass).subscribe(
            (res: any) => {
              this.user.username = res.username;
              this.ChangeInformationGroup.value.passCurrent = '';
              document.getElementById('changeUsernameError').style.color = 'green';
              document.getElementById('changeUsernameError').textContent = 'Success!';
            },
            err => {
              document.getElementById('changeUsernameError').style.color = 'red';
              document.getElementById('changeUsernameError').textContent = err.error.message;
              console.log(err);
              
            }
          );
        }
        
      }
    
      changeInformation() {
        if(this.ChangeInformationGroup.valid) {
          var n = this.ChangeInformationGroup.value.fullname;
          var addr = this.ChangeInformationGroup.value.address;
          var email = this.ChangeInformationGroup.value.email;
          var bd = this.ChangeInformationGroup.value.birthday;
          var date = new Date(bd);
          bd = date.toDateString();
          var pass = this.ChangeInformationGroup.value.passCurrent;
          this.userService.SaveWAdminAccountDetails(n, bd, addr, pass, email).subscribe(
            (res: any) => {
              this.user.fullName = res.fullName;
              this.user.address = res.address;
              this.user.birthday = res.birthday;
              this.user.email = res.email;
              this.ChangeInformationGroup.value.passCurrent = '';
              document.getElementById('changeInfoError').style.color = 'green';
              document.getElementById('changeInfoError').textContent = 'Success!';
            },
            err => {
              document.getElementById('changeInfoError').style.color = 'red';
              document.getElementById('changeInfoError').textContent = err.error.message;
              console.log(err);
              
            }
          );
        }
      }
    
    ShowNewAdminDiv() {
        this.hideNewAdminDiv = false;
    }

    addUser() {
        if(this.newAdminGroup.valid) {
          var email = this.newAdminGroup.value.email;
          var pass = this.newAdminGroup.value.pass;
          var username = this.newAdminGroup.value.username;
          var model = {
            username: username,
            password: pass,
            email: email,
          }
          this.userService.AddNewWebAdmin(model).subscribe(
            (res: any) => {
              this.user.websiteAdministrators = res;
              document.getElementById('changeInfoError').style.color = 'green';
              document.getElementById('changeInfoError').textContent = 'Success!';
              this.newAdminGroup.patchValue({
                email: '',
                pass: '',
                username: '',
                confirmPass: '',
              });
              this.hideNewAdminDiv = true;
            },
            err => {
              document.getElementById('changeInfoError').style.color = 'red';
              document.getElementById('changeInfoError').textContent = err.error.message;
              console.log(err);
              
            }
          );
        }
        else {
            // forma nije validna
        }
    }

    CloseNewAdminDiv() {
        this.newAdminGroup.reset();
        this.hideNewAdminDiv = true;
    }



    getAdmin2(company: Airline) {
        for(let i = 0; i <this.airlineAdministrators.length; i++) {
            if(company.admin == this.airlineAdministrators[i].id) {
                return this.airlineAdministrators[i];
            }
        }
    }

    ShowNewCarCompanyDiv() {
      document.getElementById("newCarCompanyDiv").style.display = 'block';
    }

    closeNewCarCompanyDiv() {
      this.newCarCompanyGroup.reset();
      document.getElementById("newCarCompanyDiv").style.display = 'none';
    }

    registerCarCompany() {
      if(this.newCarCompanyGroup.valid && this.clickedAddress != "") {
        var companyName = this.newCarCompanyGroup.value.companyName;
        // var email = this.newCarCompanyGroup.value.email;
        // var username = this.newCarCompanyGroup.value.username;
        // var pass = this.newCarCompanyGroup.value.pass;
        // var pass2 = this.newCarCompanyGroup.value.confirmPass;

        if(true) {
          this.carRentalService.RegisterCarCompany(companyName, '', '', '', this.clickedAddress, this.clickedLat, this.clickedLon).subscribe(
            (res: any) => {
              this.user.rentAcars = res;
              this.newCarCompanyGroup.patchValue({
                companyName: '',
              });
              this.closeNewCarCompanyDiv();
              this.userService.getAdmins(this.user.rentAcars).subscribe(
                (res: any) => {
                  this.user.rentAcars = res;
                },
                err => {
                  console.log(err);
                  
                }
              );
            },
            err => {
              console.log(err);
              
            }
          );
        }
      }
    }

    ShowNewAirlineDiv() {
      document.getElementById("newAirlineDiv").style.display = 'block';
    }

    closeNewAirlineDiv() {
      this.newAirlineGroup.reset();
      document.getElementById("newAirlineDiv").style.display = 'none';
    }

    registerAirline() {
      if(this.newAirlineGroup.valid) {
        var companyName = this.newAirlineGroup.value.companyName;
        var address = this.newAirlineGroup.value.address;
       

        if(true) {
          var model = {
            companyName: companyName,
            address: address,
            email:'',
            username: '',
            password: ''
          }
          this.airlineService.RegisterAirCompany(model).subscribe(
            (res: any) => {
              this.user.airlines = res;
              this.userService.getAirAdmins(this.user.airlines).subscribe(
                (res: any) => {
                  this.user.airlines = res;
                },
                err => {
                  console.log(err);
                  
                }
              );
              this.newCarCompanyGroup.reset();
              this.closeNewAirlineDiv();
            },
            err => {
              console.log(err);
              
            }
          );
        }

        // var adminID = this.userService.addUser(new User(new Array<Ticket>(), 
        // [], [], [], "", new Date(Date.now()), [], 
        // this.airlineAdministrators.length + 100, "", "", "", username, pass, email, UserType.AirlineAdmin, [], [], 0));
        // this.airlineAdministrators = [];
        // this.airlineAdministrators = this.userService.getAllAirlineAdmins();
        // var addr = new Address(this.clickedAddress, this.clickedLat, this.clickedLon);
        
        // this.airlineService.addNewCompany(companyName, adminID, addr);
        // this.closeNewAirlineDiv();
      }
      
    }

    OpenAddAdmin(id:number) {
      document.getElementById("addAdmin" + id.toString()).style.display = 'block';
    }
    OpenAddAdmin2(id:number) {
      document.getElementById("addAdminAir" + id.toString()).style.display = 'block';
    }
    CloseAddAdmin(id: number) {
      document.getElementById("addAdmin" + id.toString()).style.display = 'none';
    }
    CloseAddAdmin2(id: number) {
      document.getElementById("addAdminAir" + id.toString()).style.display = 'none';
    }
    AddAdmin(id:number) {
      
      var fields = document.getElementsByName("addAdmin" + id.toString());
      var model = {
        companyId: id,
        username: (fields[0] as HTMLInputElement).value,
        email: (fields[1] as HTMLInputElement).value,
        password: (fields[2] as HTMLInputElement).value,
        password2: (fields[3] as HTMLInputElement).value,
      }
      if(model.password == model.password2) {
        this.userService.AddAdminToCompany(model).subscribe(
          (res: any) => {
            this.user.rentAcars = res;
            document.getElementById("addAdmin" + id.toString()).style.display = 'none';
          },
          err => {
            console.log(err);
            
          }
        );
      }
      else {
        //passwords do not match
      }
    }

    AddAdmin2(id:number) {
      
      var fields = document.getElementsByName("addAdminA" + id.toString());
      var model = {
        companyId: id,
        username: (fields[0] as HTMLInputElement).value,
        email: (fields[1] as HTMLInputElement).value,
        password: (fields[2] as HTMLInputElement).value,
        password2: (fields[3] as HTMLInputElement).value,
      }
      if(model.password == model.password2) {
        this.userService.AddAdminToACompany(model).subscribe(
          (res: any) => {
            this.user.airlines = res;
            document.getElementById("addAdminAir" + id.toString()).style.display = 'none';
          },
          err => {
            console.log(err);
            
          }
        );
      }
      else {
        //passwords do not match
      }
    }

    SaveDiscountChange() {
      if(this.DiscountChange.valid) {
        var model = {
          bronzeTier: this.DiscountChange.value.bronze,
          silverTier: this.DiscountChange.value.silver,
          goldTier: this.DiscountChange.value.gold,
          discountPercent: this.DiscountChange.value.percent,
        }
        this.userService.SaveDiscount(model).subscribe(
          (res: any) => {
          },
          err => {
            console.log(err);
            
          }
        );
      }
      else {
        alert("invalid");
      }
    }

    mapClick1($event){
      this.clickedLat = ($event.coords.lat);
      this.clickedLon = ($event.coords.lng);
      this.getAddress(this.clickedLat, this.clickedLon);
    }

    mapClick2($event){
      this.clickedLat = ($event.coords.lat);
      this.clickedLon = ($event.coords.lng);
      this.getAddress(this.clickedLat, this.clickedLon);
    }

    getAddress(latitude: number, longitude:number) {
      this.geoCoder.geocode({ 'location': { lat: latitude, lng: longitude } }, (results, status) => {
        console.log(results);
        console.log(status);
        if (status === 'OK') {
          if (results[0]) {
            this.zoom = 12;
            this.clickedAddress = results[0].formatted_address;
          } else {
            window.alert('No results found');
          }
        } else {
          window.alert('Geocoder failed due to: ' + status);
        }
  
      });
    }

}