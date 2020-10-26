import { Component, OnInit, Input } from '@angular/core';
import { Flight } from 'src/app/entities/flight/fligh';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { Seat } from 'src/app/entities/Seat/seat';
import { LogInComponent } from '../logIn/logIn.component';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Traveller } from 'src/app/entities/user/traveller';
import { DataService } from 'src/app/data.service';
import { SignUpComponent } from '../signUp/signUp.component';
import { Car } from 'src/app/entities/car/car';
import { trigger, state, transition, style, animate } from '@angular/animations';
import { CarRentalService } from 'src/app/Services/carRentalService';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { IdModel, FlightListingInfo, RatersInfo, AddTravellerModel, FinishModel, TicketListingInfo } from 'src/app/entities/flight/flightListingInfo';
import { Destination } from 'src/app/entities/destination/destination';
import { Luggage } from 'src/app/entities/flight/luggage';
import { Trip } from 'src/app/entities/flight/trip';
import { UserModel, SearchUserModel } from 'src/app/entities/user/userModel';


@Component({
  selector: 'app-flight',
  templateUrl: './flight.component.html',
  styleUrls: ['./flight.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class FlightComponent implements OnInit {
  registerForm2 = this.fb.group({
    Name: ['', Validators.required],
  });
  public brkolona: number;
  public brredova: number;
  public brojkolona: Array<number>;
  public duzinareda: Array<number>;
  isAdmin: boolean;
  public brojredovaukoloni: Array<number>;
  public selectedSeats = new Array<Seat>();
  public seats = new Array<Seat>();
  numOfSelected: number;
  trenutniBroj: number;
  isLoggedIn: string;
  dataSource: Array<Car>;
  expandedElement: Car | null;
  columnsToDisplay = ['brand', 'model', 'year', 'price'];
  f: FlightListingInfo;
  public flight = new FlightListingInfo(new Array<TicketListingInfo>(), new Array<RatersInfo>(), new Array<Destination>(), new Destination(1, "", "", ""), new Destination(1, "", "", ""), new Array<Seat>(), 0, 0, 0, new Luggage(0, "", 0), "", 0, "", 9, "", "", this.f, 0);
  idF: number;
  @Input() idA: number;
  user: UserModel;
  seat: Seat;
  spom: Seat;
  dataSource2 = new Array<UserModel>();
  mainDataSource2 = new Array<UserModel>();
  prikazan: boolean;
  finished: boolean;
  gotovo: boolean;
  indxSjednista: number;
  clicked: boolean;
  idModel: IdModel;
  tableHidden: boolean;
  tableHidden2: boolean;
  invited: boolean;
  silver: number;
  bronze: number;
  gold: number;
  percent: number;
  passengersData: number;
  days: number;
  dates = new Array<string>();
  NewGroup: FormGroup;
  public seatsSend = new Array<Seat>();
  registerForm = this.fb.group({
    fullName: ['', Validators.required],
    Contact: ['', [Validators.required, Validators.email]],
    Passport: ['', Validators.required],
  });
  registerForm3 = this.fb.group({
    Date: ['', Validators.pattern],
  });
  displayedColumns2 = ['fullName', 'option'];

  tickets: Array<FinishModel>;

  seatspom = new Array<Seat>();
  // tslint:disable-next-line: max-line-length
  constructor(private carService: CarRentalService, private modalService: NgbModal, private dataService: DataService, private fb: FormBuilder, private route: Router, private router: ActivatedRoute, private airlineService: AirlineService, private userService: UserService) {
    this.tickets = new Array<FinishModel>();
    this.trenutniBroj = 0;
    this.tableHidden = true;
    this.tableHidden2 = true;
    this.brojkolona = new Array<number>();
    this.duzinareda = new Array<number>();
    this.brredova = 2;
    this.gotovo = false;
    this.brkolona = 2;
    this.prikazan = false;
    this.finished = false;
    this.invited = false;
    this.clicked = false;
    this.passengersData = 1;
    this.indxSjednista = 0;
    this.selectedSeats = new Array<Seat>();
    this.brojredovaukoloni = new Array<number>();
    this.days = 0;
    this.dates = this.userService.getUserDates();

  }

  RentACar(element: Car) {
    var date = this.registerForm3.value.Date;

    this.userService.getDiscount().subscribe(
      (res: any) => {
        this.silver = parseInt(res.silverTier);
        this.bronze = parseInt(res.bronzeTier);
        this.gold = parseInt(res.goldTier);
        this.percent = parseInt(res.discountPercent);

        var carReservation = {
          company: 1,
          car: element.id,
          from: this.flight.departureDate,
          to: date,
          pickUpAddr: '',
          dropOffAddr: '',
          fromTime: '',
          toTime: '',
          extras: new Array<number>(),
          quickRes: 1,
          price: 0,
          gold: this.gold,
          silver: this.silver,
          bronze: this.bronze,
          percent: this.percent
        }

        this.seatsSend.forEach(element => {
          this.tickets.push(new FinishModel(element, this.silver, this.bronze, this.gold, this.percent));
        });

        var combinedReservation = {
          car: carReservation,
          tickets: this.tickets
        }

        this.userService.combinedReservation(combinedReservation).subscribe(
          (res: any) => {
            document.getElementById("carContent").style.display = 'none';
            document.getElementById("finishAllContent").style.display = 'block';
            this.tableHidden = true;
            this.tableHidden2 = true;
          },
          err => {
            console.log(err.console.error.message);
          }
        );

        // this.carService.rentCar(carReservation).subscribe(
        //   (res: any) => {
        //     this.userService.addPoints().subscribe(
        //       (res: any) => {
        //         document.getElementById("carContent").style.display = 'none';
        //         document.getElementById("finishAllContent").style.display = 'block';
        //         this.tableHidden = true;
        //         this.tableHidden2 = true;
        //       },
        //       err => {
        //         console.log(err.console.error.message);
        //       }
        //     );
        //   },
        //   err => {
        //     console.log(err.console.error.message);
        //   }
        // );

      },
      err => {
        console.log(err.error.message);
      }
    );




  }

  searchcar() {
    var date = this.registerForm3.value.Date;
    var returnPlace = this.flight.to.name;

    this.carService.getDiscountedCarsForFlight(this.flight.to.name, this.flight.departureDate, date, returnPlace).subscribe(
      (res: any) => {
        this.dataSource = res;
        this.tableHidden = false;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );



  }
  newTicket() {
    var seat = parseInt((document.getElementById("idseat") as HTMLInputElement).value);
    var discount = parseInt((document.getElementById("iddis") as HTMLInputElement).value);

    var usao = false;
    for (let i = 0; i < this.flight.seats.length; i++) {
      if (this.flight.seats[i].id == seat) {
        usao = true;
      }
    }
    if (!usao) {
      alert("Seat ID doesn't exists!");
      return;
    }
    var model =
    {
      seatId: seat,
      discount: discount,
      flightId: this.flight.id
    }


    this.airlineService.NewFastTicket(model).subscribe(
      (res: any) => {
        ((document.getElementById("idseat") as HTMLInputElement).value) = '';
        ((document.getElementById("iddis") as HTMLInputElement).value) = '';
        alert('success');
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  car() {
    this.tableHidden2 = false;
    this.gotovo = false;
    document.getElementById("finishContent").style.display = 'none';
    document.getElementById("carContent").style.display = 'block';
  }
  klik(): void {

    this.clicked = true;
    this.userService.loadUser().subscribe(
      (res: any) => {
        this.user = res;
        this.dataSource2 = this.user.friends;
        this.mainDataSource2 = this.dataSource2;
        this.registerForm.setValue({
          fullName: this.user.fullName,
          Contact: this.user.email,
          Passport: this.user.passport
        });

        this.numOfSelected = this.seatspom.length;
        if (this.numOfSelected > 0) {
          if (this.passengersData == this.numOfSelected) {
            this.finished = true;
          }
          this.prikazan = true;
        }
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );

  }
  search() {
    var Name = this.registerForm2.value.Name;

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
  inviteFriend(id: string) {
    var idModel = new SearchUserModel(id);
    this.userService.loadUserById(idModel).subscribe(
      (res: any) => {
        var user = res;
        this.invited = false;

        this.registerForm.setValue({
          fullName: user.fullName,
          Contact: user.email,
          Passport: user.passport
        });
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );


  }

  klik2(id: number): void {
    if (this.isAdmin) {
      var model =
      {
        seatId: id
      }
      this.airlineService.DisableSeat(model).subscribe(
        (res: any) => {

        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        }
      );
      this.airlineService.loadFlightById(this.idModel).subscribe(
        (res: any) => {

          this.flight = res;
          this.seats = this.flight.seats;
        },
        err => {
          if (err.status == 400)
            alert("error");
          else
            console.log(err);
        }
      );
    }
    else if (this.isLoggedIn == "true") {
      if (!this.prikazan) {
        this.clicked = false;
        for (let i = 0; i < this.flight.seats.length; i++) {
          if (this.flight.seats[i].id == id) {
            this.seat = this.flight.seats[i];
          }
        }
        //this.seat = this.airlineService.findSeat(id, this.idA, this.flight.id);
        this.seat.isSelected = !this.seat.isSelected;

        if (!this.seat.isSelected) {
          const index = this.seatspom.indexOf(this.seat);
          this.seatspom.splice(index, 1);
        }
        else {
          this.seatspom.push(this.seat);
        }
      }
      else {
        alert('You need to select at least one seat!');
      }
    }
    else {
      this.clicked = true;
    }
  }
  mydata() {
    this.userService.loadUser().subscribe(
      (res: any) => {
        this.user = res;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
    this.registerForm.setValue({
      fullName: this.user.fullName,
      Contact: this.user.email,
      Passport: this.user.passport
    });
  }
  invite() {
    if (this.user.friends.length == 0) {
      alert('You do not have any friends!');
    }
    else {
      this.invited = true;
    }
  }
  ngOnInit(): void {

    // tslint:disable-next-line: no-string-literal
    this.router.params.subscribe(params => { this.idF = params['idF']; this.idA = params['idA']; });
    this.idModel = new IdModel(parseInt(this.idF.toString()), 0);
    this.airlineService.loadFlightById(this.idModel).subscribe(
      (res: any) => {
        this.flight = res;
        this.seats = this.flight.seats;

        for (let index = 0; index < this.flight.seats.length / 4; index++) {

          this.duzinareda.push(index);
        }

        for (let ind = 0; ind < this.brkolona; ind++) {
          this.brojredovaukoloni.push(ind);
        }

        for (let ind1 = 0; ind1 < 2; ind1++) {
          this.brojkolona.push(ind1);
        }

        this.dataService.sharedMessage.subscribe(message => this.isLoggedIn = message);

        if (this.isLoggedIn == 'true') {
          var model =
          {
            id: this.flight.idCompany
          }
          this.airlineService.GetCompanyId(model).subscribe(
            (res: any) => {


              var model2 =
              {
                id: res.id
              }
              this.userService.IsAdmin(model2).subscribe(
                (res: any) => {

                  this.isAdmin = res;

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

      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );


  }

  isRegistered() {
    return this.userService.isRegistered();
  }


  finish() {
    var psp = this.registerForm.value.Passport;
    var sm = new SearchUserModel(psp);
    this.seat = this.seatspom[this.seatspom.length - 1];
    this.userService.findUserByPassport(sm).subscribe(
      (res: any) => {
        var user = res;

        this.indxSjednista++;
        if (user == null) {
          var fr = this.registerForm.value.fullName.split(' ', 2)[0];
          var ln = this.registerForm.value.fullName.split(' ', 2)[1];

          var em = this.registerForm.value.Contact;

          var ps = this.registerForm.value.Passport;

          var trav = new Traveller('', ps, '', fr, ln, em);
          var model = new AddTravellerModel(this.seat.id, trav);
          this.airlineService.addTraveller(model).subscribe(
            (res: any) => {
              this.seatsSend.push(res);
              //this.pomocna();
              document.getElementById("flightContent").style.display = 'none';
              document.getElementById("finishContent").style.display = 'block';
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
          var fr = user.fullName.split(' ', 2)[0];
          var ln = user.fullName.split(' ', 2)[1];

          var em = user.email;

          var ps = user.passport;

          var trav = new Traveller(user.id, ps, '', fr, ln, em);

          var model = new AddTravellerModel(this.seat.id, trav);

          this.airlineService.addTraveller(model).subscribe(
            (res: any) => {
              this.seatsSend.push(res);
              //this.pomocna();
              document.getElementById("flightContent").style.display = 'none';
              document.getElementById("finishContent").style.display = 'block';
            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
          );
        }


      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );


  }

  pomocna() {

    this.passengersData = 1;
    this.seatspom = [];
    this.finished = false;
    this.prikazan = false;
    this.indxSjednista = 0;

    this.gotovo = true;
    this.seatsSend.forEach(element => {
      var finishModel = new FinishModel(element, this.gold, this.silver, this.bronze, this.percent);
      this.userService.getDiscount().subscribe(
        (res: any) => {
          this.silver = parseInt(res.silverTier);
          this.bronze = parseInt(res.bronzeTier);
          this.gold = parseInt(res.goldTier);
          this.percent = parseInt(res.discountPercent);

          this.airlineService.finish(finishModel).subscribe(
            (res: any) => {
              this.userService.SetPoints().subscribe(
                (res: any) => {
                  document.getElementById("finishContent").style.display = 'none';
                  document.getElementById("finishAllContent").style.display = 'block';
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
        });
    },
      err => {
        document.getElementById('rentError').style.color = 'red';
        document.getElementById('rentError').textContent = err.error.message;
        document.getElementById('rentError').style.display = 'block';
      }
    );


    this.airlineService.loadFlightById(this.idModel).subscribe(
      (res: any) => {

        this.flight = res;
        this.seats = this.flight.seats;
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );
  }
  onSubmit(): void {
    var psp = this.registerForm.value.Passport;
    var sm = new SearchUserModel(psp);
    this.userService.findUserByPassport(sm).subscribe(
      (res: any) => {
        var user = res;

        // tslint:disable-next-line: triple-equals
        for (let i = 0; i < this.flight.seats.length; i++) {
          if (this.flight.seats[i].id == this.seatspom[this.indxSjednista].id) {
            this.seat = this.flight.seats[i];
          }
        }
        this.indxSjednista++;
        if (user == null) {
          var fr = this.registerForm.value.fullName.split(' ', 2)[0];
          var ln = this.registerForm.value.fullName.split(' ', 2)[1];

          var em = this.registerForm.value.Contact;

          var ps = this.registerForm.value.Passport;

          var trav = new Traveller('', ps, '', fr, ln, em);

          var model = new AddTravellerModel(this.seat.id, trav);
          this.airlineService.addTraveller(model).subscribe(
            (res: any) => {
              this.seatsSend.push(res);

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
          var fr = user.fullName.split(' ', 2)[0];
          var ln = user.fullName.split(' ', 2)[1];

          var em = user.Contact;

          var ps = user.Passport;

          var trav = new Traveller(user.id, ps, '', fr, ln, em);
          var model = new AddTravellerModel(this.seat.id, trav);
          this.airlineService.addTraveller(model).subscribe(
            (res: any) => {
              this.seatsSend.push(res);

            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
          );
        }

        this.registerForm.setValue({
          fullName: this.user.fullName,
          Contact: this.user.email,
          Passport: this.user.passport

        });
        this.passengersData++;
        if (this.passengersData == this.numOfSelected) {
          this.finished = true;
        }
      },
      err => {
        if (err.status == 400)
          alert("error");
        else
          console.log(err);
      }
    );



  }
  reset() {
    this.registerForm.reset();
  }
  cancel() {
    for (let i = 0; i < this.seatspom.length; i++) {
      // tslint:disable-next-line: triple-equals
      for (let j = 0; j < this.flight.seats.length; j++) {
        if (this.flight.seats[j].id == this.seatspom[i].id) {
          this.flight.seats[i].isSelected = false;
          this.flight.seats[i].taken = false;
          this.seatspom[i].isSelected = false;
          this.seatspom[i].taken = false;
        }
      }
    }
    this.passengersData = 1;
    this.seatspom = [];
    this.finished = false;
    this.prikazan = false;
    this.indxSjednista = 0;
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
    const modalRef = this.modalService.open(SignUpComponent);
  }

  SendMessage(message: string) {
    this.dataService.nextMessage(message);
  }
}



