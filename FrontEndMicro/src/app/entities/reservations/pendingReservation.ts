export class PendingReservation {
    id: number;
    dateMade: string;
    status: string;
    description: string;

    // tslint:disable-next-line: max-line-length
    constructor(id: number, dm: string, st: string, desc: string){
        this.id = id;
        this.dateMade = dm;
        this.status = st;
        this.description = desc;
    }
}
