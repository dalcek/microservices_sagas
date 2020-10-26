using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AirlinesMicroservice.Models.Model
{
	public class Flight
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string IdCompany { get; set; }
		public string Duration { get; set; }
		public string Extra { get; set; }
		public Flight Povratnilet { get; set; }
		public int Price { get; set; }
		public int NumOfPassengers { get; set; }
		public string DepartureDate { get; set; }
		public Luggage Luggage { get; set; }


		public Destination From { get; set; }
		public Destination To { get; set; }

		public List<Destination> Stops { get; set; }
		public int Rate { get; set; }
		public List<Rater> Raters { get; set; }
		public List<Seat> Seats { get; set; }
		public Trip Trip { get; set; }
		public List<SoldTicket> SoldTickets { get; set; }
		public List<Ticket> AllTickets { get; set; }
		[Timestamp]
		public byte[] RowVersion { get; set; }

	

		public Flight() { }

	}
	public class Rater
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int Rate { get; set; }

		public Rater() { }
	}

	public class SoldTicket
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public Ticket ticket { get; set; }

		public SoldTicket() { }
	}

	public class Discount
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public double BronzeTier { get; set; }
		public double SilverTier { get; set; }
		public double GoldTier { get; set; }
		public double DiscountPercent { get; set; }

	}
	public class IdModel
	{
		public int id { get; set; }
		public IdModel() { }
	}
	public class Airline
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public double Lon { get; set; }
		public double Lat { get; set; }
		public string Description { get; set; }
		public List<Destination> Destinations { get; set; }
		public List<DestinationPopular> PopDestinaations { get; set; }
		public string Img { get; set; }
		public List<Flight> Flights { get; set; }
		public int Rating { get; set; }
		public List<Rater> Raters { get; set; }
		public int Admin { get; set; }
		public List<Ticket> FastTickets { get; set; }

		[Timestamp]
		public byte[] RowVersion { get; set; }

		public Airline()
		{

		}
	}
	public class DestinationPopular
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public Destination destination { get; set; }
		public DestinationPopular() { }
	}

	public enum Classes
	{
		Economy = 0,
		Business = 1,
		First = 2
	}
	public class Destination
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Img { get; set; }

		public string Description { get; set; }

		public Destination() { }
	}
	public class AddDestinationModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Img { get; set; }

		public string Description { get; set; }
		public int CompanyId { get; set; }

		public AddDestinationModel() { }
	}
	public class Luggage
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int Weight { get; set; }
		public int Quantity { get; set; }
		public string Dimensions { get; set; }

		public Luggage() { }

	}
	public class Seat
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public bool Taken { get; set; }
		public bool IsSelected { get; set; }
		public Traveller Traveller { get; set; }
		public Classes Type { get; set; }

		public Seat() { }
	}
	public class Ticket
	{
		[Key]
		public int Id { get; set; }
		public Flight Flight { get; set; }
		public Seat Seat { get; set; }
		public int Discount { get; set; }
		public bool gaveRate { get; set; }
		public bool gaveRateCompany { get; set; }
		public string userId { get; set; }
		public string reqTick { get; set; }
		public Ticket() { }
	}
	public class Traveller
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }
		public string IdUser { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Passport { get; set; }

		public Traveller() { }
	}
	public enum Trip
	{
		One_way = 0,
		Round_trip = 1,
	}
}
