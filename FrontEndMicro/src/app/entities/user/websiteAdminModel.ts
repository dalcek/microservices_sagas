import { CompanyInfoModel } from '../rentAcar/companyInfoModel';
import { Address } from '../address/address';

export class WebsiteAdminModel {
    fullName: string;
    address: string;
    birthday: string;
    username: string;
    email: string;
    verifiedEmail: boolean;
    changedPassword: boolean;
    mainAdmin: boolean;
    websiteAdministrators: Array<OtherAdmin>;
    rentAcars: Array<CarCompanyModel>;
    airlines: Array<AirlineCompanyModel>;
    discount: Discount
    constructor(
        FullName: string,
        Username: string,
        Email: string,
        Address: string,
        Birthday: string,
        VerifiedEmail: boolean,
        ChangedPassword: boolean,
        Main: boolean,
        others: Array<OtherAdmin>,
        rentAcars: Array<CarCompanyModel>,
        airlines: Array<AirlineCompanyModel>
        ) 
    {

        this.username = Username;
        this.email = Email;
        this.verifiedEmail = VerifiedEmail;
        this.fullName = FullName;
        this.address = Address;
        this.birthday = Birthday;
        this.changedPassword = ChangedPassword;
        this.mainAdmin = Main;
        this.websiteAdministrators = others;
        this.rentAcars = rentAcars;
        this.airlines = airlines;
    }
}

export class OtherAdmin {
    username: string;
    fullname: string;
    email: string;

}

export class CarCompanyModel {
    id: number;
    name: string;
    image: string;
    admins: Array<string>;
}

export class AirlineCompanyModel {
    id: number;
    name: string;
    image: string;
    admins: Array<string>;
}

export class Discount {
    bronzeTier: number;
    silverTier: number;
    goldTier: number;
    discountPercent: number;

}

