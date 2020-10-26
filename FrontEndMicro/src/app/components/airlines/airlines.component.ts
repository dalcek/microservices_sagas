import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Airline } from 'src/app/entities/airline/airline';
import { Router } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { FormGroup, FormBuilder, AbstractControl } from '@angular/forms';
import { style, trigger, animate, state, transition } from '@angular/animations';
import { Flight } from 'src/app/entities/flight/fligh';
import { MatTableDataSource } from '@angular/material/table';
import { AirlineListingInfo } from 'src/app/entities/airline/airlineListingInfo';
import { FlightListingInfo, IdModel } from 'src/app/entities/flight/flightListingInfo';

@Component({
  selector: 'app-airlines',
  templateUrl: './airlines.component.html',
  styleUrls: ['./airlines.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
]
})

export class AirlinesComponent implements OnInit {
  searchCompaniesForm: FormGroup;
  idInfo: IdModel;
  expandedElement: FlightListingInfo | null;
  searchString: string;
  flight: FlightListingInfo;
  columnsToDisplay = ['idCompany', 'departureDate', 'price'];
  dataSource: MatTableDataSource<FlightListingInfo>;
  formControl: AbstractControl;
  public airlines = new Array<AirlineListingInfo>();
  flightPom: Array<FlightListingInfo>;
  mainDataSource: MatTableDataSource<FlightListingInfo>;
  // tslint:disable-next-line: max-line-length
  constructor( private changeDetectorRefs: ChangeDetectorRef, private formBuilder: FormBuilder, private route: Router, private airlineService: AirlineService, private _formBuilder: FormBuilder)
  {  
  }
  
  ngOnInit(): void {
    this.searchCompaniesForm = this._formBuilder.group({
      airline1Name: [''],
      airline2Name: [''],
      from: [''],
      to: ['']
  });
  this.formControl = this.formBuilder.group({
    price: '',
    idCompany: '',
  });
    this.airlineService.getAllCompanies().subscribe(
      (res: any) => {
          this.airlines = res;

          
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
    this.airlineService.getAllFlights().subscribe(
      (res: any) => {
        this.dataSource = new MatTableDataSource<FlightListingInfo>(res);
        this.mainDataSource = new MatTableDataSource<FlightListingInfo>(res);

        this.dataSource = res;
           this.mainDataSource = res;
           this.dataSource = new MatTableDataSource<FlightListingInfo>();
           for(let i = 0;i < res.length;i++)
           {
             this.dataSource.data.push(this.mainDataSource[i]);
              //  if(this.provjeriDatum(this.mainDataSource[i].departureDate))
              //  {
              //    this.dataSource.data.push(this.mainDataSource[i]);
              //  }
           }
          this.mainDataSource = this.dataSource;
     this.dataSource.filterPredicate = ((data, filter) => {
      const a = !filter.price || data.price <= filter.price;
      const b = !filter.idCompany || data.idCompany.toLowerCase().includes(filter.idCompany);
      return a && b ;
    }) as (PeriodicElement, string) => boolean;


   this.formControl.valueChanges.subscribe(value => {
      const filter = {...value, idCompany: value.idCompany.trim().toLowerCase()} as string;
      this.dataSource.filter = filter;
    });

        // tslint:disable-next-line: prefer-for-of
      for (let i = 0; i < this.airlines.length; i++)
      {
          Math.round(this.airlines[i].rating);
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
  searchCompanies() {
    var loc1 = this.searchCompaniesForm.value.airline1Name;
    var loc2 = this.searchCompaniesForm.value.airline2Name;
    var from = this.searchCompaniesForm.value.from;
    var to = this.searchCompaniesForm.value.to;


    this.dataSource.data = new Array<FlightListingInfo>();
    this.airlineService.searchCompanies(this.searchCompaniesForm.value).subscribe(
      (res: any) => {

        this.dataSource = res;
        this.mainDataSource = res;
        this.dataSource = new MatTableDataSource<FlightListingInfo>();
        for(let i = 0;i < res.length;i++)
        {
            if(this.provjeriDatum(this.mainDataSource[i].departureDate))
            {
              this.dataSource.data.push(this.mainDataSource[i]);
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

    // tslint:disable-next-line: prefer-for-of
   /* for (let i = 0; i < this.mainDataSource.data.length; i++)
    {
      // tslint:disable-next-line: triple-equals
      if (loc1 == '' || this.mainDataSource.data[i].from.name.toLowerCase() == loc1.toLowerCase())
      {
          if (loc2 == '' || this.mainDataSource.data[i].to.name.toLowerCase() == loc2.toLowerCase())
          {
              if (from == null || from == '' || this.mainDataSource.data[i].departureDate >= from)
              {
                   this.dataSource.data.push(this.mainDataSource.data[i]);
              
              }
          }
      }
   }*/

}

rent(flightId: FlightListingInfo)
  {
    var path = '/airlines/' + flightId.airlineId + '/airline/' + flightId.id + '/flight';
    this.route.navigate([path]);

  }
  applyFilter(event: Event) {
    
    const filterValue = (event.target as HTMLInputElement).value;

    this.dataSource.filter = filterValue.trim().toLowerCase();
    var filteredData = this.dataSource.filteredData;
    this.dataSource.data = filteredData;
  }
  applyFilter2(event: Event) {
    this.dataSource.filterPredicate = (data: FlightListingInfo, filter: string) => {
      return data.price.toString() == filter;
     };  
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue;
  }
  
}



