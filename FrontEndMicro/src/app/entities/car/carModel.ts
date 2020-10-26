export class CarModel {
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
    companyId: number;
    constructor(id: number, brand: string, model: string, year: number, pricePerDay: number, type:string, passengers:number, location: string, avrStar: number, img: string) {
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
    }

}