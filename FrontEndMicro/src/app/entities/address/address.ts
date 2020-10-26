export class Address {
    fullAddress: string;
    longitude:number;
    latitude:number 

    constructor(addr:string, lat:number, lon:number,) {
        this.fullAddress = addr;
        this.longitude = lon;
        this.latitude = lat;
    }
}