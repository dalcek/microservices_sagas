
export class CarModelAdmin {
    id: number;
    brand: string;
    model: string;
    year: number;
    price: number;   
    type: string;
    passengers: number;
    location: string;
    image: string;
    avrageRating: number;
    rentedDates: Array<string>;
    discount: Array<QuickReservationModel>;
    available: string;
    constructor(id: number, brand: string, model: string, year: number, pricePerDay: number, type:string, passengers:number, location: string, avrStar: number, img: string, rented:Array<string>) {
        this.id = id;
        this.brand = brand;
        this.model = model;
        this.year = year;
        this.price = pricePerDay;
        this.type = type;
        this.passengers = passengers;
        this.location = location;
        this.image = img;
        this.avrageRating = avrStar;
        this.rentedDates = rented;
    }

}

export class QuickReservationModel {
    id: number;
    carId: number;
    from: string;
    to: string;
}