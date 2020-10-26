using Microsoft.EntityFrameworkCore;
using AirlinesMicroservice.Models.ContextData;
using AirlinesMicroservice.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlinesMicroservice
{
    public class AirlineDataIntializer
    {
        public static void SeedAirlines(AirlineContext con)
        {
            if (con.AirlineCompanies.Count() != 0)
                return;
            Airline airline = new Airline();

            Destination d1 = new Destination();
            d1.Name = "Belgrade";
            d1.Description = "The capital and largest city of Serbia. It is located at the confluence of the Sava and Danube";
            d1.Img = "belgrade.jpg";
            Destination d2 = new Destination();
            d2.Name = "Paris";
            d2.Description = "Paris contains the most visited monuments in the city, including the Notre Dame Cathedral and the Louvre as well as the Sainte-Chapelle and the Eiffel Tower.";
            d2.Img = "paris.jpg";
            Destination d3 = new Destination();
            d3.Name = "Banja Luka";
            d3.Description = "The city lies on the Vrbas River and is well known in the countries of the former Yugoslavia for being full of tree-lined avenues, boulevards, gardens, and parks.";
            d3.Img = "banjaluka.jpg";

            Destination d4 = new Destination();
            d4.Name = "London";
            d4.Description = "London is considered to be one of the  most important global cities and has been called the most powerful, most desirable, most influential and most visited city.";
            d4.Img = "london.jpg";

            airline.Lat = 45.254410;
            airline.Lon = 19.842550;
            airline.Name = "Serbian air";
            airline.FastTickets = new List<Ticket>();
            airline.Raters = new List<Rater>();
            airline.Address = "Belgrade, Serbia";
            airline.Description = "Svi direktni letovi kompanije Air Serbia Beograd na jednom mestu. Ukoliko želite direktno da letite za Pariz, Amasterdam, Njujork, Tivat, Rim, Milano i druge evropske gradove Air Serbia je pravi izbor za vas!";
            airline.Destinations = new List<Destination>();
            airline.Destinations.Add(d1);
            airline.Destinations.Add(d2);
            airline.Destinations.Add(d3);
            airline.Destinations.Add(d4);
            airline.Img = "air-serbia.jpg";
            airline.Rating = 0;

            Flight flight = new Flight();
           /* flight.SoldTickets = new List<Ticket>();
            flight.Extra = "Perfect your flight experience with full-flat seats, special food offers and other privileges.";
            flight.Luggage = new Luggage() { Dimensions = "20x30", Quantity = 200, Weight = 122 };
            flight.Rate = 0;
            flight.Raters = new List<Rater>();
            flight.IdCompany = "Serbian air";
            flight.Trip = Trip.One_way;
            flight.Seats = new List<Seat>();
            flight.From = d1;
            flight.To = d2;
            flight.DepartureDate = "10.29.1203";

            flight.NumOfPassengers = 300;
            flight.Price = 300;
            flight.Duration = "2";
            flight.Stops = new List<Destination>();


            Seat s1 = new Seat();
            s1.Type = Classes.First;
            s1.Traveller = new Traveller();
            s1.Taken = true;
            s1.IsSelected = false;

            Seat s2 = new Seat();
            s2.Type = Classes.First;
            s2.Traveller = new Traveller();
            s2.Taken = false;
            s2.IsSelected = false;

            Seat s3 = new Seat();
            s3.Type = Classes.First;
            s3.Traveller = new Traveller();
            s3.Taken = false;
            s3.IsSelected = false;

            Seat s4 = new Seat();
            s4.Type = Classes.First;
            s4.Traveller = new Traveller();
            s4.Taken = false;
            s4.IsSelected = false;

            Seat s5 = new Seat();
            s5.Type = Classes.First;
            s5.Traveller = new Traveller();
            s5.Taken = true;
            s5.IsSelected = false;

            Seat s6 = new Seat();
            s6.Type = Classes.First;
            s6.Traveller = new Traveller();
            s6.Taken = false;
            s6.IsSelected = false;

            Seat s7 = new Seat();
            s7.Type = Classes.First;
            s7.Traveller = new Traveller();
            s7.Taken = false;
            s7.IsSelected = false;

            Seat s8 = new Seat();
            s8.Type = Classes.First;
            s8.Traveller = new Traveller();
            s8.Taken = false;
            s8.IsSelected = false;

            Seat s9 = new Seat();
            s9.Type = Classes.Business;
            s9.Traveller = new Traveller();
            s9.Taken = false;
            s9.IsSelected = false;

            Seat s10 = new Seat();
            s10.Type = Classes.Business;
            s10.Traveller = new Traveller();
            s10.Taken = false;
            s10.IsSelected = false;

            Seat s11 = new Seat();
            s11.Type = Classes.Business;
            s11.Traveller = new Traveller();
            s11.Taken = false;
            s11.IsSelected = false;

            Seat s12 = new Seat();
            s12.Type = Classes.Business;
            s12.Traveller = new Traveller();
            s12.Taken = true;
            s12.IsSelected = false;

            Seat s13 = new Seat();
            s13.Type = Classes.Economy;
            s13.Traveller = new Traveller();
            s13.Taken = true;
            s13.IsSelected = false;

            Seat s14 = new Seat();
            s14.Type = Classes.Economy;
            s14.Traveller = new Traveller();
            s14.Taken = true;
            s14.IsSelected = false;

            Seat s15 = new Seat();
            s15.Type = Classes.Economy;
            s15.Traveller = new Traveller();
            s15.Taken = true;
            s15.IsSelected = false;

            Seat s16 = new Seat();
            s16.Type = Classes.Economy;
            s16.Traveller = new Traveller();
            s16.Taken = true;
            s16.IsSelected = false;


            flight.Seats.Add(s1);
            flight.Seats.Add(s2);
            flight.Seats.Add(s3);
            flight.Seats.Add(s4);
            flight.Seats.Add(s5);
            flight.Seats.Add(s6);
            flight.Seats.Add(s7);
            flight.Seats.Add(s8);
            flight.Seats.Add(s9);
            flight.Seats.Add(s10);
            flight.Seats.Add(s11);
            flight.Seats.Add(s12);
            flight.Seats.Add(s13);
            flight.Seats.Add(s14);
            flight.Seats.Add(s15);
            flight.Seats.Add(s16);
           */
            airline.Flights = new List<Flight>();
            //airline.Flights.Add(flight);

            con.AirlineCompanies.Add(airline);


            
            con.SaveChanges();
        }
    }
}

