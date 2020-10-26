import { Injectable } from '@angular/core';
import { Time } from '@angular/common';
import { User } from 'src/app/entities/user/user';
import { Flight } from '../entities/flight/fligh';
import { UserType } from '../entities/userType/userType';
import { Car } from '../entities/car/car';
import { CarReservation } from 'src/app/entities/carReservation/carReservation'
import { Ticket } from '../entities/ticket/ticket';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
   })

export class UserService {
    readonly BaseURI = 'http://localhost:5005/api';

    
    zahtjevi_pom: Array<User>;
    loggedUser: User;
    logged: boolean;
    userDates: Array<string>;
    userRetLocation: string;
    constructor(private http: HttpClient) {
        this.userDates = new Array<string>();
    }


    isLoggedUser()
    {
        return this.loggedUser;
    }

 
    isAdmin()
    {
        if (this.logged == true)
        {
            if (this.loggedUser.type == UserType.Admin)
            {
                return true;
            }
        }
        return false;
    }
    isRegistered()
    {
        if (this.logged == true)
        {
            if (this.loggedUser.type == UserType.Registered)
            {
                return true;
            }
        }
        return false;
    }
    logOut()
    {
        this.logged = false;
    }

    saveDates(dates: Array<string>) {
        this.userDates = dates;
    }

    getUserDates() {
        return this.userDates;
    }

    saveRetLocation(retLoc: string) {
        this.userRetLocation = retLoc;
    }

    getUserRetLocation() {
        return this.userRetLocation;
    }

    isLoggedIn():boolean {
        return this.logged;
    }

    getCurrentRole() {
        return this.loggedUser.type.toString();
    }

    login(formData) {
        return this.http.post(this.BaseURI + '/AppUser/Login', formData);
    }

    register(formData1) {
        return this.http.post(this.BaseURI + '/AppUser/Register', formData1);
    }
    loadUser() {
        return this.http.get(this.BaseURI + '/AppUser/LoadUser');
    }
    loadFlightRequests() {
        return this.http.get(this.BaseURI + '/AppUser/LoadFlightRequests');
    }
    loadUserById(id) {
        return this.http.post(this.BaseURI + '/AppUser/LoadUserById', id);
    }   
    CancelFlightRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/CancelFlightRequest', model);
    }
    CancelFlight(model) {
        return this.http.post(this.BaseURI + '/AppUser/CancelFlight', model);
    }
    AcceptFlightRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/AcceptFlightRequest', model);
    }
    loadPeople() {
        return this.http.get(this.BaseURI + '/AppUser/loadPeople');
    }
    sendRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/SendRequest', model);
    }
    findUserByPassport(sm) {
        return this.http.post(this.BaseURI + '/AppUser/SearchUserByPassport',sm);
    }
    finish(sm) {
        return this.http.post(this.BaseURI + '/AppUser/Finish',sm);
    }
    acceptRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/AcceptRequest', model);
    }

    CancelRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/CancelRequest', model);
    }
    RemoveFriend(model) {
        return this.http.post(this.BaseURI + '/AppUser/RemoveFriend', model);
    }
    CheckUsername(model) {
        return this.http.post(this.BaseURI + '/AppUser/CheckUsername', model);
    }
    CheckPassword(model) {
        return this.http.post(this.BaseURI + '/AppUser/CheckPassword', model);
    }
    ChangeUserName(model) {
        return this.http.post(this.BaseURI + '/AppUser/ChangeUserName', model);
    }
    loadFlightById(idModel) {

        return this.http.post(this.BaseURI + '/AppUser/LoadFlight', idModel);
    }
    ChangePassword(model) {
        return this.http.post(this.BaseURI + '/AppUser/ChangePassword', model);
    }
    SaveNewAccountDetails(model) {
        return this.http.post(this.BaseURI + '/AppUser/SaveNewAccountDetails', model);
    }   

    IsAdmin(model) {
        return this.http.post(this.BaseURI + '/AppUser/IsAdmin', model);
    }
    SetPoints() {
        return this.http.get(this.BaseURI + '/AppUser/SetPoints');
    }
    FastReserve(model) {
        return this.http.post(this.BaseURI + '/AppUser/FastReserve', model);
    }
    
    loadCarRentalAdmin() {
        return this.http.get(this.BaseURI + '/AppUser/LoadCarRentalAdmin');
    }
    SaveAdminAccountDetails(name:string, bd:string, addr:string, pass:string, email:string){
        var model = {
            name : name,
            bd: bd.toString(),
            addr: addr,
            password: pass,
            email: email
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeAdminAccountDetails', model);
    }
    SaveNewUsernameAdmin(username:string, pass:string) {
        var model = {
            username: username,
            password: pass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeAdminUsername', model);
    }
    SaveNewPasswordAdmin(pass:string, newPass:string){
        var model = {
            password: pass,
            newPassword: newPass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeAdminPassword', model);
    }
    loadWebsiteAdmin() {
        return this.http.get(this.BaseURI + '/AppUser/LoadWebsiteAdmin');
    }
    SaveWAdminAccountDetails(name:string, bd:string, addr:string, pass:string, email:string) {
        var model = {
            name : name,
            bd: bd.toString(),
            addr: addr,
            password: pass,
            email: email
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeWAdminAccountDetails', model);
    }
    SaveNewUsernameWAdmin(username:string, pass:string) {
        var model = {
            username: username,
            password: pass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeWAdminUsername', model);
    }
    SaveNewPasswordWAdmin(pass:string, newPass:string){
        var model = {
            password: pass,
            newPassword: newPass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeWAdminPassword', model);
    }
    AddNewWebAdmin(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddNewWebAdmin', model);
    }
    
    AddAdminToCompany(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddAdminToCompany', model);
    }
    AddAdminToACompany(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddAdminToAirCompany', model);
    }
    SaveDiscount(model){
        return this.http.post(this.BaseURI + '/AppUser/SaveDiscount', model);
    }
    GetProfitForAdmin(model){
        return this.http.post(this.BaseURI + '/AppUser/GetProfit', model);
    }
    socialLogIn(model){
        return this.http.post(this.BaseURI + '/AppUser/SocialLogIn', model);
    }
    socialLogInFacebook(model){
        return this.http.post(this.BaseURI + '/AppUser/SocialLogInFb', model);
    }
    
    getAdmins(model){
        var param = {
            companies: model
        }
        return this.http.post(this.BaseURI + '/AppUser/GetAdminsForCompanies', param);
    }

    getAirAdmins(model){
        var param = {
            companies: model
        }
        return this.http.post(this.BaseURI + '/AppUser/GetAdminsForAirCompanies', param);
    }

    getDiscount() {
        return this.http.get(this.BaseURI + '/AppUser/GetDiscount');
    }

    addPoints() {
        return this.http.post(this.BaseURI + '/AppUser/AddPointsToUser', '');
    }

    removePoints() {
        return this.http.post(this.BaseURI + '/AppUser/RemovePointsFromUser', '');
    }

    combinedReservation(model) {
        return this.http.post(this.BaseURI + '/AppUser/MakeCombinedReservation', model);
    }

    loadResStatus() {
        return this.http.get(this.BaseURI + '/AppUser/LoadReservationsStatus');
    }
    
}
