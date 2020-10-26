import { Injectable } from '@angular/core';
import { Time } from '@angular/common';
import { Car } from 'src/app/entities/car/car';
import { UserService } from './userService';
import { CarReservation } from 'src/app/entities/carReservation/carReservation'
import { Address } from '../entities/address/address';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
   })

export class CarRentalService {
    readonly BaseURI = 'http://localhost:5004/api';

    constructor(private usrl: UserService, private http: HttpClient) {
        
    }


    getAllCompanies() {
        return this.http.get(this.BaseURI + '/RentACar/GetAllActivatedCompanies');
    }

    searchCompanies(searchModel) {
        return this.http.post(this.BaseURI + '/RentACar/SearchCompanies', searchModel);
    }

    getCompanyProfile(idModel) {
        return this.http.post(this.BaseURI + '/RentACar/GetCompanyProfile', idModel);
    }

    getCarsSearch(searchModel) {
        return this.http.post(this.BaseURI + '/RentACar/SearchCars', searchModel);
    }

    getCar(idModelCarComp) {
        var idModel = {
            idComp: parseInt(idModelCarComp.idComp),
            idCar: parseInt(idModelCarComp.idCar)
        }
        return this.http.post(this.BaseURI + '/RentACar/GetCarInfo', idModel);
    }

    getCarsSearchHome(searchModel) {
        return this.http.post(this.BaseURI + '/RentACar/SearchCarsHome', searchModel);
    }
    getCompanyAmenities(idModel) {
        return this.http.get(this.BaseURI + '/RentACar/GetCompanyAmenities', {params: {id: idModel}});
    }

    rentCar(res) {
        var reservation = {
            company: parseInt(res.company),
            car: parseInt(res.car),
            from: res.from,
            to: res.to,
            pickUpAddr: res.pickUpAddr,
            dropOffAddr: res.dropOffAddr,
            fromTime: res.fromTime,
            toTime: res.toTime,
            extras: res.extras,
            quickRes: parseInt(res.quickRes),
            gold: parseInt(res.gold),
            silver: parseInt(res.silver),
            bronze: parseInt(res.bronze),
            percent: parseInt(res.percent)
        }
        return this.http.post(this.BaseURI + '/RentACar/RentCar', reservation);
    }

    GiveUpCarRes(companyId: number, resId: number) {
        var model = {
            compId: companyId,
            resId: resId
        }
        return this.http.post(this.BaseURI + '/RentACar/GiveUpCarReservation', model);
    }

    rateCarCompany(companyId: number, star:number, resId: number) {
        var model = {
            compId: companyId,
            star: star,
            resId: resId
        }
        return this.http.post(this.BaseURI + '/RentACar/RateCarCompany', model);
    }

    rateCar(compId:number, star:number, resId: number, carId: number) {
        var model = {
            compId: compId,
            star: star,
            resId: resId,
            carId: carId
        }
        return this.http.post(this.BaseURI + '/RentACar/RateCar', model);
    }

    getCompanyInfo(companyId) {
        return this.http.get(this.BaseURI + '/RentACar/GetCompanyInfoAdmin', {params: {id: companyId}});
    }
    updateCarCompany(companyId:number, name:string, desc:string, img:string){
        var model = {
            compId: companyId,
            name: name,
            description: desc,
            logo: img
        }
        return this.http.post(this.BaseURI + '/RentACar/UpdateCompanyInfo', model);
    }
    addCarCompanyLocation(companyId:number, clickedAddress:string, latitude:number, longitude:number) {
        var model = {
            compId: companyId,
            address: clickedAddress,
            latitude: latitude,
            longitude: longitude
        }
        return this.http.post(this.BaseURI + '/RentACar/AddCarCompanyLocation', model);
    }
    editCarCompanyLocation(companyId:number, clickedAddress:string, latitude:number, longitude:number, id:number) {
        var model = {
            compId: companyId,
            address: clickedAddress,
            latitude: latitude,
            longitude: longitude,
            locId: id
        }
        return this.http.post(this.BaseURI + '/RentACar/EditCarCompanyLocation', model);
    }
    removeCarCompanyLocation(id:number, companyId: number, newLoc: string) {
        var model = {
            id: id,
            id2: companyId,
            newAddr: newLoc
        }
        return this.http.post(this.BaseURI + '/RentACar/RemoveCarCompanyLocation', model);
    }
    UpdateCar(model){
        return this.http.post(this.BaseURI + '/RentACar/UpdateCar', model);
    }
    addNewCar(model) {
        return this.http.post(this.BaseURI + '/RentACar/AddNewCar', model);
    }
    updateAmenity(model) {
        return this.http.post(this.BaseURI + '/RentACar/UpdateAmenity', model);
    }
    AddAmenity(model) {
        return this.http.post(this.BaseURI + '/RentACar/AddAmenity', model);
    }
    SaveNewDiscountRange(model){
        return this.http.post(this.BaseURI + '/RentACar/SaveNewDiscountRange', model);
    }
    RemoveDiscountRange(id: number) {
        var model = {
            id: id,
        }
        return this.http.post(this.BaseURI + '/RentACar/RemoveDiscountRange', model);
    }
    RemoveCar(id: number) {
        var model = {
            id: id,
        }
        return this.http.post(this.BaseURI + '/RentACar/RemoveCar', model);
    }
    removeAmenity(model) {
        return this.http.post(this.BaseURI + '/RentACar/RemoveAmenity', model);
    }
    getDiscountedCars(id:number){
        return this.http.get(this.BaseURI + '/RentACar/GetDiscountedCarsForCompany', {params: {id: id.toString()}});
    }
    getDiscountedCarsForFlight(landingPlace, fromDate, toDate, returnPlace){
        var model = {
            location: landingPlace,
            returnLocation: returnPlace,
            from:fromDate,
            to:toDate
        }
        return this.http.post(this.BaseURI + '/RentACar/GetDiscountedCars', model);
    }
    getCarCompanies(){
        return this.http.get(this.BaseURI + '/RentACar/LoadWebsiteAdminCompanies');
    }
    RegisterCarCompany(companyName, email, username, pass, clickedAddress, clickedLat, clickedLon) {
        var model = {
            companyName: companyName,
            address: clickedAddress,
            latitude: clickedLat,
            longitude: clickedLon,
            email:email,
            username:username,
            password:pass
        }
        return this.http.post(this.BaseURI + '/RentACar/RegisterCarCompany', model);

    }
    getAdminCompany(id) {
        return this.http.get(this.BaseURI + '/RentACar/LoadAdminCompany', {params: {id: id.toString()}});
    }
    AvailableCarsAdmin(model) {
        return this.http.post(this.BaseURI + '/RentACar/AvailableCarsAdmin', model);
    }
    loadMyCars() {
        return this.http.get(this.BaseURI + '/RentACar/LoadMyReservations');
    }
}

