using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CarsMicroservice.Models.ContextData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarsMicroservice.Models.Model
{
    public class RentACar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public Address MainLocation { get; set; }
        public List<Address> Locations { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<Car> Cars { get; set; }
        public List<Rating> Ratings { get; set; }
        public int AvrageRating { get; set; }
        public List<ExtraAmenity> extras { get; set; }
        public List<QuickReservation> QuickReservations { get; set; }
        public List<CarReservation> Reservations { get; set; }
        public bool Activated { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public RentACar()
        {

        }
    }

    public class Car
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public double PricePerDay { get; set; }
        public string Type { get; set; }
        public int Passengers { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public int CompanyId { get; set; }
        public List<Rating> Ratings { get; set; }
        public int AvrageRating { get; set; }
        public List<Date> RentedDates { get; set; }
        public bool Removed { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }


        public Car()
        {

        }
    }

    public class Address 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FullAddress { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Address()
        {

        }
    }

    public class ExtraAmenity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }

        public bool OneTimePayment { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ExtraAmenity()
        {

        }
    }

    public class QuickReservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Car DiscountedCar { get; set; }
        public List<Date> Dates { get; set; }

        public QuickReservation()
        {

        }
    }

    public class Rating
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Rate { get; set; }

        public Rating()
        {

        }
    }

    public class Date
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DateStr { get; set; }

        public Date()
        {

        }
    }

    public class CarReservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Car Car { get; set; }
        public List<Date> Dates { get; set; }
        public int RatedCar { get; set; }
        public int RatedCompany { get; set; }
        public string PickUpLocation { get; set; }
        public string ReturnLocation { get; set; }
        public string PickUpTime { get; set; }
        public string ReturnTime { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<ExtraAmenity> Extras { get; set; }
        public double TotalPrice { get; set; }
        public string ResUser { get; set; }
        public int SeatReservationId { get; set; }

        public CarReservation() { }
    }



}
