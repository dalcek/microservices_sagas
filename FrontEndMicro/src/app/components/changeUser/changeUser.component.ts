import { Component, OnInit, ViewChild, Input } from '@angular/core';
import {NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm, FormBuilder, Validators } from '@angular/forms';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';


@Component({
    selector: 'app-change',
    templateUrl: './changeUser.component.html',
    styleUrls: ['./changeUser.component.css']
})

export class ChangeUserComponent implements OnInit {
    @Input() //child of user component
    user: User;
    @ViewChild('f') courseForm: NgForm;
    registerForm = this.fb.group({
        UserName : ['', Validators.required],
        Email : ['', [Validators.required, Validators.email]],
        Password : ['', Validators.required],
        ConfirmPassword : ['', Validators.required],
        Name : ['', Validators.pattern],
        Surname : ['', Validators.pattern],
        PhoneNumber : ['', Validators.pattern],
        DateOfBirth : ['', Validators.pattern],
        Address : ['']
      });

    constructor(public activeModal: NgbActiveModal, private fb: FormBuilder, private loginService: UserService ) { }
    ngOnInit(): void {
    }
    onSubmit(){}

}
