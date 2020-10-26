import { NgModule } from '@angular/core';
import { Routes, RouterModule, CanActivate } from '@angular/router';
import { CarComponent } from './components/car/car.component';
import { FlightComponent } from './components/flight/flight.component';
import { SignUpComponent } from './components/signUp/signUp.component';
import { UserComponent } from './components/user/user.component';
import { HomeComponent } from './components/home/home.component';
import { LogInComponent } from './components/logIn/logIn.component';
import { componentFactoryName } from '@angular/compiler';
import { AirlinesComponent } from './components/airlines/airlines.component';
import { AirlineComponent } from './components/airline/airline.component';
import { CarRentalCompaniesComponent } from './components/carRentalCompanies/carRentalCompanies.component';
import { CarRentalComponent } from './components/carRental/carRental.component';
import { DestinationsComponent } from './components/destinations/destinations.component';
import { RentComponent } from './components/carRental/rent/rent.component';
import { CarRentalAdminComponent } from './components/carRentalAdmin/carRentalAdmin.component';
import { AirlineAdminComponent } from './components/airlineAdmin/airlineAdmin.component';
import { TicketComponent } from './components/ticket/ticket.component';
import { WebsiteAdminComponent } from 'src/app/components/websiteAdmin/websiteAdmin.component';
import { AuthGuard } from 'src/app/guards/authGuard'
import { RoleGuard } from 'src/app/guards/roleGuard'

const routes: Routes = [
  {
    path: 'li',
    component: LogInComponent
  },
  {
    path: 'su',
    component: SignUpComponent
  },
  {
    path: 'flight',
    component: FlightComponent
  },
  {
    path: 'cars',
    component: CarComponent
  },
  {
    path: 'destinations',
    component: DestinationsComponent
  },
  {
    path: 'user',
    component: UserComponent,
    canActivate: [RoleGuard],
    data: { 
      expectedRole: 'RegisteredUser'
    } 
  },
  {
    path: 'carRentalAdmin',
    component: CarRentalAdminComponent,
    canActivate: [RoleGuard],
    data: { 
      expectedRole: 'RentACarAdministrator'
    } 
  },
  {
    path: 'airlineAdmin',
    component: AirlineAdminComponent,
    canActivate: [RoleGuard],
    data: { 
      expectedRole: 'AirlineAdministrator'
    } 
  },
  {
    path: 'websiteAdmin',
    component: WebsiteAdminComponent,
    canActivate: [RoleGuard],
    data: { 
      expectedRole: 'WebsiteAdministrator'
    } 
  },
  {
    path: 'airlines',
    children: [
      { path: '', component: AirlinesComponent },
      { path: ':idA/airline',
        children: [
          { path: '', component: AirlineComponent },
          { path: ':idF/flight', component: FlightComponent },
          { path: ':tickets', component: TicketComponent },
        ]
      }
    ]
  },
  {
    path: 'carCompanies',
    children: [
      { path: '', component: CarRentalCompaniesComponent },
      { path: ':idComp/carCompany', 
        children: [
          { path: '', component: CarRentalComponent },
          { path: ':idCar/rent', 
            component: RentComponent, 
            canActivate: [RoleGuard],
            data: { 
              expectedRole: 'RegisteredUser'
            }   
          }
        ] 
      }
    ]
  },
  {
    path: '',
    component: HomeComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
