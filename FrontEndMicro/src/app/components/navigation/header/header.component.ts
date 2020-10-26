import { Component, OnInit, Inject, Injectable, Input } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { LogInComponent } from '../../logIn/logIn.component';
import { NgbModal} from '@ng-bootstrap/ng-bootstrap';
import { Route } from '@angular/compiler/src/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/entities/user/user';
import { UserService } from 'src/app/Services/userService';
import { DataService } from "src/app/data.service";
import * as jwt_decode from "jwt-decode";
import { AuthService } from 'angularx-social-login';
import { FacebookLoginProvider, GoogleLoginProvider } from 'angularx-social-login';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  isLoggedIn: string;
  loggedIn: boolean;
  receivedEntry: boolean;

  constructor(private authService: AuthService, private dataService: DataService,private modalService: NgbModal, private route: Router, private router: ActivatedRoute, private logService: UserService) {

  }


  ngOnInit(): void {
    this.dataService.sharedMessage.subscribe(message => this.isLoggedIn = message);
    var token = localStorage.getItem('token');
    if(token != null) {
      var decoded = this.getDecodedAccessToken(token);
      var now = Date.now();
      if(decoded != null && (decoded.exp * 1000) > now) {
        this.loggedIn = true;
        this.isLoggedIn = "true";
        this.SendMessage("true");
      }
      else {
        this.loggedIn = false;
        this.isLoggedIn = "false";
        this.SendMessage("false");
      }
    } 
    else {
      this.loggedIn = false;
      this.isLoggedIn = "false";
        this.SendMessage("false");
    }
  }

  getDecodedAccessToken(token: string): any {
    try{
        return jwt_decode(token);
    }
    catch(Error){
        return null;
    }
  }

  logout(): void
  {
    this.authService.signOut();
    this.loggedIn = false;
    this.logService.logOut();
    this.SendMessage("false");
    localStorage.removeItem('token');
  }
  klik(): void {
    const modalRef = this.modalService.open(LogInComponent);
    modalRef.componentInstance.loggedIn = this.loggedIn;
    modalRef.result.then((result) => {if (result) { this.loggedIn = result; this.isLoggedIn= "true"; this.SendMessage("true"); }
    });
  }
  myprofile(): void
  {
    // preuzimamo token iz localstorage
    var token = localStorage.getItem('token');
    var decoded = this.getDecodedAccessToken(token);
    var type = decoded.Roles;
    if(type == "RegisteredUser") {
      this.route.navigate(['/user']);
    }
    else if (type == "RentACarAdministrator") {
      this.route.navigate(['/carRentalAdmin']);
    }
    else if (type == "AirlineAdministrator") {
      this.route.navigate(['/airlineAdmin']);
    }
    else if (type == "WebsiteAdministrator") {
      this.route.navigate(['/websiteAdmin']);
    }
  }

  SendMessage(message: string) {
    this.dataService.nextMessage(message);
  }
}

