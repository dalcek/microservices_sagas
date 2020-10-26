import { Component, OnInit } from '@angular/core';
import { Car } from 'src/app/entities/car/car';

interface Type {
  value: string;
  viewValue: string;
}

@Component({
    selector: 'app-car',
    templateUrl: './car.component.html',
    styleUrls: ['./car.component.css']
})

export class CarComponent implements OnInit {
  types: Type[] = [
    {value: 'economy-0', viewValue: 'Economy' },
    {value: 'compact-1', viewValue: 'Compact'},
    {value: 'midsize-2', viewValue: 'Mid-size'},
    {value: 'standardsize-3', viewValue: 'Standard-size'},
    {value: 'fullsize-4', viewValue: 'Full-size'},
    {value: 'luxory-5', viewValue: 'Luxory'},
    {value: 'suv-6', viewValue: 'SUV'},
    {value: 'van-7', viewValue: 'Van'},
    {value: 'minivan-8', viewValue: 'Mini Van'},
    {value: 'convertible-9', viewValue: 'Convertible'},
    {value: 'pickup-10', viewValue: 'Pickup'}
  ];

    constructor() { }

    ngOnInit(): void {
    }
}
