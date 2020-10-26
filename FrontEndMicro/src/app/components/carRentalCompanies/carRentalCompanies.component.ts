import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators}  from '@angular/forms';
import { Car } from 'src/app/entities/car/car';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { AirlineService } from 'src/app/Services/airlineService';
import { Address } from 'src/app/entities/address/address'
import { RentAcarListingInfo } from 'src/app/entities/rentAcar/rentAcarListingInfo';
import { HttpHeaders } from '@angular/common/http';

@Component({
    selector: 'app-carRentalCompanies',
    templateUrl: './carRentalCompanies.component.html',
    styleUrls: ['./carRentalCompanies.component.css']
})

export class CarRentalCompaniesComponent implements OnInit {
    searchCompaniesForm: FormGroup;
    companies: Array<RentAcarListingInfo>;
    
    searchModel = {
        companyName: '',
        location: '',
        from: '',
        to: '',
    }

    constructor(private router: Router, private carRentalService: CarRentalService, private _formBuilder: FormBuilder, private airlineService: AirlineService) {}
    
    ngOnInit(): void {
        this.searchCompaniesForm = this._formBuilder.group({
            companyName: [''],
            location: [''],
            from: [''],
            to: ['']
        });
        
        this.carRentalService.getAllCompanies().subscribe(
            (res: any) => {
                this.companies = res;
            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
          );
    }

    switchLink(id: number)
    {
        this.router.navigate(['/carCompany', {passedId: id}]);
    }

    searchCompanies() {
        this.searchModel.companyName = this.searchCompaniesForm.value.companyName;
        this.searchModel.location = this.searchCompaniesForm.value.location;
        this.searchModel.from = this.searchCompaniesForm.value.from;
        this.searchModel.to = this.searchCompaniesForm.value.to;

        if(new Date(this.searchModel.from) <= new Date(this.searchModel.to)) {
            this.carRentalService.searchCompanies(this.searchModel).subscribe(
                (res: any) => {
                    this.companies = res;
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
        alert('invalid dates');
        
        
    }
}
