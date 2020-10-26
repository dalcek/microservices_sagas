import { Car } from '../car/car';
import { Address } from 'src/app/entities/address/address'
import { ExtraAmenity } from './ExtraAmenity';

export class RentAcarModel {
    id: number;
    name: string;
    mainLocation: Address;
    description: string;
    rating: number;
    locations: Array<Address>;
    img: string;
    extras: Array<ExtraAmenity>;
    constructor(id: number, name: string, address: Address, description: string, rating: number, locs: Array<Address>, img: string, extras: Array<ExtraAmenity>) {
        this.id = id;
        this.name = name;
        this.mainLocation = address;
        this.description = description;
        this.rating = rating;
        this.locations = locs;
        this.img = img;
        this.extras = extras;
    }
}
