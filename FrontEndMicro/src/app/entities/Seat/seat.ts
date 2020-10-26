import { User } from '../user/user';
import { Traveller } from '../user/traveller';
import { Classes } from '../flight/class';

export class Seat {
    id: number;
    taken: boolean;
    isSelected: boolean;
    traveller: Traveller;
    type: Classes;

    // tslint:disable-next-line: max-line-length
    constructor(tp: Classes, tr: Traveller, id: number, taken: boolean, isSelected: boolean){
        this.id = id;
        this.taken = taken;
        this.isSelected = isSelected;
        this.traveller = tr;
        this.type = tp;
    }
}
