
import { CarModel } from '../car/carModel';
import { ExtraAmenity } from '../rentAcar/ExtraAmenity';

export class CarReservationModel {
    id: number;
    car: CarModel;
    dateFrom: string;
    dateTo: string;
    company: string;
    companyId: number;
    total: number;
    ratedCar: number;
    ratedCompany: number;
    pickUpLocation: string;
    pickUpTime: string;
    dropOffLocation:string;
    dropOffTime: string;
    timeStamp: string;
    extras: Array<ExtraAmenity>


    constructor(id: number, car: CarModel, companyId: number, compName:string, from:string, to:string, total:number, ratedCar:number, ratedComp: number, pLoc: string, pTime:string, dLoc:string, dTime:string, timestamp:string, extras: Array<ExtraAmenity>) {
        this.id = id;
        this.car = car;
        this.companyId = companyId;
        this.company = compName;
        this.dateFrom = from;
        this.dateTo = to;
        this.total = total;
        this.ratedCar = ratedCar;
        this.ratedCompany = ratedComp;
        this.pickUpLocation = pLoc;
        this.pickUpTime = pTime;
        this.dropOffLocation = dLoc;
        this.dropOffTime = dTime;
        this.timeStamp = timestamp;
        this.extras = extras;
    }
}
