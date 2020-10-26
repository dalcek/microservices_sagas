import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AgmCoreModule } from '@agm/core';

import { ToastrModule } from 'ngx-toastr';
import { CookieService } from 'ngx-cookie-service';
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";

import { MaterialModule} from './material.module';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { FlexLayoutModule } from '@angular/flex-layout';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CarComponent } from './components/car/car.component';
import { LogInComponent } from './components/logIn/logIn.component';
import { SignUpComponent } from './components/signUp/signUp.component';
import { FlightComponent } from './components/flight/flight.component';
import { HomeComponent } from './components/home/home.component';
import { SidenavListComponent } from './components/navigation/sidenav-list/sidenav-list.component';
import { HeaderComponent } from './components/navigation/header/header.component';
import { AirlinesComponent } from './components/airlines/airlines.component';
import { AirlineComponent } from './components/airline/airline.component';
import { CarRentalCompaniesComponent } from './components/carRentalCompanies/carRentalCompanies.component';
import { CarRentalComponent } from './components/carRental/carRental.component';
import { NavtabsComponent } from './components/navigation/navtabs/navtabs.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FriendsComponent } from './components/inviteFriends/inviteFriends.component';
import { DestinationsComponent } from './components/destinations/destinations.component';
import { RentComponent } from './components/carRental/rent/rent.component';
import { UserComponent } from './components/user/user.component';
import { ChangeUserComponent } from './components/changeUser/changeUser.component';
import { CarRentalAdminComponent } from './components/carRentalAdmin/carRentalAdmin.component';
import { AirlineAdminComponent } from './components/airlineAdmin/airlineAdmin.component';
import { TicketComponent } from './components/ticket/ticket.component';
import { WebsiteAdminComponent } from 'src/app/components/websiteAdmin/websiteAdmin.component';

import { AuthInterceptor } from './auth/auth.interceptor';
import { TokenInterceptor } from './auth/tokenInterceptor';

import { UserService } from './Services/userService';
import { AirlineService } from './Services/airlineService';
import { CarRentalService } from './Services/carRentalService';

import { AuthGuard } from 'src/app/guards/authGuard';
import { RoleGuard } from 'src/app/guards/roleGuard';

import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule} from '@angular/material/form-field';

import { SocialLoginModule, AuthServiceConfig } from 'angularx-social-login';
import { GoogleLoginProvider, FacebookLoginProvider } from 'angularx-social-login';


let config = new AuthServiceConfig([
  {
     id: GoogleLoginProvider.PROVIDER_ID,
     provider: new GoogleLoginProvider('851213113952-uuovjkarrpbimffbkt9ilfdvvk9pf327.apps.googleusercontent.com')
  },
{
     id: FacebookLoginProvider.PROVIDER_ID,
     provider: new FacebookLoginProvider('205952697166076')
  },
]);
export function provideConfig()
 {
    return config;
 }


@NgModule({
  entryComponents: [ LogInComponent ],
  declarations: [
    AppComponent,
    CarComponent,
    LogInComponent,
    SignUpComponent,
    FlightComponent,
    HomeComponent,
    SidenavListComponent,
 
    HeaderComponent,
    NavtabsComponent,
    AirlinesComponent,
    AirlineComponent,
    CarRentalCompaniesComponent,
    CarRentalComponent,
    FriendsComponent,
    DestinationsComponent,
    RentComponent,
    UserComponent,
    FriendsComponent,
    ChangeUserComponent,
    CarRentalAdminComponent,
    AirlineAdminComponent,
    TicketComponent,
    WebsiteAdminComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MaterialModule,
    FlexLayoutModule,
    MatDialogModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    AgmCoreModule.forRoot({apiKey: 'AIzaSyDnihJyw_34z5S1KZXp90pfTGAqhFszNJk'}),
    MDBBootstrapModule.forRoot(),
    HttpClientModule,
    ToastrModule.forRoot({
      progressBar: true
    }),
    SocialLoginModule.initialize(config)
  ],
  providers: [ 
    AirlineService, 
    CarRentalService, 
    UserService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    }, 
    AuthGuard, 
    RoleGuard, 
    CookieService,
    {
      provide: AuthServiceConfig,
      useFactory: provideConfig
    }
  ],
  bootstrap: [ AppComponent ],

})
export class AppModule { }
