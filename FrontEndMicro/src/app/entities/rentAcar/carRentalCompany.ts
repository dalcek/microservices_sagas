import { Car } from '../car/car';
import { Address } from 'src/app/entities/address/address'
import { CarReservation } from '../carReservation/carReservation';
import { ExtraAmenity } from './ExtraAmenity';

export class CarRentalCompany {
    ID: number;
    Name: string;
    MainLocation: Address;
    Locations: Array<Address>;
    Description: string;
    Image: string;
    Cars: Array<Car>;
    Ratings: Array<Rating>;
    AvrageRating: number;
    Extras: Array<ExtraAmenity>;
    DiscountedCars: Array<QuickReservation>;
    Reservations: Array<CarReservation>
    Activated: boolean;
    constructor(
        activated:boolean, 
        stars: Array<Rating>, 
        id: number, 
        name: string, 
        address: Address, 
        description: string, 
        rating: number, 
        locs: Array<Address>, 
        img: string, 
        cars: Array<Car>, 
        extras: Array<ExtraAmenity>, 
        discounted:Array<QuickReservation>,
        reservations: Array<CarReservation>
        ) 
        {
        this.Activated = activated;
        this.Ratings = stars;
        this.ID = id;
        this.Name = name;
        this.MainLocation = address;
        this.Description = description;
        this.AvrageRating = rating;
        this.Locations = locs;
        this.Image = img;
        this.Cars = cars;
        this.Extras = extras;
        this.DiscountedCars = discounted;
        this.Reservations = reservations;
    }
}

export class Rating {
    Id: number;
    Rating: number;
}

export class QuickReservation {
    Id: number;
    DiscountedCar: Car;
    Dates: Array<MyDate>
}

export class MyDate {
    Id: number;
    DateStr: string;
}
