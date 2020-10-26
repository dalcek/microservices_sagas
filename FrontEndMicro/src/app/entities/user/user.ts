import { UserType } from '../userType/userType';
import { Flight } from '../flight/fligh';
import { Car } from '../car/car';
import { CarReservation } from '../carReservation/carReservation'
import { Ticket } from '../ticket/ticket';

export class User {
    id: number;
    firstName: string;
    lastName: string;
    address: string;
    username: string;
    password: string;
    birthday: Date;
    email: string;
    passport: string;
    type: UserType;
    myFlights: Array<Ticket>;
    oldFlights: Array<Ticket>;
    flightRequests: Array<Ticket>;
    rentedCars: Array<CarReservation>;
    friends: Array<User>;
    friendRequests: Array<User>;
    sentRequests: Array<User>;
    notifications: Array<string>;
    points: number;
    
    // tslint:disable-next-line: max-line-length
    constructor(flr: Array<Ticket>, old: Array<Ticket>, ntf: Array<string>, sr: Array<User>, ps: string, bs: Date, fr: Array<User>, id: number, firstName: string, lastName: string, address: string, username: string, password: string, email: string, type: UserType, myFlights: Array<Ticket>, rentedCars: Array<CarReservation>, p:number) {
        this.notifications = ntf;
        this.flightRequests = flr;
        this.oldFlights = old;
        this.birthday = bs;
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.address = address;
        this.username = username;
        this.password = password;
        this.email = email;
        this.type = type;
        this.myFlights = myFlights;
        this.rentedCars = rentedCars;
        this.friendRequests = fr;
        this.passport = ps;
        this.sentRequests = sr;
        this.points = p;
    }
}
