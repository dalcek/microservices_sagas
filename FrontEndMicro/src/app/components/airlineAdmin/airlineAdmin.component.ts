import { Component, OnInit, LOCALE_ID } from '@angular/core';
import { Airline } from 'src/app/entities/airline/airline';
import { Router } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { User } from 'src/app/entities/user/user';
import { FormBuilder, FormGroup, Validators, FormControl}  from '@angular/forms';
import { UserService } from 'src/app/Services/userService';
import { Destination } from 'src/app/entities/destination/destination';
import { Luggage } from 'src/app/entities/flight/luggage';
import { Trip } from 'src/app/entities/flight/trip';
import { Seat } from 'src/app/entities/Seat/seat';
import { Flight } from 'src/app/entities/flight/fligh';
import { Time, formatDate } from '@angular/common';
import { Classes } from 'src/app/entities/flight/class';
import { Traveller } from 'src/app/entities/user/traveller';
import { Chart } from 'node_modules/chart.js';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { IdModel, FlightListingInfo, RatersInfo, TicketListingInfo } from 'src/app/entities/flight/flightListingInfo';
import { AirlineListingInfo } from 'src/app/entities/airline/airlineListingInfo';
import { UserModel, FriendRequestReceived, FriendRequestSent, Notification } from 'src/app/entities/user/userModel';
import { CarReservationModel } from 'src/app/entities/carReservation/carReservationModel';



@Component({
  selector: 'app-airlineAdmin',
  templateUrl: './airlineAdmin.component.html',
  styleUrls: ['./airlineAdmin.component.css']
})



export class AirlineAdminComponent implements OnInit {
  public user = new UserModel(new Array<Ticket>(),'','','','','','','',
  new Array<Ticket>(), new Array<CarReservationModel>(), new Array<TicketListingInfo>(), new Array<UserModel>(),
  new Array<FriendRequestReceived>(),new Array<FriendRequestSent>(),new Array<Notification>(),
  0,0, false, false);
  public company = new AirlineListingInfo(new Array<FlightListingInfo>(), 0, 0 , "", new Array<Destination>(),
  new Array<Destination>(), new Array<RatersInfo>(), 0, new Array<TicketListingInfo>(),
  0, "", "", 0, "logof.jpg");
  loaded: boolean;
  tickets: Array<string>;
  dashboardHidden: boolean;
  sakriven: boolean;
  statisticsHidden: boolean;
  companyInfoHidden: boolean;
  locationsHidden: boolean;
  flightsHidden: boolean;
  pricesHidden: boolean;
  idModel: IdModel;
  traveller: Traveller;
  profileInfoHidden: boolean;
  time: Time = {hours: 2, minutes: 40};
  hide = true;
  brojkolona = new Array<number>();
  duzinareda = new Array<number>();
  brredova = 2;
  brkolona = 2;
  ChangePasswordGroup: FormGroup;
  ChangeUsernameGroup: FormGroup;
  ChangeInformationGroup: FormGroup;
  panelOpenState = false;
  wrongPassword: boolean;
  usernameExists: boolean;
  flg: Flight;

  optionsChecked = [];
  
  ChangeCompanyInfoGroup: FormGroup;
  NewFlightGroup: FormGroup;
  NewFlightGroup2: FormGroup;
  returnFlight: FlightListingInfo;

  successPass: boolean = false;
  successUn: boolean = false;

  daily: boolean;
  weekly: boolean;
  permonth: boolean;
  myChart: any;
  hours: Array<number> = new Array<number>();
 
  ticketDaily: Array<number> = new Array<number>(24);
  ticketWeekly: Array<number> = new Array<number>(7);
  ticketMonthly: Array<number> = new Array<number>(12);

    modelPom = { idCompany: '',
    departureDate: '',
    departureTime: '',
    povratniLet: this.returnFlight,
    price: 0,
    duration: '',
    luggage: new Luggage(2, '', 1),
    extra: '',
    airlineId: 0,
    rate: 0,
    trip: 'One way',
    from: '',
    to: '',
    seats: new Array<Seat>(),
    stops: new Array<string>()
  }
  //za prihode

  daily2: boolean;
  weekly2: boolean;
  permonth2: boolean;
  myChart2: any;
  hours2: Array<number> = new Array<number>();
 
