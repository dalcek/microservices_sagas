import { CompanyInfoModel } from '../rentAcar/companyInfoModel';
import { Address } from '../address/address';
import { QuickReservation } from '../rentAcar/carRentalCompany';

export class CarRentalAdminModel {
    fullName: string;
    address: string;
    birthday: string;
    companyId: number;
    username: string;
    email: string;
    verifiedEmail: boolean;
    changedPassword: boolean;
    companyInfo: CompanyInfoModel;
    locations: Array<Address>;
    

    // tslint:disable-next-line: max-line-length
    constructor(
        FullName: string,
        Username: string,
        Email: string,
        Address: string,
        Birthday: string,
        CompanyId: number,
        VerifiedEmail: boolean,
        ChangedPassword: boolean,
        info: CompanyInfoModel,
        locs: Array<Address>) {

        this.username = Username;
        this.email = Email;
        this.verifiedEmail = VerifiedEmail;
        this.fullName = FullName;
        this.address = Address;
        this.birthday = Birthday;
        this.companyId = CompanyId;
        this.changedPassword = ChangedPassword;
        this.companyInfo = info;
        this.locations = locs;
    }
}

