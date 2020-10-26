import { Component, OnInit, NgModule, ViewChild } from '@angular/core';
import { Routes, Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { UserService } from 'src/app/Services/userService';
import { User } from 'src/app/entities/user/user';
import { Flight } from 'src/app/entities/flight/fligh';
import { Car } from 'src/app/entities/car/car';
import { NgForm, FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { ChangeUserComponent } from '../changeUser/changeUser.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AirlineService } from 'src/app/Services/airlineService';
import { CarReservation } from 'src/app/entities/carReservation/carReservation';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { Sort } from '@angular/material/sort';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { UserModel, FriendRequestReceived, TicketInvitation, FriendRequestSent, Notification, SendRequestModel } from 'src/app/entities/user/userModel';
import { FlightListingInfo, TicketListingInfo } from 'src/app/entities/flight/flightListingInfo';
import { CarReservationModel } from 'src/app/entities/carReservation/carReservationModel';
import { PendingReservation } from 'src/app/entities/reservations/pendingReservation';


@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class UserComponent implements OnInit {
  public user = new UserModel(new Array<Ticket>(), '', '', '', '', '', '', '',
    new Array<Ticket>(), new Array<CarReservationModel>(), new Array<TicketListingInfo>(), new Array<UserModel>(),
    new Array<FriendRequestReceived>(), new Array<FriendRequestSent>(), new Array<Notification>(),
    0, 0, false, false);
  showFlights: boolean;
  showCars: boolean;
  showFriends: boolean;
  showPeople: boolean;
  showRequests: boolean;
  showProfile: boolean;
  showOldFlights: boolean;
  showFlgRequests: boolean;
  showOldCars: boolean;
  showReservations: boolean;
  ocenioL: Array<boolean>;
  ocenioA: Array<boolean>;
  displayedColumns = ['from', 'to', 'departureDate', 'duration', 'prise', 'discount', 'seat', 'trip', 'option'];
  displayedColumnsOF = ['aservice', 'from', 'to', 'departureDate', 'duration', 'prise', 'discount', 'seat', 'trip', 'rate'];


  displayedColumns2 = ['fullName', 'option'];
  displayedColumns3 = ['fullName', 'option'];
  displayedColumns4 = ['fullName', 'option'];
  mySubscription: any;
  successPass: boolean = false;
  successUn: boolean = false;
  dataSource: Array<TicketListingInfo>;
  dataSourceFlgReq: Array<TicketListingInfo>;
  dataSource2: Array<UserModel>;
  dataSource3: Array<UserModel>;
  dataSource4: Array<FriendRequestReceived>;
  mainDataSource2: Array<UserModel>;
  mainDataSource3: Array<UserModel>;
  dataSourceOldFlights: Array<TicketListingInfo>;
  expandedElement: FlightListingInfo | null;

  dataSourcePendingRes: Array<PendingReservation>;
  displayedColumnsPenRes = ['dateMade', 'description', 'status'];

  dataSourceOldCars: Array<CarReservation>;
  allCarsReservations: Array<CarReservation>;
  expandedElement2: Object | null;
  columnsToDisplay2 = ['dateFrom', 'dateTo', 'total'];

  hide = true;
  ChangePasswordGroup: FormGroup;
  ChangeUsernameGroup: FormGroup;
  ChangeInformationGroup: FormGroup;
  panelOpenState = false;
  wrongPassword: boolean;
  usernameExists: boolean;
  niz: Array<string>;
  niz2: Array<string>;
  registerForm = this.fb.group({
    Name: ['', Validators.required],
  });


  // tslint:disable-next-line: max-line-length
  constructor(private carService: CarRentalService, private airl: AirlineService, private modalService: NgbModal, private fb: FormBuilder, private route: Router, private router: ActivatedRoute, private logService: UserService) {
    this.showFlights = false;
    this.showCars = false;
    this.showFriends = false;
    this.showRequests = false;
    this.showProfile = false;
    this.showOldCars = false;
    this.showOldFlights = false;
    this.showFlgRequests = false;
    this.showReservations = false;
    this.route.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    this.mySubscription = this.route.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.route.navigated = false;
      }
    });
  }

  ngOnInit(): void {
    this.allCarsReservations = new Array<CarReservation>();
    this.dataSourceFlgReq = new Array<TicketListingInfo>();
    this.allCarsReservations = new Array<CarReservation>();
    this.logService.loadUser().subscribe(
      (res: any) => {
        this.user = res;
        this.carService.loadMyCars().subscribe(
          (res: any) => {
            this.dataSourceOldCars = res;
            this.user.rentedCars = res;

            this.allCarsReservations = res.rentedCars;
            this.dataSourceOldCars = this.allCarsReservations.slice();


          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
        this.airl.LoadMyFlights().subscribe(
          (res: any) => {
            this.dataSource = res;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
        this.airl.LoadMyOldFlights().subscribe(
          (res: any) => {
            this.dataSourceOldFlights = res;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
        this.airl.Requests().subscribe(
          (res: any) => {
            this.dataSourceFlgReq = res;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
        this.logService.loadResStatus().subscribe(
          (res: any) => {
            this.dataSourcePendingRes = res;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
        this.dataSource2 = this.user.friends;
        this.mainDataSource2 = this.dataSource2;
        //this.dataSourceOldCars = this.user.oldRides;

        this.wrongPassword = false;
        this.usernameExists = false;


        this.dataSource4 = this.user.friendRequests;
        //this.dataSourceFlgReq = this.user.FlightRequests;
        this.ChangePasswordGroup = this.fb.group({
          'newPassword': new FormControl('', Validators.required),
          'repeatNewPassword': new FormControl('', Validators.required),
          'oldPassword': new FormControl('', Validators.required),
        }, { validator: this.checkIfMatchingPasswords('newPassword', 'repeatNewPassword') });
        this.ChangeUsernameGroup = this.fb.group({
          'username': new FormControl(this.user.username, Validators.required),
          'currentPass': new FormControl('', Validators.required),
        });
        this.ChangeInformationGroup = this.fb.group({
          'firstname': new FormControl(this.user.fullName, Validators.required),
          'address': new FormControl(this.user.address, Validators.required),
          'email': new FormControl(this.user.email, [Validators.required, Validators.email]),
          'passport': new FormControl(this.user.passport, Validators.required),
          'birthday': new FormControl('', Validators.required),
          'passCurrent': new FormControl('', Validators.required),
        });

        this.wrongPassword = false;
        this.usernameExists = false;

        this.dataSourceOldFlights = res.oldFlights;
        this.niz = new Array<string>();
        this.niz[0] = 'Economy';
        this.niz[1] = 'Business';
        this.niz[2] = 'First';
        this.niz2 = new Array<string>();
        this.niz2[0] = 'One way';
        this.niz2[1] = 'Round trip';
        //this.dataSourceOldFlights = this.dataSource.slice();
        this.ocenioA = new Array<boolean>();
        this.ocenioL = new Array<boolean>();
        // tslint:disable-next-line: prefer-for-of
        // for (let i = 0; i < this.user.oldFlights.length; i++)
        // {
        //     this.ocenioA[this.user.oldFlights[i].id] = false;
        //     this.ocenioL[this.user.oldFlights[i].id] = false;
        // }
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );



    this.logService.loadPeople().subscribe(
      (res: any) => {
        this.dataSource3 = res;
        this.mainDataSource3 = this.dataSource3;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  dozvoljeno(flight: FlightListingInfo) {
    var date = new Date();
    var dani = flight.departureDate.split(' ')[0].split('/')[0];
    var mjes = flight.departureDate.split(' ')[0].split('/')[1];
    var god = flight.departureDate.split(' ')[0].split('/')[2];
    var h = Number((flight.departureDate.split(' ')[1]).split(':')[0]);
    var m = Number((flight.departureDate.split(' ')[1]).split(':')[1]);

    date.setFullYear(Number(god), Number(mjes) - 1, Number(dani));

    date.setHours(h - 2);
    date.setMinutes(m);
    if (date > new Date()) {
      return false;
    }
    return true;
  }
  sortData(sort: Sort) {
    const data = this.allCarsReservations.slice();
    if (!sort.active || sort.direction === '') {
      this.dataSourceOldCars = data;
      return;
    }

    this.dataSourceOldCars = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'dateFrom': return compare(new Date(Date.parse(a.From)), new Date(Date.parse(b.From)), isAsc);
        case 'dateTo': return compare(new Date(Date.parse(b.From)), new Date(Date.parse(b.To)), isAsc);
        case 'total': return compare(a.Car, b.Car, isAsc);
        default: return 0;
      }
    });
  }
  sortData2(sort: Sort) {
    const data = this.allCarsReservations.slice();
    if (!sort.active || sort.direction === '') {
      this.dataSourceOldCars = data;
      return;
    }

    this.dataSourceOldCars = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'dateFrom': return compare(new Date(Date.parse(a.From)), new Date(Date.parse(b.From)), isAsc);
        case 'dateTo': return compare(new Date(Date.parse(b.From)), new Date(Date.parse(b.To)), isAsc);
        case 'total': return compare(a.Car, b.Car, isAsc);
        default: return 0;
      }
    });
  }

  cancelRequest(id: string) {
    this.dataSource4 = [];
    var model = new SendRequestModel(this.user.id, id);
    this.dataSource3 = [];
    this.logService.CancelRequest(model).subscribe(
      (res: any) => {
        this.user = res;
        this.dataSource2 = this.user.friends;
        this.mainDataSource2 = this.dataSource2;
        this.dataSource = res.flights;
        this.dataSourceOldFlights = res.oldFlights;
        //this.dataSourceOldCars = this.user.oldRides;
        this.logService.loadPeople().subscribe(
          (res: any) => {
            this.dataSource3 = res;
            this.mainDataSource3 = this.dataSource3;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );

        this.dataSourceOldCars = this.allCarsReservations.slice();
        this.logService.loadPeople().subscribe(
          (res: any) => {
            this.dataSource3 = res;
            this.mainDataSource3 = this.dataSource3;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );


        this.dataSource4 = this.user.friendRequests;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  acceptRequest(id: string) {
    this.dataSource4 = [];
    var model = new SendRequestModel(this.user.id, id);
    this.dataSource3 = [];
    this.logService.acceptRequest(model).subscribe(
      (res: any) => {
        this.user = res;
        this.dataSource2 = this.user.friends;
        this.mainDataSource2 = this.dataSource2;

        //this.dataSourceOldCars = this.user.oldRides;
        this.logService.loadPeople().subscribe(
          (res: any) => {
            this.dataSource3 = res;
            this.mainDataSource3 = this.dataSource3;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );

        this.dataSourceOldCars = this.allCarsReservations.slice();
        this.logService.loadPeople().subscribe(
          (res: any) => {
            this.dataSource3 = res;
            this.mainDataSource3 = this.dataSource3;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );


        this.dataSource4 = this.user.friendRequests;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  rate(id: number, flight: FlightListingInfo) {
    if (!this.ocenioL[flight.id]) {
      var model =
      {
        id: id,
        flight: flight.id
      }
      this.airl.RateFlight(model).subscribe(
        (res: any) => {

        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        }
      );

      this.ocenioL[flight.id] = true;
    }
    else {
      alert('You already gave a rate!');
    }

  }
  rate2(id: number, idC: number, flight: FlightListingInfo) {
    if (!this.ocenioA[flight.id]) {
      var model =
      {
        id: id,
        flightiD: flight.id,
        compId: idC
      }
      this.airl.RateCompany(model).subscribe(
        (res: any) => {
          this.ocenioA[flight.id] = true;
        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        }
      );
    }
    else {
      alert('You have already gave a rate!');
    }
  }
  cancel(el: TicketListingInfo): void {

    this.dataSource = [];
    this.airl.CancelFlight(el).subscribe(
      (res: any) => {
        this.dataSource = res;
        this.carService.loadMyCars().subscribe(
          (res: any) => {
            this.dataSourceOldCars = res;
            this.user.rentedCars = res;

            this.allCarsReservations = res.rentedCars;
            this.dataSourceOldCars = this.allCarsReservations.slice();


          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );

  }
  myFlightsOld() {
    this.closeAll();
    this.showOldFlights = !this.showOldFlights;
  }
  sendRequest(id: string) {
    var model = new SendRequestModel(this.user.id, id);
    this.dataSource3 = [];
    this.logService.sendRequest(model).subscribe(
      (res: any) => {
        this.dataSource3 = res;
        this.mainDataSource3 = this.dataSource3;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  oldCars() {
    this.closeAll();
    this.showOldCars = !this.showOldCars;
  }
  pendingReservations() {
    this.closeAll();
    this.showReservations = !this.showReservations;
  }
  closeAll() {
    this.showRequests = false;
    this.showFlights = false;
    this.showCars = false;
    this.showFriends = false;
    this.showPeople = false;
    this.showProfile = false;
    this.showOldFlights = false;
    this.showOldCars = false;
    this.showFlgRequests = false;
    this.showReservations = false;
  }
  requests() {
    this.closeAll();
    this.showRequests = !this.showRequests;
  }
  flightRequests() {
    this.closeAll();
    this.showFlgRequests = !this.showFlgRequests;
  }
  myFlights() {
    this.closeAll()
    this.showFlights = !this.showFlights;
  }
  myCars() {
    this.closeAll();
    this.showCars = !this.showCars;
  }
  removeFriend(id: string) {
    this.dataSource2 = [];
    var model = new SendRequestModel(this.user.id, id);
    this.dataSource3 = [];
    this.logService.RemoveFriend(model).subscribe(
      (res: any) => {
        this.user = res;
        this.dataSource2 = this.user.friends;
        this.mainDataSource2 = this.dataSource2;
        this.dataSource = res.flights;
        this.dataSourceOldFlights = res.oldFlights;
        //this.dataSourceOldCars = this.user.oldRides;
        this.logService.loadPeople().subscribe(
          (res: any) => {
            this.dataSource3 = res;
            this.mainDataSource3 = this.dataSource3;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );

        this.dataSourceOldCars = this.allCarsReservations.slice();
        this.logService.loadPeople().subscribe(
          (res: any) => {
            this.dataSource3 = res;
            this.mainDataSource3 = this.dataSource3;
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );


        this.dataSource4 = this.user.friendRequests;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  people() {
    this.closeAll();
    this.showPeople = !this.showPeople;

  }
  change() {
    const modalRef = this.modalService.open(ChangeUserComponent);
    modalRef.componentInstance.user = this.user;  //parent to change user
  }
  friends() {
    this.closeAll();
    this.showFriends = !this.showFriends;
  }
  editProfile() {
    this.closeAll();
    this.showProfile = !this.showProfile;
  }
  search() {
    var Name = this.registerForm.value.Name;

    this.dataSource2 = [];
    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < this.mainDataSource2.length; i++) {
      // tslint:disable-next-line: triple-equals

      // tslint:disable-next-line: max-line-length
      if (Name == '' || this.mainDataSource2[i].fullName.toLowerCase().includes(Name.toLowerCase())) {
        this.dataSource2.push(this.mainDataSource2[i]);
      }
    }
  }
  search2() {
    var Name = this.registerForm.value.Name;

    this.dataSource3 = [];
    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < this.mainDataSource3.length; i++) {
      // tslint:disable-next-line: triple-equals

      // tslint:disable-next-line: max-line-length
      if (Name == '' || this.mainDataSource3[i].fullName.toLowerCase().includes(Name.toLowerCase())) {
        this.dataSource3.push(this.mainDataSource3[i]);
      }
    }
  }

  checkIfMatchingPasswords(passwordKey: string, passwordConfirmationKey: string) {
    return (group: FormGroup) => {
      let passwordInput = group.controls[passwordKey],
        passwordConfirmationInput = group.controls[passwordConfirmationKey];
      if (passwordInput.value !== passwordConfirmationInput.value) {
        return passwordConfirmationInput.setErrors({ notEquivalent: true })
      }
      else {
        return passwordConfirmationInput.setErrors(null);
      }
    }
  }
  acceptreq(el: TicketListingInfo) {
    this.airl.AcceptFlightRequest(el).subscribe(
      (res: any) => {
        this.dataSourceFlgReq = res;

        this.logService.SetPoints().subscribe(
          (res: any) => {

          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          }
        );
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  cancelreq(el: TicketListingInfo) {
    this.airl.CancelFlightRequest(el).subscribe(
      (res: any) => {
        this.dataSourceFlgReq = res;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }



  // rateCompany(star: number, reservation: CarReservation) {
  //   this.carService.rateCompany(reservation.companyID, star);
  //   for(let i = 0; i<this.dataSourceOldCars.length; i++) {
  //     if(this.dataSourceOldCars[i] == reservation) {
  //       this.dataSourceOldCars[i].ratedCompany = star;
  //     }
  //   }
  // }

  // rateCar(star: number, reservation: CarReservation) {
  //   this.carService.rateCar(reservation.companyID, reservation.carEn, star);
  //   for(let i = 0; i<this.dataSourceOldCars.length; i++) {
  //     if(this.dataSourceOldCars[i] == reservation) {
  //       this.dataSourceOldCars[i].ratedCar = star;
  //     }
  //   }
  // }

  checkIfReservationFinished(toDate: string) {
    var date = new Date(Date.parse(toDate));
    var now = new Date(Date.now());
    if (now < date) {
      return false;
    }
    else {
      return true;
    }
  }

  checkUsername() {
    var model = {
      username: this.ChangeUsernameGroup.value.username
    }
    this.logService.CheckUsername(model).subscribe(
      (res: any) => {

        if (res == true) {
          this.usernameExists = false;
          return true;
        }
        this.usernameExists = true;
        return false;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
        return false;
      });

  }

  ChangePassword() {
    this.CheckPassword(this.ChangePasswordGroup.value.oldPassword);
    if (this.successPass) {
      if (this.ChangePasswordGroup.valid) {
        var model =
        {
          password: this.ChangePasswordGroup.value.oldPassword,
          newPassword: this.ChangePasswordGroup.value.newPassword
        }
        this.logService.ChangePassword(model).subscribe(
          (res: any) => {
            this.user = res;
            this.ChangePasswordGroup = this.fb.group({
              'newPassword': new FormControl('', Validators.required),
              'repeatNewPassword': new FormControl('', Validators.required),
              'oldPassword': new FormControl('', Validators.required),
            });
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          });
        document.getElementById('changePassError').style.color = 'green';
        document.getElementById('changePassError').textContent = 'Success!';
      }
      else {
        document.getElementById('changePassError').style.color = 'red';
        document.getElementById('changePassError').textContent = 'Passwords do not match';
      }
    }
    else {
      document.getElementById('changePassError').style.color = 'red';
      document.getElementById('changePassError').textContent = 'Current password is wrong';
    }
  }
  CheckPassword(username) {

    var model = {
      username: username
    }

    this.logService.CheckPassword(model).subscribe(
      (res: any) => {
        if (res.ret == true) {
          this.successPass = true;
        }
        else {
          this.successPass = false;
        }
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
        this.successPass = false;
      });

  }
  changeUsername() {
    this.CheckPassword(this.ChangeUsernameGroup.value.currentPass);
    this.checkUsername();
    if (this.successPass) {
      if (!this.usernameExists && this.ChangeUsernameGroup.valid) {
        var model = {
          username: this.ChangeUsernameGroup.value.username
        }
        this.logService.ChangeUserName(model).subscribe(
          (res: any) => {
            this.user = res;
            this.ChangeUsernameGroup = this.fb.group({
              'username': new FormControl('', Validators.required),
              'currentPass': new FormControl('', Validators.required),
            });
          },
          err => {
            if (err.status == 400)
              alert("error");
            else
              console.log(err);
          });
        document.getElementById('changeUsernameError').style.color = 'green';
        document.getElementById('changeUsernameError').textContent = 'Success!';
      }
      else {
        document.getElementById('changeUsernameError').style.color = 'red';
        document.getElementById('changeUsernameError').textContent = 'Username is taken';
      }
    }
    else {
      document.getElementById('changeUsernameError').style.color = 'red';
      document.getElementById('changeUsernameError').textContent = 'Wrong password';
    }

  }

  changeInformation() {
    this.CheckPassword(this.ChangeInformationGroup.value.passCurrent);

    if (!this.successPass) {
      document.getElementById('changeInfoError').style.color = 'red';
      document.getElementById('changeInfoError').textContent = 'Wrong password';
      return;
    }
    if (this.ChangeInformationGroup.valid) {
      var fn = this.ChangeInformationGroup.value.fullName;
      var addr = this.ChangeInformationGroup.value.address;
      var email = this.ChangeInformationGroup.value.email;

      var model =
      {
        fullName: fn,
        address: addr,
        email: email
      }

      this.logService.SaveNewAccountDetails(model).subscribe(
        (res: any) => {
          this.user = res;

        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        });

      document.getElementById('changeInfoError').style.color = 'green';
      document.getElementById('changeInfoError').textContent = 'Success!';
    }
    else {
      document.getElementById('changeInfoError').style.color = 'red';
      document.getElementById('changeInfoError').textContent = 'Please enter valid data in all fields';
    }
  }

  GiveUpCarRes(companyId: number, resId: number) {
    this.carService.GiveUpCarRes(companyId, resId).subscribe(
      (res: any) => {
        this.dataSourceOldCars = res;
        
      },
      err => {
        if (err.status == 400)
          alert("error 400");
        else
          console.log(err);
      }
    );
  }

  rateCompany(star: number, compId: number, resId: number, element: CarReservationModel) {
    this.carService.rateCarCompany(compId, star, resId).subscribe(
      (res: any) => {
        element.ratedCompany = star;
      },
      err => {
        if (err.status == 400)
          alert("error 400");
        else
          console.log(err);
      }
    );
    // this.carService.rateCompany(reservation.companyID, star);
    // for(let i = 0; i<this.dataSourceOldCars.length; i++) {
    //   if(this.dataSourceOldCars[i] == reservation) {
    //     this.dataSourceOldCars[i].ratedCompany = star;
    //   }
    // }
  }

  rateCar(star: number, compId: number, carId: number, resId: number, element: CarReservationModel) {
    this.carService.rateCar(compId, star, resId, carId).subscribe(
      (res: any) => {
        element.ratedCar = star;
      },
      err => {
        if (err.status == 400)
          alert("error 400");
        else
          console.log(err);
      }
    );
    // this.carService.rateCar(reservation.companyID, reservation.carEn, star);
    // for(let i = 0; i<this.dataSourceOldCars.length; i++) {
    //   if(this.dataSourceOldCars[i] == reservation) {
    //     this.dataSourceOldCars[i].ratedCar = star;
    //   }
    // }
  }



  checkIfCanGiveUp(fromDate: string, toDate: string) {
    if (!this.checkIfReservationFinished(toDate)) {
      var date = new Date(Date.parse(fromDate) - (3 * (1000 * 60 * 60 * 24)));
      var now = new Date(Date.now());
      if (now < date) {
        return true;
      }
      else {
        return false;
      }
    }
    else
      return false;
  }

}


function compare(a: number | string | Date, b: number | string | Date, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);


}