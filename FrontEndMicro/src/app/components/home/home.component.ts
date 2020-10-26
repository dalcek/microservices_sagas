import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, Validators } from '@angular/forms';
import { AirlineService } from 'src/app/Services/airlineService';
import { Airline } from 'src/app/entities/airline/airline';
import { Flight } from 'src/app/entities/flight/fligh';
import { Car } from 'src/app/entities/car/car';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { UserService } from 'src/app/Services/userService';
import { Router } from '@angular/router';
import { LogInComponent } from 'src/app/components/logIn/logIn.component';
import { HeaderComponent } from '../navigation/header/header.component';
import { DataService } from "src/app/data.service";
import { Classes } from 'src/app/entities/flight/class';
import { CarModel } from 'src/app/entities/car/carModel';
import { FlightListingInfo } from 'src/app/entities/flight/flightListingInfo';
import * as jwt_decode from "jwt-decode";

interface Ticket {
  value: string;
  viewValue: string;
}

interface Class {
  value: string;
  viewValue: string;
}

interface Type {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  animations: [
      trigger('detailExpand', [
        state('collapsed', style({height: '0px', minHeight: '0'})),
        state('expanded', style({height: '*'})),
        transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
      ]),
  ]
})
export class HomeComponent implements OnInit {
  loggedIn: string;
  dates:Array<string>;
  returnLocation: string;
  @Output() myEvent = new EventEmitter<string>();

  displayedColumns = ['from', 'to', 'departureDate', 'duration', 'numOfPassengers', 'prise', 'numOfStops', 'option' ];
  company: Airline;
  dataSource: Array<FlightListingInfo>;
  mainDataSource: Array<FlightListingInfo>;
  expandedElement: FlightListingInfo | null;
  registerForm = this.fb.group({
    From: ['', Validators.pattern],
    Clas: ['', Validators.pattern],
    To: ['', Validators.pattern],
    Prise: ['', Validators.pattern],
    Trip: ['', Validators.pattern],
    Date: ['', Validators.pattern],
    Date2: ['', Validators.pattern],
  });

  carsSource: Array<CarModel>;
  allCars: Array<CarModel>;
  expandedCarElement: CarModel | null;
  carsColumnsToDisplay = ['brand', 'model', 'year', 'price'];
  searchCarsForm = this.fb.group({
    PickUpLocation: ['', Validators.required],
    PickUpDate: ['', Validators.required],
    DropOffLocation: [''],
    DropOffDate: ['', Validators.required],
    CarType: [''],
  })

  tickets: Ticket[] = [
    {value: '0', viewValue: 'One Way'},
    {value: '1', viewValue: 'Round Trip'},
    {value: '', viewValue: 'All'}
  ];
  classes: Class[] = [
    {value: '0', viewValue: 'Economy'},
    {value: '1', viewValue: 'Business'},
    {value: '2', viewValue: 'First'},
    {value: '', viewValue: 'All'},
  ];
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

  searchModel = {
    dateFrom: '',
    dateTo: '',
    location: '',
    returnLocation: '',
    type: ''
  }

  searchResultsHidden: boolean;
  flightsHidden: boolean;
  carsHidden: boolean;
  notRegisteredHidden: boolean;
  loadingHidden: boolean;
  noResultHidden: boolean;

  // tslint:disable-next-line: max-line-length
  constructor(private dataService: DataService, private modalService: NgbModal,private route: Router, private fb: FormBuilder, private airlineService: AirlineService, private userService: UserService, private carRentalService: CarRentalService) { }

