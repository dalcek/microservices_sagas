export class Destination {
    name: string;
    img: string;
    id: number;
    description: string;

    constructor(id: number, name: string, img: string, des: string) {
        this.id = id;
        this.name = name;
        this.img  = img;
        this.description = des;
    }
}
