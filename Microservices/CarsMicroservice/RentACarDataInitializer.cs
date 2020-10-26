using CarsMicroservice.Models.ContextData;
using CarsMicroservice.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace CarsMicroservice
{
    public static class RentACarDataInitializer
    {
        
        public static void SeedCompanies(RentACarContext context)
        {
            
            if (context.RentACarCompanies.Count() != 0)
                return;
            Address addr1 = new Address() { FullAddress = "Bulevar Oslobodjenja 81, Novi Sad, Serbia", Latitude = 45.254152, Longitude = 19.836035 };
            Address addr2 = new Address() { FullAddress = "Blok 28, Belgrade, Serbia", Latitude = 44.812298, Longitude = 20.417439 };
            RentACar company1 = new RentACar();
            company1.Activated = true;
            company1.AvrageRating = 0;
            company1.Cars = new List<Car>();
            company1.Description = "No description";
            company1.extras = new List<ExtraAmenity>();
            company1.Image = "kompanija1.jpg";
            company1.Locations = new List<Address>() { addr1, addr2 };
            company1.MainLocation = new Address() { FullAddress = "Vukovarska 2, Belgrade, Serbia", Latitude = 44.792089, Longitude = 20.430300 };
            company1.Name = "National Car Rental";
            company1.QuickReservations = new List<QuickReservation>();
            company1.Ratings = new List<Rating>();

            ExtraAmenity amenity1 = new ExtraAmenity();
            amenity1.Name = "Additional driver";
            amenity1.Image = "addDriver2.png";
            amenity1.Price = 100;
            amenity1.OneTimePayment = false;

            ExtraAmenity amenity2 = new ExtraAmenity();
            amenity2.Name = "GPS Navigation";
            amenity2.Image = "GPS2.png";
            amenity2.Price = 20;
            amenity2.OneTimePayment = true;

            ExtraAmenity amenity3 = new ExtraAmenity();
            amenity3.Name = "Unlimited miles";
            amenity3.Image = "UnlimitedMiles2.png";
            amenity3.Price = 80;
            amenity3.OneTimePayment = true;

            ExtraAmenity amenity4 = new ExtraAmenity();
            amenity4.Name = "Child & baby seats";
            amenity4.Image = "childSeats2.png";
            amenity4.Price = 40;
            amenity4.OneTimePayment = true;

            ExtraAmenity amenity5 = new ExtraAmenity();
            amenity5.Name = "WiFi hotspot device";
            amenity5.Image = "wifi2.png";
            amenity5.Price = 25;
            amenity5.OneTimePayment = true;

            ExtraAmenity amenity6 = new ExtraAmenity();
            amenity6.Name = "Car rental protection plan";
            amenity6.Image = "protectionPlan2.png";
            amenity6.Price = 75;
            amenity6.OneTimePayment = true;

            ExtraAmenity amenity7 = new ExtraAmenity();
            amenity7.Name = "Prepaid fuel";
            amenity7.Image = "fuel2.png";
            amenity7.Price = 30;
            amenity7.OneTimePayment = true;

            ExtraAmenity amenity8 = new ExtraAmenity();
            amenity8.Name = "Extended roadside protection";
            amenity8.Image = "roadsideProtection2.png";
            amenity8.Price = 65;
            amenity8.OneTimePayment = true;

            ExtraAmenity amenity9 = new ExtraAmenity();
            amenity9.Name = "Winter tires & snow chains";
            amenity9.Image = "snow2.png";
            amenity9.Price = 50;
            amenity9.OneTimePayment = true;

            ExtraAmenity amenity10 = new ExtraAmenity();
            amenity10.Name = "Delivery & collection";
            amenity10.Image = "driver2.png";
            amenity10.Price = 100;
            amenity10.OneTimePayment = true;

            List<ExtraAmenity> amenities = new List<ExtraAmenity>();
            amenities.Add(amenity1);
            amenities.Add(amenity2);
            amenities.Add(amenity3);
            amenities.Add(amenity4);
            amenities.Add(amenity5);
            amenities.Add(amenity6);
            amenities.Add(amenity7);
            amenities.Add(amenity8);
            amenities.Add(amenity9);
            amenities.Add(amenity10);
            company1.extras = amenities;

            context.RentACarCompanies.Add(company1);
            context.SaveChanges();

            List<RentACar> companies = context.RentACarCompanies.ToList();

            Car car1 = new Car();
            car1.AvrageRating = 0;
            car1.Brand = "Ford";
            car1.CompanyId = companies[0].ID;
            car1.Image = "fusion.jpg";
            car1.Location = addr1.FullAddress;
            car1.Model = "Fusion";
            car1.Passengers = 5;
            car1.PricePerDay = 70;
            car1.Ratings = new List<Rating>();
            car1.RentedDates = new List<Date>();
            car1.Type = "full-size";
            car1.Year = 2015;
            car1.Removed = false;

            Car car2 = new Car();
            car2.AvrageRating = 0;
            car2.Brand = "WW";
            car2.CompanyId = companies[0].ID;
            car2.Image = "audia4.jpg";
            car2.Location = addr2.FullAddress;
            car2.Model = "Audi A4";
            car2.Passengers = 5;
            car2.PricePerDay = 85;
            car2.Ratings = new List<Rating>();
            car2.RentedDates = new List<Date>();
            car2.Type = "standard-size";
            car2.Year = 2015;
            car2.Removed = false;

            companies[0].Cars = new List<Car>();
            companies[0].Cars.Add(car1);
            companies[0].Cars.Add(car2);
            context.RentACarCompanies.Update(companies[0]);
            context.SaveChanges();

        }
    }
}
