import { Address } from '../address/address';
import { CarModelAdmin } from '../car/carModelAdmin';
import { ExtraAmenity } from './ExtraAmenity';
import { QuickReservation } from './carRentalCompany';

export class CompanyInfoModel {
    name: string;
    description: string;
    logo: string;
    locations: LocationsModel;
    cars: Array<CarModelAdmin>;
    extras: Array<ExtraAmenity>;
    activated: boolean;
    ratings: Array<number>;
    dailyReservations: Array<number>;
    weeklyReservations: Array<number>;
    monthlyReservations: Array<number>;
    dailyLabels: Array<string>;
    weeklyLabels: Array<string>;
    monthlyLabels: Array<string>;
    

    constructor(n:string, des:string, l:string, lm:LocationsModel, cars:Array<CarModelAdmin>, extras: Array<ExtraAmenity>) {
        this.name = n;
        this.description = des;
        this.logo = l;
        this.locations = lm;
        this.cars = cars;
        this.extras = extras;
    }
}

export class LocationsModel {
    mainLocation: Address;
    locations: Array<Address>;
    
    constructor(addr:Address, locs:Array<Address>) {
        this.mainLocation = addr;
        this.locations = locs;
    }
}