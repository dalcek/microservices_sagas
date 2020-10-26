import { Destination } from '../destination/destination';
import { Time } from '@angular/common';
import { Seat } from '../Seat/seat';
import { Classes } from './class';
import { Trip } from './trip';

export class Luggage {
    weight: number;
    dimensions: string;
    quantity: number;


    // tslint:disable-next-line: max-line-length
    constructor(w: number, d: string, q: number) {
        this.weight = w;
        this.dimensions = d;
        this.quantity = q;

    }
}
