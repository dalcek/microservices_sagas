import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Flight } from 'src/app/entities/flight/fligh';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FriendsComponent } from '../inviteFriends/inviteFriends.component';
import { Airline } from 'src/app/entities/airline/airline';
import { Destination } from 'src/app/entities/destination/destination';
import { Validators, FormBuilder } from '@angular/forms';
import { Classes } from 'src/app/entities/flight/class';
import { IdModel, FlightListingInfo, RatersInfo, TicketListingInfo } from 'src/app/entities/flight/flightListingInfo';
import { AirlineListingInfo } from 'src/app/entities/airline/airlineListingInfo';
import { Ticket } from 'src/app/entities/ticket/ticket';

interface Ticket2 {
  value: string;
  viewValue: string;
}
interface Class {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-destinations',
  templateUrl: './destinations.component.html',
  styleUrls: ['./destinations.component.css']
})
export class DestinationsComponent implements OnInit {
  displayedColumns = ['from', 'to', 'departureDate', 'duration', 'numOfPassengers', 'prise', 'numOfStops', 'option' ];
  dataSource: Array<FlightListingInfo>;
  mainDataSource: Array<FlightListingInfo>;
  expandedElement: FlightListingInfo | null;
  company = new AirlineListingInfo(new Array<FlightListingInfo>(), 0, 0 , "", new Array<Destination>(),
  new Array<Destination>(), new Array<RatersInfo>(), 0, new Array<TicketListingInfo>(),
  0, "", "", 0, "logof.jpg");
  id: number;
  idModel: IdModel;
  desName: string;
  mySubscription: any;
  registerForm = this.fb.group({
    From: ['', Validators.pattern],
    Clas: ['', Validators.pattern],
    Date2: ['', Validators.pattern],
    To: ['', Validators.pattern],
    Prise: ['', Validators.pattern],
    Trip: ['', Validators.pattern],
    Date: ['', Validators.pattern],

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
  // tslint:disable-next-line: max-line-length
  constructor( private route: Router, private fb: FormBuilder, private router: ActivatedRoute, private airlineService: AirlineService, private changeDetectorRefs: ChangeDetectorRef) { 
    this.route.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    this.mySubscription = this.route.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.route.navigated = false;
      }
    });

  }
  klik(): void
  {
  }
  switchLink(idF: number)
  {
    this.route.navigate(['/flight', {passedId: idF, passedId2: this.company.id}]);
  }
  odrediste(name: string): void
  {
    this.route.navigate(['/destinations', {passedId: this.id, desName: name}]);
  }
  ngOnInit(): void {

    this.desName = '';
    // tslint:disable-next-line: no-string-literal
    this.router.params.subscribe(params => { this.id = params['passedId']; this.desName = params['desName']; });

    this.idModel = new IdModel(parseInt(this.id.toString()), 0);
    this.airlineService.loadAirline(this.idModel).subscribe(
      (res: any) => {
        this.company = res;
        //this.dataSource = this.airlineService.flightsForDestination(this.id, this.desName);
        this.mainDataSource = this.company.flights;
    
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
  );



    this.registerForm.setValue({
      From: '',
      Clas: '',
      To: this.desName,
      Prise: '',
      Trip: '',
      Date: '',
      Date2: ''
    });
  }
  search(): void
  {
    var From = this.registerForm.value.From;
    var To = this.registerForm.value.To;
    var prise = this.registerForm.value.Prise;
    var tr = this.registerForm.value.Trip;
    var date = this.registerForm.value.Date;
    var date2 = this.registerForm.value.Date2;
    var cls = this.registerForm.value.Clas;
    this.dataSource = [];
    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < this.mainDataSource.length; i++){
        // tslint:disable-next-line: triple-equals

        if (From == '' || this.mainDataSource[i].from.name.toLowerCase() == From.toLowerCase())
        {
          if (To == '' || this.mainDataSource[i].to.name.toLowerCase() == To.toLowerCase())
            {

              if (prise == '' || this.mainDataSource[i].price <= prise)
                {
                  if (tr == '' || this.mainDataSource[i].trip == tr)
                  {
                   
                    if (date == null || date == '' || this.mainDataSource[i].departureDate >= date)
                    {

                        if(cls == '')
                        {
                          this.dataSource.push(this.mainDataSource[i]);
                        }
                        else if (cls == Classes.Business)
                        {
                          // tslint:disable-next-line: prefer-for-of
                          for (let j = 0; j < this.mainDataSource[i].seats.length; j++)
                          {
                            if (this.mainDataSource[i].seats[j].type == Classes.Business)
                            {
                              this.dataSource.push(this.mainDataSource[i]);
                              break;
                            }
                          }
                        }
                        else if (cls == Classes.Economy)
                        {
                          // tslint:disable-next-line: prefer-for-of
                          for (let j = 0; j < this.mainDataSource[i].seats.length; j++)
                          {
                            if (this.mainDataSource[i].seats[j].type == Classes.Economy)
                            {
                              this.dataSource.push(this.mainDataSource[i]);
                              break;
                            }
                          }
                        }
                        else if (cls == Classes.First)
                        {
                          // tslint:disable-next-line: prefer-for-of
                          for (let j = 0; j < this.mainDataSource[i].seats.length; j++)
                          {
                            if (this.mainDataSource[i].seats[j].type == Classes.First)
                            {
                              this.dataSource.push(this.mainDataSource[i]);
                              break;
                            }
                          }
                        }
                      
                    }
                  }
             }               
          }
        }
      }
    }
  formatLabel(value: number) {
    if (value >= 1) {
      return '€' + value;
    }
    return value;
  }
}



