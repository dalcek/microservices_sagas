import { Injectable } from '@angular/core';
import { 
  Router,
  CanActivate,
  ActivatedRouteSnapshot
} from '@angular/router';
import { UserService } from 'src/app/Services/userService';
import * as jwt_decode from "jwt-decode";

@Injectable()
export class RoleGuard implements CanActivate {
  constructor(public userService: UserService, public router: Router) {}
  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data.expectedRole;
    var token = localStorage.getItem('token');
    var decoded = this.getDecodedAccessToken(token);
    if (token == null || (decoded.exp * 1000) <= Date.now()) {
        alert("Not registered");
        return ;
    }
    if(decoded.Roles !== expectedRole) 
    {
        alert("Wrong role");
        return ;
    }
    return true;
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

