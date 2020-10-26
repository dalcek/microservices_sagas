import { Component, OnInit, Input, ElementRef, NgZone } from '@angular/core';
import { Router } from '@angular/router'
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { FormBuilder, FormGroup, Validators, FormControl}  from '@angular/forms';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { group } from '@angular/animations';
import { ChartsModule, WavesModule, CarouselModule } from 'angular-bootstrap-md'
import { Polyline } from 'src/app/entities/user/traveller';
import { Address } from 'src/app/entities/address/address';
import { CarRentalAdminModel } from 'src/app/entities/user/carRentalAdminModel';
import { CompanyInfoModel } from 'src/app/entities/rentAcar/companyInfoModel';
import { ExtraAmenity } from 'src/app/entities/rentAcar/ExtraAmenity';

interface Type {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-carRentalAdmin',
  templateUrl: './carRentalAdmin.component.html',
  styleUrls: ['./carRentalAdmin.component.css']
})



export class CarRentalAdminComponent implements OnInit {
  user: CarRentalAdminModel;
  companyInfo: CompanyInfoModel;
  amenityPayment: string;
  loaded: boolean;
  selected: string;
  lastClosedEdit: number;
  lastEditedAmenity: number;
  rating: number;
  rRating: number;

  types: Type[] = [
    {value: 'economy', viewValue: 'Economy' },
    {value: 'compact', viewValue: 'Compact'},
    {value: 'mid-size', viewValue: 'Mid-size'},
    {value: 'standard-size', viewValue: 'Standard-size'},
    {value: 'full-size', viewValue: 'Full-size'},
    {value: 'luxory', viewValue: 'Luxory'},
    {value: 'suv', viewValue: 'SUV'},
    {value: 'van', viewValue: 'Van'},
    {value: 'minivan', viewValue: 'Mini Van'},
    {value: 'convertible', viewValue: 'Convertible'},
    {value: 'pickup', viewValue: 'Pickup'}
  ];
  selectedType: string;
  selectedLocation: string;
  selectedNewType: string;
  selectedNewLocation:string;

