import { Component, OnInit, NgZone } from '@angular/core';
import { Airline } from 'src/app/entities/airline/airline';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { Flight } from 'src/app/entities/flight/fligh';
import { Classes } from 'src/app/entities/flight/class';
import { state, trigger, transition, style, animate } from '@angular/animations';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { FormBuilder, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { Polyline } from 'src/app/entities/user/traveller';
import { IdModel, FlightListingInfo, RatersInfo, TicketListingInfo } from 'src/app/entities/flight/flightListingInfo';
import { AirlineListingInfo } from 'src/app/entities/airline/airlineListingInfo';
import { Destination } from 'src/app/entities/destination/destination';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { DataService } from 'src/app/data.service';


interface Class {
  value: string;
  viewValue: string;
}
interface Ticket2 {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-airline',
  templateUrl: './airline.component.html',
  styleUrls: ['./airline.component.css'],
   styles: ['agm-map {height: 700px; width: 1200px;}']
})

export class AirlineComponent implements OnInit {

  displayedColumns = ['from', 'to', 'departureDate', 'duration', 'numOfPassengers', 'prise', 'numOfStops', 'option' ];
  public company = new AirlineListingInfo(new Array<FlightListingInfo>(), 0, 0 , "", new Array<Destination>(),
  new Array<Destination>(), new Array<RatersInfo>(), 0, new Array<TicketListingInfo>(),
  0, "", "", 0, "logof.jpg");
  dataSource: Array<FlightListingInfo>;
  mainDataSource: Array<FlightListingInfo>;
  expandedElement: FlightListingInfo | null;
  isAdmin: boolean;
  star1: number;
  star2: number;
  star3: number;
  star4: number;
  star5: number;
  idModel: IdModel;
  public zoom: number = 15;
  startLat : number;
  startLon : number;
  public polyline: Polyline;
  options : string[];
  isLoggedIn: string;
  busImgIcon : any = {url:"assets/logof.jpg", scaledSize: {width: 50, height: 50}};

  previous : any;
  user: User;
  registerForm = this.fb.group({
    From: ['', Validators.pattern],
    To: ['', Validators.pattern],
    Clas: ['', Validators.pattern],
    Prise: ['', Validators.pattern],
    Trip: ['', Validators.pattern],
    Date: ['', Validators.pattern],
    Date2: ['', Validators.pattern],
  });

  tickets: Ticket2[] = [
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
  id: number;
  // tslint:disable-next-line: max-line-length
  constructor(private ngZone: NgZone, private route: Router, private router: ActivatedRoute, private fb: FormBuilder, private airlineService: AirlineService, private userService: UserService,
    private data: DataService) {
   }

  ngOnInit(): void {
    this.data.sharedMessage.subscribe(message => this.isLoggedIn = message);



    // tslint:disable-next-line: no-string-literal
    this.router.params.subscribe(params => { this.id = params['idA']; });
    this.polyline = new Polyline([], 'blue', { url:"assets/logof.jpg", scaledSize: {width: 50, height: 50}});

    this.idModel = new IdModel(parseInt(this.id.toString()), 0);

    this.airlineService.loadAirline(this.idModel).subscribe(
      (res: any) => {

           this.company = res;
           this.dataSource = this.company.flights;
           this.company.img = 'air-serbia.jpg';
           this.mainDataSource = this.company.flights;
           this.user = this.userService.isLoggedUser();

          this.startLat = this.company.lat;
          this.startLon = this.company.lon;

          this.dataSource = [];
          for(let i = 0;i < this.mainDataSource.length;i++)
          {
              if(this.provjeriDatum(this.mainDataSource[i].departureDate))
              {
                this.dataSource.push(this.mainDataSource[i]);
              }
          }
         this.mainDataSource = this.dataSource;
          this.star1 = 1;
          this.star2 = 2;
          this.star3 = 3;
          this.star4 = 4;
          this.star5 = 5;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
  );

    }
    


  clickedMarker(infoWindow){
    if(this.previous)
    {
      this.previous.close();
    }
    this.previous = infoWindow;
  }
  seeall(): void
  {
    this.route.navigate(['/destinations', {passedId: this.company.id, desName: ''}]);
  }
  fastreserve(): void
  {
       this.route.navigate(['/airlines/airline/tickets', {idA: this.company.id}]);
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
  search(): void
  {

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
  odrediste(name: string): void
  {
     this.route.navigate(['/destinations', {passedId: this.company.id, desName: name}]);
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
  rate()
  {
      alert('sdfgh');
  }
}



