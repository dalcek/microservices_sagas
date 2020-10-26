import { Injectable } from '@angular/core';
import { Router, CanActivate,ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { UserService } from 'src/app/Services/userService';
import { Observable } from 'rxjs';
import * as jwt_decode from "jwt-decode";

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(public userService: UserService, public router: Router) {}

  canActivate( next: ActivatedRouteSnapshot,state: RouterStateSnapshot)
      : Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    var token = localStorage.getItem('token');
    var decoded = this.getDecodedAccessToken(token);
    if (token != null && (decoded.exp * 1000) <= Date.now()) {
        return true;
    }
    alert("Not registered");
    return ;
  }

  getDecodedAccessToken(token: string): any {
    try{
        return jwt_decode(token);
    }
    catch(Error){
        return null;
    }
  }
}

