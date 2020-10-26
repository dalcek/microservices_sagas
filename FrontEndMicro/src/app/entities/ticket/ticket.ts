import { User } from '../user/user';
import { Traveller } from '../user/traveller';
import { Classes } from '../flight/class';
import { Seat } from '../Seat/seat';
import { Flight } from '../flight/fligh';

export class Ticket {
    id: number;
    flighth: Flight;
    seat: Seat;
    discount: number;

    // tslint:disable-next-line: max-line-length
    constructor(id: number, flight: Flight, seat: Seat, dis: number){
        this.id = id;
        this.flighth = flight;
        this.seat = seat;
        this.discount = dis;
    }
}
