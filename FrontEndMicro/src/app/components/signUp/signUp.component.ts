import { Component, OnInit, ViewChild } from '@angular/core';
import {NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-signup',
    templateUrl: './signUp.component.html',
    styleUrls: ['./SignUp.component.css']
})

export class SignUpComponent implements OnInit {
    moreOptionsActive = false;
    file: File = null;
    user: User;
    model = {
      Password: '',
      FullName: '',
      Address: '',
      Username: '',
      //Birthday: Date,
      Email: '',
      Passport: ''
    }

    firstFormGroup = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      username: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    }, {validator: this.checkIfMatchingPasswords('password', 'confirmPassword')});

    secondFormGroup = this.fb.group({
      fullName: ['', Validators.required],
      address: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern]],
      dateOfBirth: ['', Validators.required],
      passport: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(8)]],
    });



    registerForm = this.fb.group({
        UserName : ['', Validators.required],
        Email : ['', [Validators.required, Validators.email]],
        Password : ['', Validators.required],
        Passport : ['', Validators.required],
        ConfirmPassword : ['', Validators.required],
        Name : ['', Validators.pattern],
        Surname : ['', Validators.pattern],
        PhoneNumber : ['', Validators.pattern],
        DateOfBirth : ['', Validators.pattern],
        Address : ['']
      });

    @ViewChild('f') courseForm: NgForm;
    constructor(private toastr: ToastrService, public activeModal: NgbActiveModal, private fb: FormBuilder, private loginService: UserService ) { }
    optionsClick(){
        this.moreOptionsActive = !this.moreOptionsActive;
      }
    ngOnInit(): void {
      document.getElementById("regError").style.display = 'none';
    }
    onSubmit()
    {
        // tslint:disable-next-line: triple-equals
       // if (this.registerForm.value.UserName == 'stanka')
        // tslint:disable-next-line: max-line-length
        // this.user = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), passport: 'A13KD1', birthday: new Date(2020, 4, 1, 22, 0, 0, 0), sentRequests: new Array<User>(), friendRequests: new Array<User>(), id: 1, firstName: this.registerForm.value.Name,
        //     lastName: this.registerForm.value.Surname, email: this.registerForm.value.Email,
        //     address: this.registerForm.value.Address, password: this.registerForm.value.Password,
        //  username: this.registerForm.value.UserName, type: 1, myFlights: new Array<Ticket>(), rentedCars: Array<CarReservation>(),
        // friends: Array<User>(), points: 0};

        // this.loginService.addUser(this.user);
        // this.activeModal.close();
       if (this.registerForm.value.Password == this.registerForm.value.ConfirmPassword){
      
          this.model.Password = this.registerForm.value.Password;
          this.model.FullName = this.registerForm.value.Name + ' ' + this.registerForm.value.Surname; 
          this.model.Username = this.registerForm.value.UserName;
          this.model.Address = this.registerForm.value.Address;
          //this.model.Birthday = this.registerForm.value.DateOfBirth;
          this.model.Email = this.registerForm.value.Email;
          this.model.Passport = this.registerForm.value.Passport;
          
          this.loginService.register(this.model).subscribe(
              (res: any) => {
                localStorage.setItem('token', res.token);
                this.activeModal.close();
              },
              err => {
                if (err.status == 400)
                  this.toastr.error('User name exists!');
                else
                  console.log(err);
              }
            );
      }
    }

    checkPassword(pass: string, confirmPass: string): boolean{
        return pass === confirmPass;
    }

    Submit() {
      if(this.firstFormGroup.valid && this.secondFormGroup.valid){
        var date = new Date(this.secondFormGroup.value.dateOfBirth);
        var model = {
          username: this.firstFormGroup.value.username,
          email: this.firstFormGroup.value.email,
          password: this.firstFormGroup.value.password,
          fullName: this.secondFormGroup.value.fullName,
          address: this.secondFormGroup.value.address,
          phone: this.secondFormGroup.value.phoneNumber,
          passport: this.secondFormGroup.value.passport,
          birthday: date.toDateString()
        }
        this.loginService.register(model).subscribe(
          (res: any) => {
            document.getElementById("regError").style.display = 'none';
            localStorage.setItem('token', res.token);
            this.activeModal.close();
          },
          err => {
            document.getElementById("regError").textContent = err.error.message;
            document.getElementById("regError").style.display = 'block';
          }
        );
      }
    }

    checkIfMatchingPasswords(passwordKey: string, passwordConfirmationKey: string) {
      return (group: FormGroup) => {
        let passwordInput = group.controls[passwordKey],
            passwordConfirmationInput = group.controls[passwordConfirmationKey];
        if (passwordInput.value !== passwordConfirmationInput.value) {
          return passwordConfirmationInput.setErrors({notEquivalent: true})
        }
        else {
            return passwordConfirmationInput.setErrors(null);
        }
      }
    }

}
