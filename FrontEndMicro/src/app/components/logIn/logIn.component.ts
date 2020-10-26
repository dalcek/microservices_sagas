import { Component, OnInit, Inject, ViewChild, Injectable, Input, Output} from '@angular/core';
import { NgForm } from '@angular/forms';
import {NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SignUpComponent } from '../signUp/signUp.component';
import { EventEmitter } from 'events';
import { UserService } from 'src/app/Services/userService';
import { UserType } from 'src/app/entities/userType/userType';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'angularx-social-login';
import { FacebookLoginProvider, GoogleLoginProvider } from 'angularx-social-login';

@Component({
    selector: 'app-login',
    templateUrl: './logIn.component.html',
    styleUrls: ['./logIn.component.css']
})

export class LogInComponent implements OnInit {
  @Input() public loggedIn;
  @Output() passEntry: EventEmitter;
  @ViewChild('f') courseForm: NgForm;
  model = {
    UserName: '',
    Password: ''
  }

  //create array to store user data we need
  //userData: any [] = [];
  // create a field to hold error messages so we can bind it to our        template
  userData = {
    UserId: '',
    Provider: '',
    FirstName: '',
    LastName:'',
    EmailAddress: '',
    PictureUrl: '',
    IdToken: '',
    AuthToken: '',
  };
  resultMessage: string;

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService, public activeModal: NgbActiveModal, private modalService: NgbModal, private userService: UserService ) {
  }

  ngOnInit(): void {
  }

  onSubmit(form: NgForm){
    document.getElementById("loginError").textContent = '';
    this.model.UserName = this.courseForm.value.un;
    this.model.Password = this.courseForm.value.password;
    this.userService.login(this.model).subscribe(
      (res: any) => {
        localStorage.setItem('token', res.token);
        this.loggedIn = true;
        this.activeModal.close(this.loggedIn);
        //this.router.navigateByUrl('/home');
        document.getElementById("loginError").textContent = 'Success';
      },
      err => {
        if (err.status == 400) {
          this.toastr.error('Incorrect username or password.', 'Authentication failed.');
          document.getElementById("loginError").textContent = err.error.message;
        }
        else
          console.log(err);
      }
    );
  }

  signUp()
  {
    this.activeModal.close();
    const modalRef = this.modalService.open(SignUpComponent);
  }

  close()
  {
    this.activeModal.close();
  }

  //logIn with google method. Takes the platform (Google) parameter.
  logInWithGoogle(platform: string): void {
    platform = GoogleLoginProvider.PROVIDER_ID;
    //Sign In and get user Info using authService that we just injected
    this.authService.signIn(platform).then(
      (response) => {
        //Get all user details
        console.log(platform + ' logged in user data is= ' , response);
        //Take the details we need and store in an array
        this.userData.FirstName = response.firstName;
        this.userData.UserId = response.id;
        this.userData.LastName = response.lastName;
        this.userData.EmailAddress = response.email;
        this.userData.IdToken = response.idToken;
        this.userData.AuthToken = response.authToken;
        this.userService.socialLogIn(this.userData).subscribe(
          (res: any) => {
            localStorage.setItem('token', res.token);
            this.loggedIn = true;
            this.activeModal.close(this.loggedIn);
          },
          err => {
            if (err.status == 400) {
              var mess = this.userData;
              alert(this.userData.FirstName);
              console.log(err);
            }
            else
              console.log(err);
          }
        );
      },
      (error) => {
        console.log(error);
        this.resultMessage = error;
      }
    );
  }

  //logIn with google method. Takes the platform (Google) parameter.
  logInWithFacebook(platform: string): void {
    platform = FacebookLoginProvider.PROVIDER_ID;
    //Sign In and get user Info using authService that we just injected
    this.authService.signIn(platform).then(
      (response) => {
        //Get all user details
        console.log(platform + ' logged in user data is= ' , response);
        //Take the details we need and store in an array
        this.userData.FirstName = response.firstName;
        this.userData.UserId = response.id;
        this.userData.LastName = response.lastName;
        this.userData.EmailAddress = response.email;
        this.userData.IdToken = response.idToken;
        this.userData.AuthToken = response.authToken;
        this.userService.socialLogInFacebook(this.userData).subscribe(
          (res: any) => {
            localStorage.setItem('token', res.token);
            this.loggedIn = true;
            this.activeModal.close(this.loggedIn);
          },
          err => {
            if (err.status == 400) {
              var mess = this.userData;
              alert(this.userData.FirstName);
              console.log(err);
            }
            else
              console.log(err);
          }
        );
      },
      (error) => {
        console.log(error);
        this.resultMessage = error;
      }
    );
  }
}
