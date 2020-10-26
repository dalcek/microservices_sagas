import { UserType } from '../userType/userType';
import { Flight } from '../flight/fligh';
import { Car } from '../car/car';
import { CarReservation } from '../carReservation/carReservation'
import { Ticket } from '../ticket/ticket';
import { CarReservationModel } from '../carReservation/carReservationModel';
import { TicketListingInfo } from '../flight/flightListingInfo';

export class UserModel {
    fullName: string;
    address: string;
    birthday: string;
    passport: string;
    flights: Array<Ticket>;
    oldFlights: Array<Ticket>;
    rentedCars: Array<Object>;
    flightRequests: Array<TicketListingInfo>;
    //RentedCars: Array<CarReservation>;
    friends: Array<UserModel>;
    friendRequests: Array<FriendRequestReceived>;
    sentRequests: Array<FriendRequestSent>;
    notifications: Array<Notification>;
    points: number;
    companyId: number;
    id: string;
    username: string;
    email: string;
    changedPassword: boolean;
    socialUser: boolean;


    // tslint:disable-next-line: max-line-length
    constructor(of: Array<Ticket>,
        FullName: string,
        Id: string,
        Username: string,
        Email: string,
        Address: string,
        Birthday: string,
        Passport: string,
        Flights: Array<Ticket>,
        RentedCars: Array<Object>,
        FlightRequests: Array<TicketListingInfo>,
        //RentedCars: Array<CarReservation>;
        Friends: Array<UserModel>,
        FriendRequests: Array<FriendRequestReceived>,
        SentRequests: Array<FriendRequestSent>,
        Notifications: Array<Notification>,
        Points: number,
        CompanyId: number,
        cp:boolean, 
        su: boolean) {

        this.oldFlights = of;
        this.id = Id;
        this.username = Username;
        this.email = Email;
        
        this.fullName = FullName;
        this.notifications = Notifications;
        this.address = Address;
        this.birthday = Birthday;
        this.passport = Passport;
        this.flights = Flights;
        this.flightRequests = FlightRequests;
        this.friends = Friends;
        this.friendRequests = FriendRequests;
        this.points = Points;
        this.sentRequests = SentRequests;

        this.rentedCars = RentedCars;
        this.companyId = CompanyId;
        this.changedPassword = cp;
        this.socialUser = su;
    }
}

export class TicketInvitation
{
    Id: number;
    Ticket: Ticket;
    constructor(i: number, t: Ticket)
    {
        this.Id = i;
        this.Ticket = t;
    }
}

export class SearchUserModel
{
    passport: string;
    constructor(i: string)
    {
        this.passport = i;
    }
}

export class FriendRequestReceived
{
    id: number;
    user: UserModel;
    constructor(i: number, t: UserModel)
    {
        this.id = i;
        this.user = t;
    }
}

export class FriendRequestSent
{
    id: number;
    user: UserModel;
    constructor(i: number, t: UserModel)
    {
        this.id = i;
        this.user = t;
    }
}
export class Notification
{
    Id: number;
    Text: string;
    constructor(i: number, t: string)
    {
        this.Id = i;
        this.Text = t;
    }
}

export class SendRequestModel
{
    idFrom: string;
    idTo: string;
    constructor(i: string, t: string)
    {
        this.idFrom = i;
        this.idTo = t;
    }
}
