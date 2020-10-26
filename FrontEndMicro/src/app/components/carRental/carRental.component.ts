import { Component, OnInit, Inject, Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators}  from '@angular/forms';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { Car } from 'src/app/entities/car/car';
import { CarModel } from 'src/app/entities/car/carModel';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UserService } from 'src/app/Services/userService';
import { Polyline } from 'src/app/entities/user/traveller'
import { RentAcarModel } from 'src/app/entities/rentAcar/rentAcarModel';

interface Type {
    value: string;
    viewValue: string;
  }

@Component({
    selector: 'app-carRental',
    templateUrl: './carRental.component.html',
    styleUrls: ['./carRental.component.css'],
    animations: [
        trigger('detailExpand', [
          state('collapsed', style({height: '0px', minHeight: '0'})),
          state('expanded', style({height: '*'})),
          transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
        ]),
    ]
})

export class CarRentalComponent implements OnInit {
    company: RentAcarModel;
    id: string;
    idModel = {
        id: 0
    }
    discountedCars: Array<CarModel>;
    dataSource: Array<CarModel>;
    expandedElement: CarModel | null;
    columnsToDisplay = ['brand', 'model', 'year', 'price'];
    searchCarsForm: FormGroup;
    searchModel = {
        companyId: 0,
        dateFrom: '',
        dateTo: '',
        location: '',
        returnLocation: '',
        type: '',
        brand: '',
        model: '',
        year: '',
        price: '',
        passengers: ''
    }
    types: Type[] = [
        {value: '', viewValue: '--'},
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

    tableHidden: boolean;
    loadingHidden: boolean;
    noResultsHidden: boolean;
    
    startLat:number;
    startLon:number;
    zoom: number;
    public polyline: Polyline;
    options : string[];
    busImgIcon : any = {url:"assets/carMarker.png", scaledSize: {width: 30, height: 30}};

    formatLabel(value: number) {
        if (value >= 1) {
          return '€' + value;
        }
        return value;
    }

    constructor(private fb: FormBuilder, private route: Router, private router: ActivatedRoute, private carRentalService: CarRentalService, public dialog: MatDialog, private userService: UserService) 
    { 
        router.params.subscribe(params => { this.id = params['idComp']; });
        var idNum = parseInt(this.id);
        this.idModel.id = idNum;
        this.carRentalService.getCompanyProfile(this.idModel).subscribe(
            (res: any) => {
            this.company = res;
            this.discountedCars = new Array<CarModel>();
            this.startLat = this.company.mainLocation.latitude;
            this.startLon = this.company.mainLocation.longitude;
            this.zoom = 12;
            this.polyline = new Polyline([], 'blue', { url:"assets/carMarker.png", scaledSize: {width: 30, height: 30}});
            this.searchCarsForm = this.fb.group({
                dateFrom: ['', Validators.required],
                dateTo: ['', Validators.required],
                location: [this.company.mainLocation.fullAddress, Validators.required],
                returnLocation: [this.company.mainLocation.fullAddress],
                type: [''],
                brand: [''],
                model: [''],
                year: [''],
                price: [''],
                passengers: [''],
            });
            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
        );
    }

    ngOnInit(): void {
        
        //this.company = this.carRentalService.loadCompany(this.id);
        //this.dataSource = this.company.cars;
        this.tableHidden = true;
        this.loadingHidden = true;
        this.noResultsHidden = true;
        
        
    }

    SearchTable(): void {
        if(this.searchCarsForm.valid) {
            
            var date1 = this.searchCarsForm.value.dateFrom;
            var date2 = this.searchCarsForm.value.dateTo;
            var location = this.searchCarsForm.value.location;
            var retLoc = this.searchCarsForm.value.returnLocation;
            var y = this.searchCarsForm.value.year;
            var type = this.searchCarsForm.value.type;
            var brand = this.searchCarsForm.value.brand;
            var model = this.searchCarsForm.value.model;
            var pass = this.searchCarsForm.value.passengers;
            var price = this.searchCarsForm.value.price;

            if(new Date(date1) < new Date(date2) && new Date(date1) >= new Date(Date.now()) && new Date(date1) >= new Date(Date.now()) ) {
                this.loadingHidden = false;
                this.noResultsHidden = true;
                this.tableHidden = true;

                this.searchModel.brand = brand;
                this.searchModel.dateFrom = date1.toDateString();
                this.searchModel.dateTo = date2.toDateString();
                this.searchModel.location = location;
                this.searchModel.model = model;
                this.searchModel.passengers = pass;
                this.searchModel.price = price.toString();
                this.searchModel.returnLocation = retLoc;
                this.searchModel.type = type;
                this.searchModel.year = y;
                this.searchModel.companyId = this.idModel.id;

                this.dataSource = [];

                this.carRentalService.getCarsSearch(this.searchModel).subscribe(
                    (res: any) => {
                        this.loadingHidden = true;
                        this.dataSource = res;
                        if(this.dataSource.length == 0) {
                            this.noResultsHidden = false;
                        }
                        else {
                            this.tableHidden = false;
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
            else
            alert("invalid dates");
            
        }
        
    }

    RentACar(id: number) {
        localStorage.setItem('dateFrom', this.searchModel.dateFrom);
        localStorage.setItem('dateTo', this.searchModel.dateTo);
        if(this.searchCarsForm.value.returnLocation == "") {
            localStorage.setItem('retLoc', this.searchCarsForm.value.location)
        }
        else {
            localStorage.setItem('retLoc', this.searchCarsForm.value.returnLocation)
        }
        var path = '/carCompanies/'+ this.company.id + '/carCompany/'+ id +'/rent';
        this.route.navigate([path]);
    }

    ShowDiscountedDiv() {
        document.getElementById("discountedDiv").style.display = 'block';
        this.carRentalService.getDiscountedCars(this.company.id).subscribe(
            (res: any) => {
                this.discountedCars = res;
            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
        );
    }

    CloseDiscountedDiv() {
        document.getElementById("discountedDiv").style.display = 'none';
    }
}
