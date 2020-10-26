import { UserType } from '../userType/userType';
import { Flight } from '../flight/fligh';
import { Car } from '../car/car';

export class Traveller {
    id: string;
    idUser: string;
    firstName: string;
    lastName: string;
    email: string;
    passport: string;


    // tslint:disable-next-line: max-line-length
    constructor(uy: string, ps: string, id: string, firstName: string, lastName: string, email: string) {
        this.id = id;
        this.idUser = uy;
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;

        this.passport = ps;
    }
}
export class GeoLocation {
    
    constructor(public latitude: number, public longitude: number) {
      
    }
}
export class Polyline {
    public path: GeoLocation[];
    public color: string;
    public icon: any;

    constructor(path: GeoLocation[], color: string, icon: any){
        this.color = color;
        this.path = path;
        this.icon = icon;
    }

    addLocation(location: GeoLocation){
        this.path.push(location);
    }
}
