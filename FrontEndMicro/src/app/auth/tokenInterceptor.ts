import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { Router } from "@angular/router";
import { CookieService } from "ngx-cookie-service";

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private cookieService: CookieService ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  //   request = request.clone({
  //     headers: new HttpHeaders({
  //       'Authorization': 'CfDJ8Ol7K2Sn7TtCq-IEp6D9bSJFWFNBexxxpkYaQkbxAiDx7PXxtYR11kA-FjEGAC86lIMejEJxLODtnKy-deqxgs-lJoiVo9OYhS6yRWXkabCUf4-yF37skAHskO3-gX-Khh4nATHGZfN3yK3wVUMBmRgmKQ1gb1cWFLa-cJ1EARkdpV8XapUL89Nsj-BEMvQuIogUGLj-zxluXTuZxWcuQE6fgXlGOK-RyUwDaVbNjYASUp-6ZA5lt87kHUjOl_A7zkOpaswWhg9490DT3L76O7Fi7Cybs0d0uPK8XAofg7Cf6CADRAD-fZ_7HBkMZV_mPHr4PbNcBF7nMZgiRHADXbpmuGCnrnbhH_Uzve09kKnV9hRSF5ZwocVS80mqzgYaia4kEU4EHWVEX5Vign0ammyBoVPgSllcO1aT8wPkQ4_MC8GOUm4_N6FFCnHA_-NAQT2y23AbRWbUn5RizKGC-xHowH-7kARmDwxnGEQ4ik2qRBBf9n_jhBkGQ-FaxikbCMbk21IQ7UNYOtZy9k9xKJ4zm2HhR27pbJU1MVv-izBP',
  //     }),
  //   });
    console.log(this.cookieService.get('.AspNetCore.Application'));
    return next.handle(request);
  }

}