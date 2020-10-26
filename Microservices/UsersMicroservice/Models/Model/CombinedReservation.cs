using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroservice.Models.Model
{
    //public class CombinedReservation
    //{
    //    public CarReservation car { get; set; }
    //    public List<TicketReservation> tickets { get; set; }
    //}

    public class CarReservation
    {
        public int company { get; set; }
        public int car { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string pickUpAddr { get; set; }
        public string dropOffAddr { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public List<int> extras { get; set; }
        public int quickRes { get; set; }
        public double price { get; set; }
        public double silver { get; set; }
        public double bronze { get; set; }
        public double gold { get; set; }
        public double percent { get; set; }

        public CarReservation() { }
    }

    public class TicketReservation
    {
        public Seat seat { get; set; }
        public int b { get; set; }
        public int s { get; set; }
        public int g { get; set; }
        public int d { get; set; }
        public TicketReservation() { }
    }

    public class Seat
    {
        public int Id { get; set; }
        public bool Taken { get; set; }
        public bool IsSelected { get; set; }
        public Traveller Traveller { get; set; }
        public Classes Type { get; set; }

        public Seat() { }
    }

    public class Traveller
    {
        public string Id { get; set; }
        public string IdUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Passport { get; set; }

        public Traveller() { }
    }

    public enum Classes
    {
        Economy = 0,
        Business = 1,
        First = 2
    }
}
