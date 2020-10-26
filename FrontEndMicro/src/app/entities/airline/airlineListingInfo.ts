import { Destination } from '../destination/destination';
import { RatersInfo, FlightListingInfo, TicketListingInfo } from '../flight/flightListingInfo';
import { Ticket } from '../ticket/ticket';

export class AirlineListingInfo {
    id: number;
    name: string;
    mainLocation: string;
    rating: number;
    img: string;

    
    lon: number;
    lat: number;
    description: string;
    destinations: Array<Destination>;
    popDestinaations: Array<Destination>;

    raters: Array<RatersInfo>;
    admin: number;
    fastTickets: Array<TicketListingInfo>;
    flights: Array<FlightListingInfo>;

    constructor (fli?: Array<FlightListingInfo>, Lon?: number, Lat?: number, Description?: string, Destinations?: Array<Destination>, PopDestinaations?: Array<Destination>,
        Raters?: Array<RatersInfo>,
        
        Admin?: number,
        FastTickets?: Array<TicketListingInfo>, id?: number, name?: string, address?: string,
        rating?: number, img?: string) {
        this.id = id;
        this.name = name;
        this.flights = fli;
        this.mainLocation = address;
        this.rating = rating;
        this.img = img;
        this.lon = Lon;
        this.lat = Lat;
        this.description = Description;
        this.destinations = Destinations;
        this.popDestinaations = PopDestinaations;
    
        this.raters = Raters;
        this.admin = Admin;
        this.fastTickets = FastTickets;
    }
}
