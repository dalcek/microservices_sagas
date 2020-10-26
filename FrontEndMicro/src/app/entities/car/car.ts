export class Car {
    id: number;
    brand: string;
    model: string;
    year: number;
    pricePerDay: number;
    type: string;
    passengers: number;
    location: string;
    rented: Array<string>;
    image: string;
    agencyID: number;
    stars: Array<number>;
    avrageStar: number;

    constructor(id: number, agencyID:number, brand: string, model: string, year: number, pricePerDay: number, type:string, passengers:number, location: string, rented: Array<string>, image:string, stars: Array<number>, avrStar: number) {
        this.id = id;
        this.agencyID = agencyID;
        this.brand = brand;
        this.model = model;
        this.year = year;
        this.pricePerDay = pricePerDay;
        this.type = type;
        this.passengers = passengers;
        this.location = location;
        this.rented = rented;
        this.image = image;
        this.stars = stars;
        this.avrageStar = avrStar;
    }
}