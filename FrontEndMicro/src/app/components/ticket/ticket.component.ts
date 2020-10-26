import { Component, OnInit, ViewChild, Input } from '@angular/core';
import {NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm, FormBuilder, Validators } from '@angular/forms';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { Flight } from 'src/app/entities/flight/fligh';
import { Ticket } from 'src/app/entities/ticket/ticket';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { TicketInvitation } from 'src/app/entities/user/userModel';
import { TicketListingInfo, IdModel } from 'src/app/entities/flight/flightListingInfo';
import { AirlineListingInfo } from 'src/app/entities/airline/airlineListingInfo';

@Component({
    selector: 'app-tic',
    templateUrl: './ticket.component.html',
    styleUrls: ['./ticket.component.css']
})

export class TicketComponent implements OnInit {

    @Input() idA: number;
    displayedColumns = ['from', 'to', 'departureDate', 'duration', 'price', 'discount', 'seat', 'option'];
    dataSource = new Array<TicketListingInfo>();
    niz: Array<string>;
    idModel: IdModel;
    company: AirlineListingInfo;
    // tslint:disable-next-line: max-line-length
    constructor(public route: Router, public router: ActivatedRoute, private fb: FormBuilder, private airlineService: AirlineService, private userService: UserService ) { }

    ngOnInit(): void {
        this.router.params.subscribe(params => { this.idA = params['idA']; });

        this.idModel = new IdModel(parseInt(this.idA.toString()), 0);
        this.airlineService.loadAirline(this.idModel).subscribe(
            (res: any) => {

                 this.company = res;
                 this.dataSource = this.company.fastTickets;

            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
        );
        this.niz = new Array<string>();
        this.niz[0] = 'Economy';
        this.niz[1] = 'Business';
        this.niz[2] = 'First';
    }
    reserve(t: TicketListingInfo)
    {
        this.userService.FastReserve(t).subscribe(
            (res: any) => {
                alert("Reservated!");
                var path = '/airlines/' + this.company.id + '/airline/';
                this.route.navigate([path]);

            },
            err => {
              if (err.status == 400)
                alert("error");
              else
                console.log(err);
            }
        );
      
        
    }

}