  ngOnInit(): void {
    this.dataService.sharedMessage.subscribe(message => this.loggedIn = message);
    this.dates = [];

    this.searchResultsHidden = true;
    this.flightsHidden = true;
    this.carsHidden = true;
    this.notRegisteredHidden = true;
    this.loadingHidden = true;
    this.noResultHidden = true;
    // this.dataSource = this.airlineService.loadAllFlights();
    // this.mainDataSource = this.airlineService.loadAllFlights();
    //this.allCars = this.carRentalService.loadAllCars();
    //this.carsSource = this.allCars;
    
  

    this.carsSource = this.allCars;
  }


searchFlightsClicked(): void
{
    this.searchResultsHidden = false;
    this.flightsHidden = false;
    this.carsHidden = true;
    var From = this.registerForm.value.From;
    var To = this.registerForm.value.To;
    var prise = this.registerForm.value.Prise;
    var tr = this.registerForm.value.Trip;
    var cls = this.registerForm.value.Clas;

    var date1 = this.registerForm.value.Date;
  if (this.registerForm.value.Trip == "0")
  {
    this.registerForm.value.Trip = "One_way";
  }
  else if (this.registerForm.value.Trip == "1")
  {
    this.registerForm.value.Trip = "Round_trip";
  }

  if (this.registerForm.value.Clas == "0")
  {
    this.registerForm.value.Clas = "Economy";
  }
  else if (this.registerForm.value.Clas == "1")
  {
    this.registerForm.value.Clas = "Business";
  }
  else if (this.registerForm.value.Clas == "2")
  {
    this.registerForm.value.Clas = "First";

  } 
    this.dataSource = new Array<FlightListingInfo>();
    this.airlineService.searchFlights(this.registerForm.value).subscribe(
      (res: any) => {

        this.dataSource = res;
        this.mainDataSource = res;
        this.dataSource = [];
        for(let i = 0;i < this.mainDataSource.length;i++)
        {
            if(this.provjeriDatum(this.mainDataSource[i].departureDate))
            {
              this.dataSource.push(this.mainDataSource[i]);
            }
        }
       this.mainDataSource = this.dataSource;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
  );


  }
  provjeriDatum(date): boolean
{

  var dani = date.split(' ')[0].split('/')[0];
  var mjes = date.split(' ')[0].split('/')[1];
  var god = date.split(' ')[0].split('/')[2];
  var pom = new Date();
  pom.setFullYear(god,mjes - 1,dani);
  pom.setHours(date.split(' ')[1].split(':')[0]);
  pom.setMinutes(date.split(' ')[1].split(':')[1]);

  var dat = new Date();
    if(pom <= dat)
      return false;
    return true;
}
  details(f: Flight)
  {
    var com = this.airlineService.loadAirlineByName(f.idCompany);
    var path = '/airlines/' + com.id + '/airline/' + f.id + '/flight';
    this.route.navigate([path]);
  }
  searchCarsClicked() {
    if(this.searchCarsForm.valid) {

      this.searchModel.dateFrom = this.searchCarsForm.value.PickUpDate.toDateString();
      this.searchModel.dateTo = this.searchCarsForm.value.DropOffDate.toDateString();
      this.searchModel.location = this.searchCarsForm.value.PickUpLocation;
      this.searchModel.returnLocation = this.searchCarsForm.value.DropOffLocation;
      this.searchModel.type = this.searchCarsForm.value.CarType;

      if((new Date(this.searchModel.dateFrom) > new Date(this.searchModel.dateTo)) || new Date(this.searchModel.dateFrom) < new Date(Date.now()) || new Date(this.searchModel.dateTo) < new Date(Date.now())){
        alert("invalid dates");
        return;
      }

      this.searchResultsHidden = false;
      this.flightsHidden = true;
      this.loadingHidden = false;
      this.noResultHidden = true;
      this.carsHidden = true;

      

      this.carRentalService.getCarsSearchHome(this.searchModel).subscribe(
        (res: any) => {
            this.loadingHidden = true;
            this.carsSource = res;
            if(this.carsSource.length == 0) {
                this.noResultHidden = false;
            }
            else {
                this.carsHidden = false;
            }
        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        }
    );

    }
    
  }

  isRegistered()
  {
    return this.userService.isRegistered();
  }

  formatLabel(value: number) {
    if (value >= 1) {
      return '€' + value;
    }
    return value;
  }

  isRegistered2() {
    var token = localStorage.getItem('token');
    var decoded = this.getDecodedAccessToken(token);
    if (token == null || decoded.exp >= Date.now()) {
        return false;
    }
    if(decoded.Roles == "RegisteredUser") 
    {
        return true;
    }
    else
      return false;
  }
  getDecodedAccessToken(token: string): any {
    try{
        return jwt_decode(token);
    }
    catch(Error){
        return null;
    }
  }

  RentACar(car: CarModel) {
    if(this.isRegistered2()) {
      this.userService.saveDates(this.dates);
      this.userService.saveRetLocation(this.returnLocation);
      var path = '/carCompanies/'+ car.companyId + '/carCompany/'+ car.id +'/rent';
      this.route.navigate([path]);
    } else {
      this.carsHidden = true;
      this.notRegisteredHidden = false;
    }
  }

  BackToCars() {
    this.notRegisteredHidden = true;
    this.carsHidden = false;
  }

  SignIn() {
    this.BackToCars();
    const modalRef = this.modalService.open(LogInComponent);
      modalRef.componentInstance.loggedIn = this.loggedIn;
      modalRef.result.then((result) => {
        if (result) { 
          this.loggedIn = "true"; 
          this.SendMessage("true");
        }
    });
  }

  SignUp() {

  }

  SendMessage(message: string) {
    this.dataService.nextMessage(message);
  }
}