  public chartType: string = 'line';
  public chartDatasets: Array<any> = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'Reservations' }
  ];
  public chartLabels: Array<any> = ['January', 'February', 'March', 'April', 'May', 'June', 'July'];

  public chartColors: Array<any> = [
    {
      backgroundColor: 'rgba(105, 0, 132, .2)',
      borderColor: 'rgba(200, 99, 132, .7)',
      borderWidth: 2,
    },
  ];

  public chartOptions: any = {
    responsive: true
  };



  // drugi grafik
  public chartType2: string = 'bar';

  public chartDatasets2: Array<any> = [
    { data: [0, 0, 0, 0, 0], label: 'Ratings' }
  ];

  public chartLabels2: Array<any> = ['☆', '☆☆', '☆☆☆', '☆☆☆☆', '☆☆☆☆☆'];

  public chartColors2: Array<any> = [
    {
      backgroundColor: [
        'rgba(255, 99, 132, 0.2)',
        'rgba(54, 162, 235, 0.2)',
        'rgba(255, 206, 86, 0.2)',
        'rgba(75, 192, 192, 0.2)',
        'rgba(153, 102, 255, 0.2)'
      ],
      borderColor: [
        'rgba(255,99,132,1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 206, 86, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(153, 102, 255, 1)'
      ],
      borderWidth: 2,
    }
  ];

  public chartOptions2: any = {
    responsive: true
  };

  GetProfitGroup: FormGroup;

  dashboardHidden: boolean;
  extrasHidden: boolean;
  companyInfoHidden: boolean;
  carsHidden: boolean;
  branchesHidden: boolean;
  profileInfoHidden: boolean;

  hide = true;
  ChangePasswordGroup: FormGroup;
  ChangeUsernameGroup: FormGroup;
  ChangeInformationGroup: FormGroup;
  panelOpenState = false;
  wrongPassword: boolean;
  usernameExists: boolean;

  ChangeCompanyInfoGroup: FormGroup;
  NewCarGroup: FormGroup;

  AvailableCarsGroup: FormGroup;


  //branches
  startLat:number;
  startLon:number;
  zoom: number;
  public polyline: Polyline;
  options : string[];
  busImgIcon : any = {url:"assets/carMarker.png", scaledSize: {width: 30, height: 30}};

  private geoCoder;
  latitude: number;
  longitude: number;
  clickedAddress:string;
  public searchElementRef: ElementRef;

  latitudeEdit: number;
  longitudeEdit: number;
  clickedAddress2: string;



  constructor(private mapsAPILoader: MapsAPILoader, private ngZone: NgZone, private route: Router, private carRentalService: CarRentalService, private fb:FormBuilder, private userService: UserService) { }

  ngOnInit(): void {
    this.loaded = false;
    this.GetProfitGroup = this.fb.group({
      'dateFrom': new FormControl('', Validators.required),
      'dateTo': new FormControl('', Validators.required),
    });
    this.AvailableCarsGroup = this.fb.group({
      'from': new FormControl('', Validators.required),
      'to': new FormControl('', Validators.required),
    }, {validator: this.checkDatesValidator('from', 'to')});
    this.ChangePasswordGroup = this.fb.group({
      'newPassword' : new FormControl ('', [Validators.required, Validators.minLength(4)]),
      'repeatNewPassword' : new FormControl ('', [Validators.required, Validators.minLength(4)]),
      'oldPassword' : new FormControl ('', Validators.required),
    }, {validator: this.checkIfMatchingPasswords('newPassword', 'repeatNewPassword')});

    this.userService.loadCarRentalAdmin().subscribe(
      (res: any) => {
        this.user = res;
        this.carRentalService.getAdminCompany(this.user.companyId).subscribe(
          (res: any) => {
            this.user.companyInfo = res;
            for(let i = 0; i < this.user.companyInfo.cars.length; i++) {
              this.user.companyInfo.cars[i].available = '';
            }
            this.ChangeCompanyInfoGroup = this.fb.group({
              'companyName' : new FormControl (this.user.companyInfo.name, Validators.required),
              'description' : new FormControl (this.user.companyInfo.description, Validators.required),
              'logoImage' : new FormControl ('', Validators.required)
            });
            //branches
            this.startLat = this.user.companyInfo.locations.mainLocation.latitude;
            this.startLon = this.user.companyInfo.locations.mainLocation.longitude;
            this.latitude = this.user.companyInfo.locations.mainLocation.latitude;
            this.longitude = this.user.companyInfo.locations.mainLocation.longitude;
            this.zoom = 6;
            this.polyline = new Polyline([], 'blue', { url:"assets/carMarker.png", scaledSize: {width: 30, height: 30}});
            this.chartDatasets2[0].data = [this.user.companyInfo.ratings[0], this.user.companyInfo.ratings[1], this.user.companyInfo.ratings[2], this.user.companyInfo.ratings[3], this.user.companyInfo.ratings[4]];
            this.chartDatasets[0].data = [this.user.companyInfo.dailyReservations[0], this.user.companyInfo.dailyReservations[1],this.user.companyInfo.dailyReservations[2],this.user.companyInfo.dailyReservations[3],this.user.companyInfo.dailyReservations[4],this.user.companyInfo.dailyReservations[5],this.user.companyInfo.dailyReservations[6]];
            this.chartLabels = [this.user.companyInfo.dailyLabels[0], this.user.companyInfo.dailyLabels[1],this.user.companyInfo.dailyLabels[2],this.user.companyInfo.dailyLabels[3],this.user.companyInfo.dailyLabels[4],this.user.companyInfo.dailyLabels[5],this.user.companyInfo.dailyLabels[6]];
            this.selected = "daily";
            this.loaded = true;
            this.lastClosedEdit = 0;
            this.lastEditedAmenity = 0;
            this.longitudeEdit = this.user.companyInfo.locations.mainLocation.longitude;
            this.latitudeEdit = this.user.companyInfo.locations.mainLocation.latitude;
            this.clickedAddress2 = '';

            this.rating = 0;
            for(let i = 0; i<this.user.companyInfo.ratings.length; i++) {
              this.rating += this.user.companyInfo.ratings[i];
            }
            this.rRating = Math.round(this.rating / this.user.companyInfo.ratings.length);
            this.rating = this.rating / this.user.companyInfo.ratings.length;
          },
          err => {
            document.getElementById('changeInfoError').style.color = 'red';
            document.getElementById('changeInfoError').textContent = err.error.message;
            console.log(err);
            
          }
        );
        
        
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
        
        

        
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
    
    //this.company = this.carRentalService.getCompany(this.user.id);
    this.ShowDashboard();
    
    
    
    this.NewCarGroup = this.fb.group({
       'brand' : new FormControl ('', Validators.required),
       'model' : new FormControl ('', Validators.required),
       'year' : new FormControl ('', [Validators.required, Validators.maxLength(4), Validators.minLength(4)]),
       'location' : new FormControl ('', Validators.required),
       'passengers' : new FormControl ('', Validators.required),
       'type' : new FormControl ('', Validators.required),
       'price' : new FormControl ('', Validators.required),
       'image' : new FormControl('', Validators.required),
    });

    this.wrongPassword = false;
    this.usernameExists = false;

    //branches
    this.mapsAPILoader.load().then(() => {
      this.geoCoder = new google.maps.Geocoder;

      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement);
      autocomplete.addListener("place_changed", () => {
        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocomplete.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude and zoom
          this.latitude = place.geometry.location.lat();
          this.longitude = place.geometry.location.lng();
          this.zoom = 12;
        });
      });
    });
  }

  markerDragEnd($event: MouseEvent) {
    console.log($event);
    this.latitude = $event.coords.lat;
    this.longitude = $event.coords.lng;
    this.getAddress(this.latitude, this.longitude, false);
  }

  markerDragEnd2($event: MouseEvent) {
    console.log($event);
    this.latitudeEdit = $event.coords.lat;
    this.longitudeEdit = $event.coords.lng;
    this.getAddress(this.latitudeEdit, this.longitudeEdit, true);
  }

  getAddress(latitude, longitude, edit:boolean) {
    this.geoCoder.geocode({ 'location': { lat: latitude, lng: longitude } }, (results, status) => {
      console.log(results);
      console.log(status);
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 12;
          if(!edit)
            this.clickedAddress = results[0].formatted_address;
          else 
            this.clickedAddress2 = results[0].formatted_address;

        } else {
          window.alert('No results found');
        }
      } else {
        window.alert('Geocoder failed due to: ' + status);
      }

    });
  }

  CloseAll() {
    this.dashboardHidden = true;
    var dashboard = document.getElementById("dashboardRow");
    dashboard.style.backgroundColor = "transparent";
    this.extrasHidden = true;
    var extars = document.getElementById("extrasRow");
    extars.style.backgroundColor = "transparent"; 
    this.companyInfoHidden = true;
    var company = document.getElementById("companyInfoRow");
    company.style.backgroundColor = "transparent";  
    this.carsHidden = true;
    var cars = document.getElementById("carsRow");
    cars.style.backgroundColor = "transparent";  
    this.branchesHidden = true;
    var branches = document.getElementById("branchesRow");
    branches.style.backgroundColor = "transparent"; 
    this.profileInfoHidden = true;
    var profile = document.getElementById("profileInfoRow");
    profile.style.backgroundColor = "transparent"; 
  }

  ShowDashboard() {
    this.CloseAll();
    document.getElementById("dashboardRow").style.backgroundColor = "gainsboro";
    this.dashboardHidden = false;
  }
  ShowExtras() {
    this.CloseAll();
    document.getElementById("extrasRow").style.backgroundColor = "gainsboro";
    this.extrasHidden = false;
  }
  ShowCompanyInfo() {
    this.CloseAll();
    document.getElementById("companyInfoRow").style.backgroundColor = "gainsboro";
    this.companyInfoHidden = false;
  }
  ShowCars() {
    this.CloseAll();
    document.getElementById("carsRow").style.backgroundColor = "gainsboro";
    this.carsHidden = false;
  }
  ShowBranches() {
    this.CloseAll();
    document.getElementById("branchesRow").style.backgroundColor = "gainsboro";
    this.branchesHidden = false;
  }
  ShowProfileInfo() {
    this.CloseAll();
    document.getElementById("profileInfoRow").style.backgroundColor = "gainsboro";
    this.profileInfoHidden = false;
  }

  GetProfit() {
    var date1 = this.GetProfitGroup.value.dateFrom;
    var date2 = this.GetProfitGroup.value.dateTo;

    if(date1 <= date2) {
      var model = {
        date1: date1.toDateString(),
        date2: date2.toDateString(),
        company: this.user.companyId
      }

      
      this.userService.GetProfitForAdmin(model).subscribe(
        (res: any) => {
          document.getElementById('profit').textContent = res;
        },
        err => {
          console.log(err);
          
        }
      );
    }
    else {
      //umjesto iznosa ispisi gresku
    }
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

  checkDatesValidator(date1Key: string, date2Key:string) {
    return (group: FormGroup) => {
      let date1Input = group.controls[date1Key],
          date2Input = group.controls[date2Key];
      if (date1Input > date2Input) {
        return date1Input.setErrors({not: true})
      }
      else {
          return date1Input.setErrors(null);
      }
    }
  }

  FindAvailableCars() {
    if(this.AvailableCarsGroup.valid) {
      if(this.AvailableCarsGroup.value.from < this.AvailableCarsGroup.value.to) {
        var model = {
          from: this.AvailableCarsGroup.value.from,
          to: this.AvailableCarsGroup.value.to,
          company: this.user.companyId
        }
        this.carRentalService.AvailableCarsAdmin(model).subscribe(
          (res: any) => {
            //this.user = res;
            document.getElementById('availableError').style.display = 'none';
            for(let i = 0; i<this.user.companyInfo.cars.length; i++) {
              this.user.companyInfo.cars[i].available = 'Not available';
            }
            for(let i = 0; i<this.user.companyInfo.cars.length; i++) {
              for(let j = 0; j < res.length; j++) {
                if(this.user.companyInfo.cars[i].id == res[j]) {
                  this.user.companyInfo.cars[i].available = 'Available';
                }
              }
            }
          },
          err => {
            alert(err);
            console.log(err);
            
          }
        );
      }
      else {
        document.getElementById('availableError').style.display = 'block';
      }
      
    }
  }

  ResetAvailable() {
    this.AvailableCarsGroup.reset();
    document.getElementById('availableError').style.display = 'none';
    for(let i = 0; i < this.user.companyInfo.cars.length; i++) {
      this.user.companyInfo.cars[i].available = '';
    }
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

      this.userService.SaveNewPasswordAdmin(pass, newPass).subscribe(
        (res: any) => {
          //this.user = res;
          this.ChangeInformationGroup.value.passCurrent = '';
          document.getElementById('changePassError').style.color = 'green';
          document.getElementById('changePassError').textContent = 'Success!';
          this.user.changedPassword = true;
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
      
      this.userService.SaveNewUsernameAdmin(usrnm, pass).subscribe(
        (res: any) => {
          //this.user = res;
          this.user.username = usrnm
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
      this.userService.SaveAdminAccountDetails(n, bd, addr, pass, email).subscribe(
        (res: any) => {
          this.user.fullName = n;
          this.user.birthday = bd;
          this.user.email = email;
          this.user.address = addr;
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

  saveCompanyChanges() {
    var name = this.ChangeCompanyInfoGroup.value.companyName;
    var desc = this.ChangeCompanyInfoGroup.value.description;
    var img = this.ChangeCompanyInfoGroup.value.logoImage;
    if(img == "") {
      img = this.user.companyInfo.logo;
    }
    else {
      var splits =  (img as string).split('\\');
      img = splits[splits.length-1];
    }
    if(name != "" && desc !="" && img != "") {
      this.carRentalService.updateCarCompany(this.user.companyId, name, desc, img).subscribe(
        (res: any) => {
          // set values
          this.user.companyInfo.name = name;
          this.user.companyInfo.description = desc;
          this.user.companyInfo.logo = img;
          document.getElementById('companyError').style.color = 'green';
          document.getElementById('companyError').textContent = 'Success!';
        },
        err => {
          document.getElementById('companyError').style.color = 'red';
          document.getElementById('companyError').textContent = err.error.message;
          console.log(err);
        }
      );
    }
    else {
      document.getElementById('companyError').style.color = 'red';
      document.getElementById('companyError').textContent = 'Please enter valid data in all fields';
    }
  }

  

  setForDisc(id: number) {
    document.getElementById(id.toString()).style.display = 'none';
    document.getElementById("dicsCar" + id.toString()).style.display = 'block';
  }

  HideDiscountedDates(id: number) {
    document.getElementById("dicsCar" + id.toString()).style.display = 'none';
  }

  editCar(car) {
    if(this.lastClosedEdit != 0) {
      document.getElementById(this.lastClosedEdit.toString()).style.display = 'none';
    }
    this.lastClosedEdit = car.id;
    document.getElementById("dicsCar" + car.id.toString()).style.display = 'none';
    document.getElementById(car.id.toString()).style.display = 'block';
    this.selectedType = car.type;
    this.selectedLocation = car.location;
    var fields = document.getElementsByName(car.id.toString());
    (fields[0] as HTMLInputElement).value = car.brand;
    (fields[1] as HTMLInputElement).value = car.model;
    (fields[2] as HTMLInputElement).value = car.year;
    (fields[3] as HTMLInputElement).value = car.passengers;
    (fields[4] as HTMLInputElement).value = car.price;
  }

  saveEditCar(car) {
    var fields = document.getElementsByName(car.id.toString());

    var model = {
      companyId: this.user.companyId,
      carId: car.id,
      brand: (fields[0] as HTMLInputElement).value,
      model: (fields[1] as HTMLInputElement).value,
      year: parseInt((fields[2] as HTMLInputElement).value),
      type: this.selectedType,
      passen: parseInt((fields[3] as HTMLInputElement).value),
      price: parseInt((fields[4] as HTMLInputElement).value),
      loc: this.selectedLocation,
      
    }

    
    

    this.carRentalService.UpdateCar(model).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.cars = res;
        document.getElementById(car.id.toString()).style.display = 'none';
      },
      err => {
        console.log(err);
        alert(err.error.message);
        (fields[0] as HTMLInputElement).value = car.brand;
        (fields[1] as HTMLInputElement).value = car.model;
        (fields[2] as HTMLInputElement).value = car.year;
        (fields[3] as HTMLInputElement).value = car.type;
        (fields[4] as HTMLInputElement).value = car.passengers;
        (fields[5] as HTMLInputElement).value = car.location;
        (fields[6] as HTMLInputElement).value = car.price;
      }
    );
    
  }

  RemoveCar(car) {
    document.getElementById(car.id.toString()).style.display = 'none';
    this.carRentalService.RemoveCar(car.id).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.cars = res;
      },
      err => {
        alert(err.error.message);
        console.log(err);
        
      }
    );
  }

  closeEditCar(id:number) {
    document.getElementById(id.toString()).style.display = 'none';
  }

  openAddField() {
    document.getElementById("newLocField").style.display = 'block';
  }

  addLocation() {
    document.getElementById("newLocField").style.display = 'none';
    // var newLoc = ((document.getElementById("newLocation") as HTMLInputElement).value);
    this.carRentalService.addCarCompanyLocation(this.user.companyId, this.clickedAddress, this.latitude, this.longitude).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.locations.locations = res;
      },
      err => {
        console.log(err);
      }
    );
    // this.company = this.carRentalService.addLocation(this.company.id, newLoc);
  }

  editLocation(loc: Address) {
    this.latitudeEdit = loc.latitude;
    this.longitudeEdit = loc.longitude;
    this.clickedAddress2 = loc.fullAddress;
    document.getElementById(loc.fullAddress).style.display = 'block';
  }

  cancelEditLocation(loc: Address) {
    document.getElementById(loc.fullAddress).style.display = 'none';
  }

  saveEditLocation(loc) {
    // document.getElementById(loc).style.display = 'none';
    // var newLoc = ((document.getElementsByClassName(loc)[0] as HTMLInputElement).value);
    // this.company = this.carRentalService.saveEditLocation(this.company.id, loc, newLoc);
    this.carRentalService.editCarCompanyLocation(this.user.companyId, this.clickedAddress2, this.latitudeEdit, this.longitudeEdit, loc.id).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.locations.locations = res;
        document.getElementById(loc.fulllAddress).style.display = 'none';
        for(let i = 0; i< this.user.companyInfo.cars.length; i++) {
          if(this.user.companyInfo.cars[i].location.toLowerCase().includes(loc.fullAddress.toLowerCase()))
            this.user.companyInfo.cars[i].location = this.clickedAddress2;
        }
      },
      err => {
        console.log(err);
        if(err.error != null)
          alert(err.error.message);
      }
    );
  }

  removeLocation(locId: number) {
    document.getElementById("remLocError" + locId).textContent = '';
    var element = (document.getElementById("moveLoc"+locId.toString()) as HTMLInputElement);
    var newLoc = element.innerText;
    this.carRentalService.removeCarCompanyLocation(locId, this.user.companyId, newLoc).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.locations.locations = res;
        document.getElementById("remLocError" + locId).textContent = '';
      },
      err => {
        console.log(err);
        if(err.error != null)
          document.getElementById("remLocError" + locId).textContent = err.error.message;
        ;
      }
    );
  }
  
  closeAddCar() {
    document.getElementById("newCarDiv").style.display = 'none';
  }

  OpenAddNewCar() {
    document.getElementById("newCarDiv").style.display = 'block';
    this.selectedNewLocation = '';
  }

  AddNewCar() {
      if(this.NewCarGroup.valid) {
        var  image = this.NewCarGroup.value.image
      if(image == "") {
        return;
      }
      else {
        var splits =  (image as string).split('\\');
        image = splits[splits.length-1];
      }
      var model = {
        companyId: this.user.companyId,
        brand: this.NewCarGroup.value.brand,
        model: this.NewCarGroup.value.model,
        year: this.NewCarGroup.value.year,
        type: this.NewCarGroup.value.type,
        passen: this.NewCarGroup.value.passengers,
        loc: this.NewCarGroup.value.location,
        price: this.NewCarGroup.value.price,
        image: image
      }

      this.carRentalService.addNewCar(model).subscribe(
        (res: any) => {
          // set values
          this.NewCarGroup.reset();
          this.user.companyInfo.cars = res;
          this.closeAddCar();
        },
        err => {
          console.log(err);
        }
      );
    }
  }

  editAmenity(amenity) {
    if(this.lastEditedAmenity != 0) {
      document.getElementById("amenity" + this.lastEditedAmenity.toString()).style.display = 'none';
    }
    this.lastEditedAmenity = amenity.id; 
    var idStr = "amenity"+amenity.id.toString();
    var element = document.getElementById(idStr);
    element.style.display = 'block';
    var fields = document.getElementsByName("amenity" + amenity.id.toString());
    (fields[0] as HTMLInputElement).value = amenity.name;
    (fields[1] as HTMLInputElement).value = amenity.price;
  }
  
  closeEditAmenity(id:number) {
    var idStr = "amenity"+id.toString();
    var element = document.getElementById(idStr);
    element.style.display = 'none';
  }
  saveEditAmenity(amenity) {
    document.getElementById("amenityError" + amenity.id.toString()).style.display = 'none';
    var idStr = "amenity"+amenity.id.toString();
    var element = document.getElementById(idStr);
    element.style.display = 'none';

    var fields = document.getElementsByName(idStr);
    var  image = (fields[2] as HTMLInputElement).value
    if(image == "") {
      image = amenity.image;
    }
    else {
      var splits =  (image as string).split('\\');
      image = splits[splits.length-1];
    }
    var model = {
      companyId: this.user.companyId,
      amenityId: amenity.id,
      name: (fields[0] as HTMLInputElement).value,
      price: parseInt((fields[1] as HTMLInputElement).value),
      image: image,
    }

    this.carRentalService.updateAmenity(model).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.extras = res;
        document.getElementById("amenityError" + amenity.id.toString()).style.display = 'nona';
      },
      err => {
        console.log(err);
        if(err.error != null) {
          document.getElementById("amenityError" + amenity.id.toString()).style.display = 'block';
          document.getElementById("amenityError" + amenity.id.toString()).textContent = err.error.message;
        }
        
      }
    );
  }
  removeAmenity(id: number) {
    document.getElementById("amenityError" + id.toString()).style.display = 'none';
    var model = {
      id: id,
      id2: this.user.companyId
    }
    this.carRentalService.removeAmenity(model).subscribe(
      (res: any) => {
        // set values
        this.user.companyInfo.extras = res;
        document.getElementById("amenityError" + id.toString()).style.display = 'none';
      },
      err => {
        console.log(err);
        if(err.error != null) {
          document.getElementById("amenityError" + id.toString()).style.display = 'block';
          document.getElementById("amenityError" + id.toString()).textContent = err.error.message;
        }
      }
    );
  }
  OpenAddNewAmenity() {
    document.getElementById("newAmenityDiv").style.display = 'block';
  }
  AddNewAmenity() {
    document.getElementById("newAmenityDiv").style.display = 'none';

    var fields = document.getElementsByName("newAmenity");
    var name = (fields[0] as HTMLInputElement).value;
    var price = (fields[1] as HTMLInputElement).value;
    var payment = this.amenityPayment;
    var  image = (fields[3] as HTMLInputElement).value;
    if(name != "" && price != "" && image != "" && payment != "") {
      var splits =  (image as string).split('\\');
      image = splits[splits.length-1];

      var model = {
        companyId: this.user.companyId,
        name: name,
        price: parseInt(price),
        image: image,
        payment: payment
      }

      this.carRentalService.AddAmenity(model).subscribe(
        (res: any) => {
          this.user.companyInfo.extras = res;
        },
        err => {
          console.log(err);
        }
      );

    }
  }
  closeAddAmenity() {
    document.getElementById("newAmenityDiv").style.display = 'none';
  }

  OpenAddNewDiscountRange(id: number){
    document.getElementById("addDiscountRange"+id.toString()).style.display = 'block';
    document.getElementById("discountError").style.display = 'none';
  }

  CloseAddNewDiscountRange(id: number){
    document.getElementById("addDiscountRange"+id.toString()).style.display = 'none';
  }

  SaveNewDiscountRange(id: number) {
    var fields = document.getElementsByName("newRange" + id.toString());
    var model = {
      companyId: this.user.companyId,
      carId: id,
      dateFrom: (fields[0] as HTMLInputElement).value,
      dateTo: (fields[1] as HTMLInputElement).value,
    }
    var date1 = new Date(model.dateFrom);
    var date2 = new Date(model.dateTo);
    if(date1 > date2) {
      //error
    }
    else {
      model.dateFrom = date1.toDateString();
      model.dateTo = date2.toDateString();
      this.carRentalService.SaveNewDiscountRange(model).subscribe(
        (res: any) => {
          this.user.companyInfo.cars = res;
        },
        err => {
          console.log(err);
          document.getElementById("discountError").textContent = err.error.message;
          document.getElementById("discountError").style.display = 'block';
        }
      );
    }
  }

  RemoveDiscountRange(id: number){
    this.carRentalService.RemoveDiscountRange(id).subscribe(
      (res: any) => {
        this.user.companyInfo.cars = res;
      },
      err => {
        console.log(err);
      }
    );
  }

  SwitchToMonthly() {
      this.chartDatasets[0].data = [this.user.companyInfo.monthlyReservations[0], this.user.companyInfo.monthlyReservations[1],this.user.companyInfo.monthlyReservations[2],this.user.companyInfo.monthlyReservations[3],this.user.companyInfo.monthlyReservations[4],this.user.companyInfo.monthlyReservations[5],this.user.companyInfo.monthlyReservations[6]];
      this.chartLabels = [this.user.companyInfo.monthlyLabels[0], this.user.companyInfo.monthlyLabels[1],this.user.companyInfo.monthlyLabels[2],this.user.companyInfo.monthlyLabels[3],this.user.companyInfo.monthlyLabels[4],this.user.companyInfo.monthlyLabels[5],this.user.companyInfo.monthlyLabels[6]];
  }

  SwitchToDaily() {
      this.chartDatasets[0].data = [this.user.companyInfo.dailyReservations[0], this.user.companyInfo.dailyReservations[1],this.user.companyInfo.dailyReservations[2],this.user.companyInfo.dailyReservations[3],this.user.companyInfo.dailyReservations[4],this.user.companyInfo.dailyReservations[5],this.user.companyInfo.dailyReservations[6]];
      this.chartLabels = [this.user.companyInfo.dailyLabels[0], this.user.companyInfo.dailyLabels[1],this.user.companyInfo.dailyLabels[2],this.user.companyInfo.dailyLabels[3],this.user.companyInfo.dailyLabels[4],this.user.companyInfo.dailyLabels[5],this.user.companyInfo.dailyLabels[6]];
  }

  SwitchToWeekly() {
      this.chartDatasets[0].data = [this.user.companyInfo.weeklyReservations[0], this.user.companyInfo.weeklyReservations[1],this.user.companyInfo.weeklyReservations[2],this.user.companyInfo.weeklyReservations[3],this.user.companyInfo.weeklyReservations[4],this.user.companyInfo.weeklyReservations[5],this.user.companyInfo.weeklyReservations[6]];
      this.chartLabels = [this.user.companyInfo.weeklyLabels[0], this.user.companyInfo.weeklyLabels[1],this.user.companyInfo.weeklyLabels[2],this.user.companyInfo.weeklyLabels[3],this.user.companyInfo.weeklyLabels[4],this.user.companyInfo.weeklyLabels[5],this.user.companyInfo.weeklyLabels[6]];
  }


}
