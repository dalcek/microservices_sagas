import { Car } from '../car/car';
import { User } from '../user/user';
import { CarRentalComponent } from 'src/app/components/carRental/carRental.component';
import { Time } from '@angular/common';

export class CarReservation {
    carEn: Car;
    Car: string;
    user: User;
    dates: Array<string>;
    Company: string;
    companyID: number;
    From: string;
    To: string;
    Total:number;
    ratedCar: number;
    ratedCompany: number;
    pickUpLocation: string;
    pickUpTime: string;
    dropOffLocation:string;
    dropOffTime: string;
    timeStamp: Date;


    constructor(car: Car, carName:string, user: User, dates: Array<string>, companyId: number, compName:string, from:string, to:string, total:number, ratedCar:number, ratedComp: number, pLoc: string, pTime:string, dLoc:string, dTime:string, timestamp:Date) {
        this.carEn = car;
        this.Car = carName;
        this.user = user;
        this.dates = dates;
        this.companyID = companyId;
        this.Company = compName;
        this.From = from;
        this.To = to;
        this.Total = total;
        this.ratedCar = ratedCar;
        this.ratedCompany = ratedComp;
        this.pickUpLocation = pLoc;
        this.pickUpTime = pTime;
        this.dropOffLocation = dLoc;
        this.dropOffTime = dTime;
        this.timeStamp = timestamp;
    }
}
