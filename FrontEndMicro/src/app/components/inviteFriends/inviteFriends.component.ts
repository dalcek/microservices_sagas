import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Flight } from 'src/app/entities/flight/fligh';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/app/Services/airlineService';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Validators, NgForm, FormBuilder } from '@angular/forms';


@Component({
  selector: 'app-inviteFriends',
  templateUrl: './inviteFriends.component.html',
  styleUrls: ['./inviteFriends.component.css']
})
export class FriendsComponent implements OnInit {
    @Input() public flight;
    @ViewChild('f') courseForm: NgForm;
    prikazan: boolean;
    finished: boolean;
    passengersData: number;
    registerForm = this.fb.group({
        FirstName : ['', Validators.required],
        LastName : ['', [Validators.required]],
        Contact : ['', Validators.required],
      });

  // tslint:disable-next-line: max-line-length
  constructor(public activeModal: NgbActiveModal, private modalService: NgbModal, private fb: FormBuilder )
    {
        this.prikazan = false;
        this.finished = false;
        this.passengersData = 1;
    }

  ngOnInit(): void {
    // tslint:disable-next-line: no-string-literal
    alert(this.flight.id);
  }
  klik(): void
  {
      this.prikazan = true;
  }
  onSubmit(): void
  {
      this.registerForm.reset();
      this.passengersData++;
      // tslint:disable-next-line: triple-equals
      if (this.passengersData == this.flight.numOfPassengers) {
          this.finished = true;
      }
  }
}



