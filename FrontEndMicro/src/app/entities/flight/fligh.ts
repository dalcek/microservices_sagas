import { Destination } from '../destination/destination';
import { Time } from '@angular/common';
import { Seat } from '../Seat/seat';
import { Trip } from './trip';
import { Luggage } from './luggage';
import { Ticket } from '../ticket/ticket';

export class Flight {
    id: number;
    idCompany: string;
    from: Destination;
    to: Destination;
    departureDate: Date;
    povratnilet: Flight;
    prise: number;
    duration: string;
    numOfPassengers: number;
    seats: Array<Seat>;
    stops: Array<Destination>;
    trip: Trip;
    raters: Array<number>;
    rate: number;
    luggage: Luggage;
    extra: string;
    soldTickets: Array<Ticket>;

    // tslint:disable-next-line: max-line-length
    constructor(st: Array<Ticket>, povr: Flight, e: string, lg: Luggage, nfr: number, rt: Array<number>, idCom: string, tr: Trip, seats: Array<Seat>, id: number, from: Destination, to: Destination, departureDate: Date, numOfPassengers: number, duration: string, prise: number, stops: Array<Destination>) {
        this.rate = nfr;
        this.soldTickets = st;
        this.luggage = lg;
        this.extra = e;
        this.raters = rt;
        this.idCompany = idCom;
        this.trip = tr;
        this.id = id;
        this.from = from;
        this.to = to;
        this.departureDate = departureDate;
        this.povratnilet = povr;
        this.numOfPassengers = numOfPassengers;
        this.duration = duration;
        this.prise = prise;
        this.stops = stops;
        this.seats = seats;
    }
}
