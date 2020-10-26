export class RentAcarListingInfo {
    Id: number;
    Name: string;
    MainLocation: string;
    Rating: number;
    Img: string;
    constructor(id: number, name: string, address: string, rating: number, img: string) {
        this.Id = id;
        this.Name = name;
        this.MainLocation = address;
        this.Rating = rating;
        this.Img = img;
    }
}
