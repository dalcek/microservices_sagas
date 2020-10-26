import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { Router } from "@angular/router";
import { CookieService } from "ngx-cookie-service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private router: Router, private cookieService:CookieService) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var jwt = this.cookieService.get('jwt')

        if (localStorage.getItem('token') != null) {
            const clonedReq = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + localStorage.getItem('token'))
            });
            return next.handle(clonedReq).pipe(
                tap(
                    succ => { },
                    err => {
                        if (err.status == 401){
                            localStorage.removeItem('token');
                            this.router.navigateByUrl('');
                        }
                    }
                )
            )
        }
        else
            return next.handle(req.clone());
    }
}