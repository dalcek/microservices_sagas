import { Destination } from '../destination/destination';
import { Flight } from '../flight/fligh';
import { Ticket } from '../ticket/ticket';

export class Airline {
    id: number;
    name: string;
    address: string;
    Lon: number;
    Lat: number;
    description: string;
    destinations: Array<Destination>;
    populardestinations: Array<Destination>;
    img: string;
    flights: Array<Flight>;
    rating: number;
    raters: Array<number>;
    admin: number;
    fastTickets: Array<Ticket>;
    
    // tslint:disable-next-line: max-line-length
    constructor(lon: number, lat: number, ft: Array<Ticket>, admin: number, rtr: Array<number>,  id: number, name: string, address: string, description: string, destinations: Array<Destination>, popdest: Array<Destination>, flights: Array<Flight>, img: string, rating: number) {
        this.Lon = lon;
        this.Lat = lat;
        this.id = id;
        this.fastTickets = ft;
        this.raters = rtr;
        this.name = name;
        this.admin = admin;
        this.address = address;
        this.description = description;
        this.destinations = destinations;
        this.img = img;
        this.populardestinations = popdest;
        this.flights = flights;
        this.rating = rating;
    }
}
