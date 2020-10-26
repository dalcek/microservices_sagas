import { Luggage } from './luggage';
import { Trip } from './trip';
import { Destination } from '../destination/destination';
import { Ticket } from '../ticket/ticket';
import { Seat } from '../Seat/seat';
import { Traveller } from '../user/traveller';
import { NumberValueAccessor } from '@angular/forms';

export class FlightListingInfo {
    id: number;
    idCompany: string;
    departureDate: string;
    povratniLet: FlightListingInfo;
    price: number;
    duration: string;
    luggage: Luggage;
    extra: string;
    numOfPassengers: number;
    airlineId: number;
    rate: number;
    trip: Trip;
    from: Destination;
    to: Destination;
    seats: Array<Seat>;
    stops: Array<Destination>;
    raters: Array<RatersInfo>;
    soldTickets: Array<TicketListingInfo>;
    constructor(soldTickets: Array<TicketListingInfo>, raters: Array<RatersInfo>, stops: Array<Destination>, f: Destination, to: Destination, seats: Array<Seat>, r: number, t: Trip, AirlineId: number, lug: Luggage, ex: string, numOfPassengers: number, duration: string, id: number, idCompany: string, departureDate: string,povratniLet: FlightListingInfo, price: number) {
        this.id = id;
        this.from = f;
        this.to = to;
        this.seats = seats;

        this.stops = stops;
        this.raters = raters;
        this.soldTickets = soldTickets;


        this.rate = r;
        this.trip = t;
        this.airlineId = AirlineId;
        this.extra = ex;
        this.luggage = lug;
        this.idCompany = idCompany;
        this.departureDate = departureDate;
        this.povratniLet = povratniLet;
        this.price = price;
        this.duration = duration;
        this.numOfPassengers = numOfPassengers;
    }
}
export class FlightListingInfo2 {
    id: number;

 
    constructor(f: number) {
        this.id = f;
    }
}
export class IdModel {
    Id: number;
    pom: number;
    constructor(id: number, pm: number) {
        this.Id = id;
        this.pom = pm;
    }
}
export class RatersInfo{
    id: number;
    rate: number;
    constructor(id: number, rate: number) {
        this.id = id;
        this.rate = rate;
    }
}    
export class AddTravellerModel{
    seatId: number;
    traveller: Traveller;
    constructor(id: number, rate: Traveller) {
        this.seatId = id;
        this.traveller = rate;
    }
}

export class FinishModel
{ 
    seat: Seat;
    s: number;
    b: number;
    g: number;
    d: number;
 
    constructor(tic: Seat, si:number, br: number, go: number, disc: number) {
    this.seat = tic;
    this.s = si;
    this.b = br;
    this.g = go;
    this.d = disc;
    }
}


export class TicketListingInfo
{ 
    seat: Seat;
    discount: number;
    flight: FlightTicket;
    id: number;
    constructor(tic: Seat, d: number, f: FlightTicket, id: number) {
        this.seat = tic;
        this.discount = d;
        this.id = id;
        this.flight = f;
    }
}
export class FlightTicket
{
    id: number;
    idCompany: string;
    departureDate: string;
    price: number;
    duration: string;
    trip: Trip;
    from: Destination;
    to: Destination;

    constructor(f: Destination, to: Destination, r: number, t: Trip, duration: string, id: number, idCompany: string, departureDate: string, price: number) {
        this.id = id;
        this.from = f;
        this.to = to;
        this.trip = t;

        this.idCompany = idCompany;
        this.departureDate = departureDate;

        this.price = price;
        this.duration = duration;
    }
}