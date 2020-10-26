import { Component, OnInit, Host } from '@angular/core';
import { FormBuilder, FormGroup, Validators}  from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Car } from 'src/app/entities/car/car';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { UserService } from 'src/app/Services/userService';
import { User } from 'src/app/entities/user/user';
import { DataService } from 'src/app/data.service';
import { LogInComponent } from 'src/app/components/logIn/logIn.component';
import { CarRentalComponent } from 'src/app/components/carRental/carRental.component';
import { CarModel } from 'src/app/entities/car/carModel';
import { ExtraAmenity } from 'src/app/entities/rentAcar/ExtraAmenity';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { CarReservation } from 'src/app/entities/carReservation/carReservation';
import { UserType } from 'src/app/entities/userType/userType';

interface Type {
    value: string;
    viewValue: string;
}

interface Amenity {
    id: number;
    image: string;
    name: string;
    oneTimePayment: boolean;
    price: number;
    selected: boolean;
}

@Component({
    selector: 'app-rent',
    templateUrl: './rent.component.html',
    styleUrls: ['./rent.component.css']
})



export class RentComponent implements OnInit {
    idComp: number;
    idCar: number;
    car: CarModel
    amenities: Array<Amenity>;
    showAmenities: Array<Array<Amenity>>;
    disabled = false;
    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    user: User;
    isLoggedIn: string;
    days: number;
    totalPrice: number;
    dates: Array<string>;
    dateFrom: string;
    dateTo: string;
    returnLocation: string;
    loc1:string;
    t1:string;
    loc2:string;
    t2: string;
    idModel = {
        idComp: 0,
        idCar: 0
    }
    silver: number;
    bronze: number;
    gold: number; 
    percent: number;

    constructor(private modalService: NgbModal, private dataService: DataService, private router:Router, private route: ActivatedRoute, private carRentalService: CarRentalService, private userService: UserService, private _formBuilder: FormBuilder) { 
        route.params.subscribe(
            params => 
            { 
                this.idComp = params['idComp']; 
                this.idCar = params['idCar']; 
            });
    }

    ngOnInit(): void {
        //this.dataService.sharedMessage.subscribe(message => this.isLoggedIn = message);
        var token = localStorage.getItem('token');
        if(token != null)
            this.isLoggedIn = "true";
        else 
            this.isLoggedIn = "false";

        this.user = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13KD1', birthday: new Date(2020, 4, 1, 22, 0, 0, 0), friendRequests: new Array<User>(), id: 2, firstName: '',
            lastName: '', email: '',
            address: '', password: '', username: '',
            type: UserType.Registered, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points: 0};
        this.car = new CarModel(0, "", "", 0, 15, "", 0, "", 0, "");
        this.idModel.idCar = this.idCar;
        this.idModel.idComp = this.idComp;
        this.carRentalService.getCar(this.idModel).subscribe(
            (res: any) => {
                this.car = res;
            },
            err => {
              if (err.status == 400)
                alert("error 400");
              else
                console.log(err);
            }
        );
        this.showAmenities = new Array<Array<Amenity>>();
        this.carRentalService.getCompanyAmenities(this.idComp).subscribe(
            (res: any) => {
                this.amenities = res;
                var j = -1;
                for(let i = 0; i < this.amenities.length; i++) {
                    if(i%3 == 0) {
                        j = j + 1;
                        this.showAmenities[j] = new Array<Amenity>();
                    }
                    this.showAmenities[j].push(this.amenities[i]);
                }
            },
            err => {
              if (err.status == 400)
                alert("error 400");
              else
                console.log(err);
            }
        );
        this.userService.getDiscount().subscribe(
            (res: any) => {
                this.silver = parseInt(res.silverTier);
                this.bronze = parseInt(res.bronzeTier);
                this.gold = parseInt(res.goldTier);
                this.percent = parseInt(res.discountPercent);
            },
            err => {
                console.log(err.error.message);
            }
        );
        this.user = this.userService.loggedUser;
        this.firstFormGroup = this._formBuilder.group({
            addr1: ['', Validators.required],
            timeH1: ['', Validators.required],
            timeM1: ['', Validators.required],
            time1: ['', Validators.required],
        });
        this.secondFormGroup = this._formBuilder.group({
            addr2: ['', Validators.required],
            timeH2: ['', Validators.required],
            timeM2: ['', Validators.required],
            time2: ['', Validators.required],
        });
        this.dateFrom = localStorage.getItem('dateFrom');
        this.dateTo = localStorage.getItem('dateTo');
        this.returnLocation = localStorage.getItem('retLoc');
    }

    Submit() :void{
        if(this.firstFormGroup.valid && this.secondFormGroup.valid) {
            var addr1 = this.firstFormGroup.value.addr1;
            var timeH1 = this.firstFormGroup.value.timeH1;
            var timeM1 = this.firstFormGroup.value.timeM1;
            var time1 = this.firstFormGroup.value.time1;
            this.loc1 = this.car.location + ', ' + addr1;
            this.t1 = timeH1 + ':' + timeM1 + ':' + time1; 

            var addr2 = this.secondFormGroup.value.addr2;
            var timeH2 = this.secondFormGroup.value.timeH2;
            var timeM2 = this.secondFormGroup.value.timeM2;
            var time2 = this.secondFormGroup.value.time2;
            this.loc2 = this.returnLocation + ', ' + addr2;
            this.t2 = timeH2 + ':' + timeM2 + ':' + time2; 

            var selected = new Array<number>();

            for(let i = 0; i<this.amenities.length; i++) {
                if (this.amenities[i].selected) {
                    selected.push(this.amenities[i].id);
                }
            }

            var reservation = {
                company: this.idComp,
                car: this.idCar,
                from: this.dateFrom,
                to: this.dateTo,
                pickUpAddr: this.loc1,
                dropOffAddr: this.loc2,
                fromTime: this.t1,
                toTime: this.t2,
                extras: selected,
                price: this.car.price,
                gold: this.gold,
                silver: this.silver,
                bronze: this.bronze,
                percent: this.percent,
                quickRes: 0
            }

            this.carRentalService.rentCar(reservation).subscribe(
                (res: any) => {
                    this.userService.addPoints().subscribe(
                        (res: any) => {
                            var path = "/carCompanies/"+ this.idComp + "/carCompany";
                            this.router.navigate([path]);
                        },
                        err => {
                            document.getElementById('rentError').style.color = 'red';
                            document.getElementById('rentError').textContent = err.error.message;
                            document.getElementById('rentError').style.display = 'block';
                        }
                    );
                },
                err => {
                    document.getElementById('rentError').style.color = 'red';
                    document.getElementById('rentError').textContent = err.error.message;
                    document.getElementById('rentError').style.display = 'block';
                }
            );

        }
        
    }

    toggleChanged(id) {
        var idNum = id;
    }

    BackClick() {
        var path = "/carCompanies/"+ this.idComp + "/carCompany";
        this.router.navigate([path]);
    }

    SignIn() {
        const modalRef = this.modalService.open(LogInComponent);
          modalRef.componentInstance.loggedIn = this.isLoggedIn;
          modalRef.result.then((result) => {
            if (result) { 
              this.isLoggedIn = "true"; 
              this.SendMessage("true");
            }
        });
    }
    
    SignUp() {
    
    }

    SendMessage(message: string) {
        this.dataService.nextMessage(message);
    }
}