  ticketDaily2: Array<number> = new Array<number>(24);
  ticketWeekly2: Array<number> = new Array<number>(7);
  ticketMonthly2: Array<number> = new Array<number>(12);

  constructor(private route: Router, private airlineService: AirlineService, private fb:FormBuilder, private userService: UserService) { }

  ngOnInit(): void {
    this.loaded = false;
    this.ChangePasswordGroup = this.fb.group({
      'newPassword' : new FormControl ('', Validators.required),
      'repeatNewPassword' : new FormControl ('', Validators.required),
      'oldPassword' : new FormControl ('', Validators.required),
    }, {validator: this.checkIfMatchingPasswords('newPassword', 'repeatNewPassword')});
    this.ChangeUsernameGroup = this.fb.group({
      'username' : new FormControl ('', Validators.required),
      'currentPass' : new FormControl ('', Validators.required),
    }); 
    this.ChangeInformationGroup = this.fb.group({
      'fullName' : new FormControl ('', Validators.required),
      'address' : new FormControl ('', Validators.required),
      'email' : new FormControl ('', [Validators.required, Validators.email]),
      'passCurrent' : new FormControl ('', Validators.required),
    });
    this.ChangeCompanyInfoGroup = this.fb.group({
      'companyName' : new FormControl ('', Validators.required),
      'description' : new FormControl ('', Validators.required),
      'logoImage' : new FormControl (''),
    });

    this.userService.loadUser().subscribe(
      (res: any) => {
          this.user = res;
          this.idModel = new IdModel(parseInt(this.user.companyId.toString()), 0);
          this.ChangePasswordGroup = this.fb.group({
            'newPassword' : new FormControl ('', Validators.required),
            'repeatNewPassword' : new FormControl ('', Validators.required),
            'oldPassword' : new FormControl ('', Validators.required),
          }, {validator: this.checkIfMatchingPasswords('newPassword', 'repeatNewPassword')});
          this.ChangeUsernameGroup = this.fb.group({
            'username' : new FormControl (this.user.username, Validators.required),
            'currentPass' : new FormControl ('', Validators.required),
        });
          this.ChangeInformationGroup = this.fb.group({
            'fullName' : new FormControl (this.user.fullName, Validators.required),
            'address' : new FormControl (this.user.address, Validators.required),
            'email' : new FormControl (this.user.email, [Validators.required, Validators.email]),

            'passCurrent' : new FormControl ('', Validators.required),
          });
          this.airlineService.loadAirline(this.idModel).subscribe(
            (res: any) => {
      
                 this.company = res;
                 this.ShowDashboard();
                 this.ChangeCompanyInfoGroup = this.fb.group({
                  'companyName' : new FormControl (this.company.name, Validators.required),
                  'description' : new FormControl (this.company.description, Validators.required),
                  'logoImage' : new FormControl (''),
                });

                 this.wrongPassword = false;
                 this.usernameExists = false;
             
                 this.tickets = new Array<string>();
                 this.tickets[0] = 'One way';
                 this.tickets[1] = 'Round trip';
             
             
                 for (let i = 0; i < 24; i++) {
                   this.ticketDaily[i] = 0;
                 }
             
                 for (let i = 0; i < 7; i++) {
                   this.ticketWeekly[i] = 0;
                 }
             
                 for (let i = 0; i < 12; i++) {
                   this.ticketMonthly[i] = 0;
                 }
                 this.daily = true;
                 this.weekly = false;
                 this.permonth = false;
             
                 for (let i = 0; i < 24; i++) {
                   this.hours.push(i);
                 }
                 //za prihode
                 for (let i = 0; i < 24; i++) {
                   this.ticketDaily2[i] = 0;
                 }
             
                 for (let i = 0; i < 7; i++) {
                   this.ticketWeekly2[i] = 0;
                 }
             
                 for (let i = 0; i < 12; i++) {
                   this.ticketMonthly2[i] = 0;
                 }
                 this.daily2 = true;
                 this.weekly2 = false;
                 this.permonth2 = false;
                 this.drawCharts();
                 this.drawCharts2();

            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
        );
        this.loaded = true;
       },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
    
    this.sakriven = true;


    this.NewFlightGroup = this.fb.group({
      'from' : new FormControl ('', Validators.required),
      'to' : new FormControl ('', Validators.required),
      'departureDate' : new FormControl ('', Validators.required),
      'trip' : new FormControl ('', Validators.required),
      'lw' : new FormControl ('', Validators.required),
      'lq' : new FormControl ('', Validators.required),
      'ld' : new FormControl ('', Validators.required),
      'extra' : new FormControl ('', Validators.required),
      'price' : new FormControl ('', Validators.required),
      'departureTime' : new FormControl ('', Validators.required),
      'b' : new FormControl ('', Validators.required),
      'e' : new FormControl ('', Validators.required),
      'f' : new FormControl ('', Validators.required),
      'stops' : new FormControl ('', Validators.required),
      'duration' : new FormControl ('', Validators.required),
   });
   this.NewFlightGroup2 = this.fb.group({
    'from' : new FormControl ('', Validators.required),
    'to' : new FormControl ('', Validators.required),
    'departureDate' : new FormControl ('', Validators.required),
    'trip' : new FormControl ('', Validators.required),
    'lw' : new FormControl ('', Validators.required),
    'lq' : new FormControl ('', Validators.required),
    'ld' : new FormControl ('', Validators.required),
    'extra' : new FormControl ('', Validators.required),
    'price' : new FormControl ('', Validators.required),
    'departureTime' : new FormControl ('', Validators.required),
    'b' : new FormControl ('', Validators.required),
    'e' : new FormControl ('', Validators.required),
    'f' : new FormControl ('', Validators.required),
    'stops' : new FormControl ('', Validators.required),
    'duration' : new FormControl ('', Validators.required),
 });
  }
 

  CloseAll() {
    this.dashboardHidden = true;
    var dashboard = document.getElementById("dashboardRow");
    dashboard.style.backgroundColor = "transparent";
    this.statisticsHidden = true;
    var statistics = document.getElementById("statisticsRow");
    statistics.style.backgroundColor = "transparent"; 
    this.companyInfoHidden = true;
    var company = document.getElementById("companyInfoRow");
    company.style.backgroundColor = "transparent";  
    this.locationsHidden = true;
    var locs = document.getElementById("locationsRow");
    locs.style.backgroundColor = "transparent";  
    this.flightsHidden = true;
    var flights = document.getElementById("flightsRow");
    flights.style.backgroundColor = "transparent"; 
    this.pricesHidden = true;
    var prices = document.getElementById("pricesRow");
    prices.style.backgroundColor = "transparent"; 
    this.profileInfoHidden = true;
    var profile = document.getElementById("profileInfoRow");
    profile.style.backgroundColor = "transparent"; 
  }

  ShowDashboard() {
    this.CloseAll();
    this.dashboardHidden = false;
    document.getElementById("dashboardRow").style.backgroundColor = "gainsboro";
  }

  ShowStatistics() {
    this.CloseAll();
    this.statisticsHidden = false;
    document.getElementById("statisticsRow").style.backgroundColor = "gainsboro";
  }

  ShowCompanyInfo() {
    this.CloseAll();
    this.companyInfoHidden = false;
    document.getElementById("companyInfoRow").style.backgroundColor = "gainsboro";
  }

  ShowPrices() {
    this.CloseAll();
    this.pricesHidden = false;
    document.getElementById("pricesRow").style.backgroundColor = "gainsboro";
  }

  ShowFlights() {
    this.CloseAll();
    this.flightsHidden = false;
    document.getElementById("flightsRow").style.backgroundColor = "gainsboro";
  }

  ShowLocations() {
    this.CloseAll();
    this.locationsHidden = false;
    document.getElementById("locationsRow").style.backgroundColor = "gainsboro";
  }

  ShowProfileInfo() {
    this.CloseAll();
    this.profileInfoHidden = false;
    document.getElementById("profileInfoRow").style.backgroundColor = "gainsboro";
  }
  addToPopular(loc: Destination)
  {

    var model = 
    {
        Id: loc.id,

    }
    this.airlineService.AddToPopular(model).subscribe(
      (res: any) => {
        this.company = res;

      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });
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



  checkUsername() 
  {
    var model = {
      username: this.ChangeUsernameGroup.value.username
    } 
    this.userService.CheckUsername(model).subscribe(
      (res: any) => {

        if(res == true) {
          this.usernameExists = false;
          return true;
        }
        this.usernameExists = true;
        return false;
      },
      err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
          return false;
      });
      
  }

  ChangePassword() {
    this.CheckPassword(this.ChangePasswordGroup.value.oldPassword);
    if(this.successPass) {
      if(this.ChangePasswordGroup.valid) {
        var model = 
        {
            password: this.ChangePasswordGroup.value.oldPassword,
            newPassword: this.ChangePasswordGroup.value.newPassword
        }
        this.userService.ChangePassword(model).subscribe(
          (res: any) => {
            this.user = res;
            this.ChangePasswordGroup = this.fb.group({
              'newPassword' : new FormControl ('', Validators.required),
              'repeatNewPassword' : new FormControl ('', Validators.required),
              'oldPassword' : new FormControl ('', Validators.required),
            });
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          });
        document.getElementById('changePassError').style.color = 'green';
        document.getElementById('changePassError').textContent = 'Success!';
      }
      else {
        document.getElementById('changePassError').style.color = 'red';
        document.getElementById('changePassError').textContent = 'Passwords do not match';
      }
    }
    else {
      document.getElementById('changePassError').style.color = 'red';
      document.getElementById('changePassError').textContent = 'Current password is wrong';
    }
  }
  CheckPassword(username) 
  {
    
    var model = {
      username: username
    }
    
    this.userService.CheckPassword(model).subscribe(
      (res: any) => {
        if(res.ret == true)
        {
          this.successPass = true;
        } 
        else
        {
          this.successPass = false;
        }   
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
          this.successPass = false;
      });
      
  }
  changeUsername() {
   this.CheckPassword(this.ChangeUsernameGroup.value.currentPass);
   this.checkUsername();
   if(this.successPass) {
      if(!this.usernameExists && this.ChangeUsernameGroup.valid) {
        var model = {
          username: this.ChangeUsernameGroup.value.username
        }
        this.userService.ChangeUserName(model).subscribe(
          (res: any) => {
            this.user = res;
            this.ChangeUsernameGroup = this.fb.group({
              'username' : new FormControl ('', Validators.required),
              'currentPass' : new FormControl ('', Validators.required),
            });
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          });
        document.getElementById('changeUsernameError').style.color = 'green';
        document.getElementById('changeUsernameError').textContent = 'Success!';
      }
      else {
        document.getElementById('changeUsernameError').style.color = 'red';
        document.getElementById('changeUsernameError').textContent = 'Username is taken';
      }
    }
    else {
      document.getElementById('changeUsernameError').style.color = 'red';
      document.getElementById('changeUsernameError').textContent = 'Wrong password';
    }
    
  }

  changeInformation() {
    this.CheckPassword(this.ChangeInformationGroup.value.passCurrent);

    if(!this.successPass) {
      document.getElementById('changeInfoError').style.color = 'red';
      document.getElementById('changeInfoError').textContent = 'Wrong password';
      return;
    }
    if(this.ChangeInformationGroup.valid) {
      var fn = this.ChangeInformationGroup.value.fullName;
      var addr = this.ChangeInformationGroup.value.address;
      var email = this.ChangeInformationGroup.value.email;

      var model = 
      {
          fullName: fn,
          address: addr,
          email: email
      }

      this.userService.SaveNewAccountDetails(model).subscribe(
        (res: any) => {
            this.user = res;

        },
        err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
        });
        
      document.getElementById('changeInfoError').style.color = 'green';
      document.getElementById('changeInfoError').textContent = 'Success!';
    }
    else {
      document.getElementById('changeInfoError').style.color = 'red';
      document.getElementById('changeInfoError').textContent = 'Please enter valid data in all fields';
    }
  }

  saveCompanyChanges() {
    var name = this.ChangeCompanyInfoGroup.value.companyName;
    var desc = this.ChangeCompanyInfoGroup.value.description;
    var img = this.ChangeCompanyInfoGroup.value.logoImage;

    if(img == "")
        img = this.company.img;
    
    if(name != "" && desc !="") {
      var model = {
        id: this.company.id,
        name: name,
        description: desc,
        img: img
      }
      this.airlineService.UpdateCompany(model).subscribe(
        (res: any) => {
          this.company = res;
        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        });
  
      document.getElementById('companyError').style.color = 'green';
      document.getElementById('companyError').textContent = 'Success!';
    }
    else {
      document.getElementById('companyError').style.color = 'red';
      document.getElementById('companyError').textContent = 'Please enter valid data in all fields';
    }
  }

  addLocation() {
    var newLocName = ((document.getElementById("newLocationName") as HTMLInputElement).value);
    var des = ((document.getElementById("newLocationDesc") as HTMLInputElement).value);
    var imgInput = document.getElementById("newLocationImg");
    var newLocImg = (imgInput as HTMLInputElement).value;
    if(newLocImg == "") {
      alert('You must add image!');
    }
    else {
      var splits =  (newLocImg as string).split('\\');
      newLocImg = splits[splits.length-1];
    }
    

    var model = 
    {
      CompanyId: this.company.id,
        Name: newLocName,
        Description: des,
        Img: newLocImg
    }
    this.airlineService.AddNewDestination(model).subscribe(
      (res: any) => {
        this.company = res;
        document.getElementById("newLocField").style.display = 'none';

      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });

  }
  editLocation(idLoc: number) {
    document.getElementById(idLoc.toString()).style.display = 'block';
  }

  closeEditLocation(idLoc:number) {
    document.getElementById(idLoc.toString()).style.display = 'none';
  }

  saveEditLocation(loc: Destination) {
    
    var newLocImg = ((document.getElementsByName(loc.id.toString())[0] as HTMLInputElement).value);
    if(newLocImg == "") {
      newLocImg = loc.img;
    }
    else {
      var splits =  (newLocImg as string).split('\\');
      newLocImg = splits[splits.length-1];
    }
    var newLocName = ((document.getElementsByName(loc.id.toString())[1] as HTMLInputElement).value);
    var newLocDes = ((document.getElementsByName(loc.id.toString())[2] as HTMLInputElement).value);
    
    var model = 
    {
        Id: loc.id,
        Name: newLocName,
        Description: newLocDes,
        Img: newLocImg
    }
    this.airlineService.SaveEditLocation(model).subscribe(
      (res: any) => {
        this.company = res;
        document.getElementById(loc.id.toString()).style.display = 'none';

      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });


  }
  openAddField() {
    document.getElementById("newLocField").style.display = 'block';
  }
  removeLocation(loc: Destination)
  {

    var model = 
    {
        id: loc.id,
    }
    this.airlineService.RemoveLocation(model).subscribe(
      (res: any) => {
        this.company = res;

      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });
  }

  OpenAddFlight() {
    document.getElementById("newFlightDiv").style.display = 'block';
  }
  closeAddFlight()
  {
    document.getElementById("newFlightDiv").style.display = 'none';
  }
  convert(str: string) {
  var date = new Date(str),
    mnth = ("0" + (date.getMonth() + 1)).slice(-2),
    day = ("0" + date.getDate()).slice(-2);
  return [date.getFullYear(), mnth, day].join("-");
  }
  addReturnFlight()
  
  {
    var trip = this.NewFlightGroup2.value.trip;
    var lw = this.NewFlightGroup2.value.lw;
    var ld = this.NewFlightGroup2.value.ld;
    var lq = this.NewFlightGroup2.value.lq;

    var l = new Luggage(lw, ld, lq);
    var fr = this.NewFlightGroup2.value.f;
    var b = this.NewFlightGroup2.value.b;
    var e = this.NewFlightGroup2.value.e;
    var stopss = this.NewFlightGroup2.value.stops;

    var seats = new Array<Seat>();

    var stops = new Array<string>();

    // tslint:disable-next-line: prefer-for-of
   for (let s = 0; s < stopss.split(', ').length; s++)
    {
        var st = stopss.split(', ', stopss.split(', ').length)[s];
        stops.push(st);
    }
    let i = 0;
    for (i; i < b; i++)
    {
        seats.push(new Seat(Classes.First, this.traveller, i, false, false));
    }
    for (let j = 0; j < fr; j++)
    {
        seats.push(new Seat(Classes.Business, this.traveller, i, false, false));
        i++;
    }
    for (let z = 0; z < e; z++)
    {
        seats.push(new Seat(Classes.Economy, this.traveller, i, false, false));
        i++;
    }

    this.modelPom =
    {
      idCompany: this.company.name,
      departureDate: this.convert(this.NewFlightGroup2.value.departureDate).toString(),
      departureTime: this.NewFlightGroup2.value.departureTime,
      povratniLet: this.returnFlight,
      price: this.NewFlightGroup2.value.price,
      duration: this.NewFlightGroup2.value.duration,
      luggage: l,
      extra: this.NewFlightGroup2.value.extra,
      airlineId: this.company.id,
      rate: 0,
      trip: 'One way',
      from: this.NewFlightGroup2.value.from,
      to: this.NewFlightGroup2.value.to,
      seats: seats,
      stops: stops
    }
  }
  addFlight()
  {

   
    var trip2 = this.NewFlightGroup.value.trip;
 
    var lw2 = this.NewFlightGroup.value.lw;
    var ld2= this.NewFlightGroup.value.ld;
    var lq2 = this.NewFlightGroup.value.lq;

    var l2 = new Luggage(lw2, ld2, lq2);
    var fr2 = this.NewFlightGroup.value.f;
    var b2 = this.NewFlightGroup.value.b;
    var e2 = this.NewFlightGroup.value.e;
    var stopss2 = this.NewFlightGroup.value.stops;

    var seats2 = new Array<Seat>();

    var stops2 = new Array<string>();

    // tslint:disable-next-line: prefer-for-of
   for (let s = 0; s < stopss2.split(', ').length; s++)
    {
        var st = stopss2.split(', ', stopss2.split(', ').length)[s];
        stops2.push(st);
    }
    let j = 0;
    for (j; j < b2; j++)
    {
        seats2.push(new Seat(Classes.First, this.traveller, j, false, false));
    }
    for (let j = 0; j < fr2; j++)
    {
        seats2.push(new Seat(Classes.Business, this.traveller, j, false, false));
      
    }
    for (let z = 0; z < e2; z++)
    {
        seats2.push(new Seat(Classes.Economy, this.traveller, j, false, false));
  
    }



    var model =
    {
      idCompany: this.company.name,
      departureDate: this.convert(this.NewFlightGroup.value.departureDate.toString()).toString(),
      departureTime: this.NewFlightGroup.value.departureTime,
      povratniLet: this.modelPom,
      price: this.NewFlightGroup.value.price,
      duration: this.NewFlightGroup.value.duration,
      luggage: l2,
      extra: this.NewFlightGroup.value.extra,
      airlineId: this.company.id,
      trip: trip2,
      from: this.NewFlightGroup.value.from,
      to: this.NewFlightGroup.value.to,
      seats: seats2,
      stops: stops2
    }
    this.airlineService.AddNewFlight(model).subscribe(
      (res: any) => {
        this.company = res;
        document.getElementById("newFlightDiv").style.display = 'none';
        document.getElementById("newFlightDiv2").style.display = 'none';
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });
  }
  changed()
  {
    if (this.NewFlightGroup.value.trip == 'Round trip')
    {
      document.getElementById("newFlightDiv2").style.display = 'block';
    }
  }
  editFlight(id: number) {
    this.sakriven = false;
  }
  removeFlight(id: number) {

   var model = 
    {
        Id: id,
    }
    this.airlineService.RemoveFlight(model).subscribe(
      (res: any) => {
        this.company = res;

      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });
  }
  saveEditFlight(id: number) {
    var price = (<HTMLInputElement>document.getElementById("idprice")).value;
    var date = (<HTMLInputElement>document.getElementById("iddate")).value;
    var date2 = (<HTMLInputElement>document.getElementById("iddate2")).value;
    var duration = (<HTMLInputElement>document.getElementById("idduration")).value;
    var lug = (<HTMLInputElement>document.getElementById("idlug")).value;
    var extra = (<HTMLInputElement>document.getElementById("idextra")).value;
    if(date != "")
      date = this.convert(date).toString();

    var pomlug = parseInt(lug.toString());
    var pomlug2 = parseInt(price.toString());
    var model =
    {
      id: id,
      price: pomlug2,
      date: date,
      duration: duration,
      extra: extra,
      lug: pomlug,
      date2: date2
    }

    this.airlineService.EditFlight(model).subscribe(
      (res: any) => {
        this.company = res;
        this.sakriven = true;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      });

  }

  closeEditFlight(id:number) {
    document.getElementById(id.toString()).style.display = 'none';
  }
 

  drawCharts() {
    this.myChart = new Chart("myChart", {
      type: 'bar',
      data: {
        labels: this.hours,
        datasets: [{
          label: 'Tickets',
          data: this.ticketDaily,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });

    this.myChart = new Chart("myChart2", {
      type: 'bar',
      data: {
        labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
        datasets: [{
          label: 'Tickets',
          data: this.ticketWeekly,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });

    this.myChart = new Chart("myChart3", {
      type: 'bar',
      data: {
        labels: ['January', 'February', 'Mart', 'April', 'May', 'June', 'July', 'August',
          'September', 'October', 'November', 'December'],
        datasets: [{
          label: 'Tickets',
          data: this.ticketMonthly,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });
  }

  checkSelect() {
    let input = (<HTMLInputElement>document.getElementById("chartType")).value;
    console.log(input);
    if (input == "daily") {
      this.daily = true;
      this.weekly = false;
      this.permonth = false;
    }
    else if (input == "weekly") {
      this.daily = false;
      this.weekly = true;
      this.permonth = false;
    }
    else if (input == "permonth") {
      this.daily = false;
      this.weekly = false;
      this.permonth = true;

    }
  }

  submitInput() {
    for (let i = 0; i < 24; i++) {
      this.ticketDaily[i] = 0;
    }
    var input = (<HTMLInputElement>document.getElementById("dayTxt")).value;
    var date;
    this.company.flights.forEach(flight => {
      flight.soldTickets.forEach(ticket => {
        if (flight.departureDate != undefined) {
          if (input == formatDate(flight.departureDate, 'yyyy-MM-dd', 'en-US')) {
            this.ticketDaily[Number((flight.departureDate.split(' ')[1]).split(':')[0])]++;
          }
        }
      });
    });
    
    this.drawCharts()
  }

  submitInput2() {
    for (let i = 0; i < 7; i++) {
      this.ticketWeekly[i] = 0;
    }
    var input = (<HTMLInputElement>document.getElementById("weekTxt")).value;
    //console.log(input)
    var inputSplit1 = input.split('-');
    var year = inputSplit1[0];
    var inputSplit2 = input.split('W');
    var week = inputSplit2[1];
    var date = this.getDateOfISOWeek(week, year);
    var getDate = date.getDate();

    this.company.flights.forEach(flight => {
      flight.soldTickets.forEach(ticket => {
        if (flight.departureDate != undefined) {
          for (let i = 0; i < 7; i++) {
            date.setDate(getDate + i)
            console.log(date);
            if (formatDate(flight.departureDate, 'yyyy-MM-dd', 'en-US') ==
              formatDate(date, 'yyyy-MM-dd', 'en-US')) {
              this.ticketWeekly[new Date(flight.departureDate).getDay() - 1]++;
            }
          }
        }
      });
    });

    this.drawCharts()
  }

  submitInput3() {
    for (let i = 0; i < 12; i++) {
      this.ticketMonthly[i] = 0;
    }
    var input = (<HTMLInputElement>document.getElementById("monthTxt")).value;
    var inputSplit = input.split('-');

    this.company.flights.forEach(flight => {
      flight.soldTickets.forEach(ticket => {
        if (flight.departureDate != undefined) {
          if (new Date(flight.departureDate).getMonth() == Number(inputSplit[1]) - 1) {
            this.ticketMonthly[new Date(flight.departureDate).getMonth()]++;
          }
        }
      });
    });

    this.drawCharts();
  }

  getDateOfISOWeek(w, y) {
    var simple = new Date(y, 0, 1 + (w - 1) * 7);
    var dow = simple.getDay();
    var ISOweekStart = simple;
    if (dow <= 4)
      ISOweekStart.setDate(simple.getDate() - simple.getDay() + 1);
    else
      ISOweekStart.setDate(simple.getDate() + 8 - simple.getDay());
    return ISOweekStart;
  }



  ////za prihode 
  drawCharts2() {
    this.myChart = new Chart("myChart4", {
      type: 'bar',
      data: {
        labels: this.hours,
        datasets: [{
          label: 'Tickets prise',
          data: this.ticketDaily2,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });

    this.myChart = new Chart("myChart5", {
      type: 'bar',
      data: {
        labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
        datasets: [{
          label: 'Tickets prise',
          data: this.ticketWeekly2,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });

    this.myChart = new Chart("myChart6", {
      type: 'bar',
      data: {
        labels: ['January', 'February', 'Mart', 'April', 'May', 'June', 'July', 'August',
          'September', 'October', 'November', 'December'],
        datasets: [{
          label: 'Tickets prise',
          data: this.ticketMonthly2,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });
  }

  checkSelect2() {
    let input = (<HTMLInputElement>document.getElementById("chartType2")).value;
    console.log(input);
    if (input == "daily2") {
      this.daily2 = true;
      this.weekly2 = false;
      this.permonth2 = false;
    }
    else if (input == "weekly2") {
      this.daily2 = false;
      this.weekly2 = true;
      this.permonth2 = false;
    }
    else if (input == "permonth2") {
      this.daily2 = false;
      this.weekly2 = false;
      this.permonth2 = true;

    }
  }

  submitInput4() {
    for (let i = 0; i < 24; i++) {
      this.ticketDaily2[i] = 0;
    }
    var input = (<HTMLInputElement>document.getElementById("dayTxt2")).value;
    var date;
    this.company.flights.forEach(flight => {
      flight.soldTickets.forEach(ticket => {
        if (flight.departureDate != undefined) {
          if (input == formatDate(flight.departureDate, 'yyyy-MM-dd', 'en-US')) { 
            var dodaj = 0;
            if(ticket.discount != 0)
            {
              dodaj = flight.price - 20*flight.price/100;
            }
            else
            {
              dodaj = flight.price;
            }
            this.ticketDaily2[Number((flight.departureDate.split(' ')[1]).split(':')[0])] += dodaj;
          }
        }
      });
    });

    this.drawCharts2();
  }

  submitInput5() {
    for (let i = 0; i < 7; i++) {
      this.ticketWeekly2[i] = 0;
    }
    var input = (<HTMLInputElement>document.getElementById("weekTxt2")).value;
    //console.log(input)
    var inputSplit1 = input.split('-');
    var year = inputSplit1[0];
    var inputSplit2 = input.split('W');
    var week = inputSplit2[1];
    var date = this.getDateOfISOWeek(week, year);
    var getDate = date.getDate();

    this.company.flights.forEach(flight => {
      flight.soldTickets.forEach(ticket => {
        if (flight.departureDate != undefined) {
          for (let i = 0; i < 7; i++) {
            date.setDate(getDate + i)
            console.log(date);
            if (formatDate(flight.departureDate, 'yyyy-MM-dd', 'en-US') ==
              formatDate(date, 'yyyy-MM-dd', 'en-US')) {
                var dodaj = 0;
                if(ticket.discount != 0)
                {
                  dodaj = flight.price - 20*flight.price/100;
                }
                else
                {
                  dodaj = flight.price;
                }
              this.ticketWeekly2[new Date(flight.departureDate).getDay() - 1] += dodaj;
            }
          }
        }
      });
    });

    this.drawCharts2();
  }

  submitInput6() {
    for (let i = 0; i < 12; i++) {
      this.ticketMonthly2[i] = 0;
    }
    var input = (<HTMLInputElement>document.getElementById("monthTxt2")).value;
    var inputSplit = input.split('-');

    this.company.flights.forEach(flight => {
      flight.soldTickets.forEach(ticket => {
       if (flight.departureDate != undefined) {
          if (new Date(flight.departureDate).getMonth() == Number(inputSplit[1]) - 1) {
            var dodaj = 0;
            if(ticket.discount != 0)
            {
              dodaj = flight.price - 20*flight.price/100;
            }
            else
            {
              dodaj = flight.price;
            }
            this.ticketMonthly2[new Date(flight.departureDate).getMonth()] += dodaj;
          }
        }
      });
    });

    this.drawCharts2();
  }
}
