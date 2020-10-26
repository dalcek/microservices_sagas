using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlinesMicroservice.Models.Model
{
    public class AirlineModel
    {
    }
    public class AirlineListingInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MainLocation { get; set; }
        public string Img { get; set; }
        public int Rating { get; set; }

        public double Lon { get; set; }
        public double Lat { get; set; }
        public string Description { get; set; }
        public List<DestinationListingInfoModel> Destinations { get; set; }
        public List<DestinationListingInfoModel> PopDestinaations { get; set; }

        public List<RaterListingInfoModel> Raters { get; set; }
        public int Admin { get; set; }
        public List<TicketListingInfoModel> FastTickets { get; set; }
        public List<FlightListingInfoModel> Flights { get; set; }
        public AirlineListingInfoModel() { }
    }
    public class UsernameModel
    {
        public string username { get; set; }
        public UsernameModel() { }
    }
    public class IdModel2
    {
        public int Id { get; set; }
        public int pom { get; set; }
        public IdModel2() { }
    }
    public class UpdateCompanyModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string img { get; set; }
    
        public UpdateCompanyModel() { }
    }

    public class AddFlightModel
    {
        public string idCompany { get; set; }
        public string departureTime { get; set; }
        public string departureDate { get; set; }
        public AddFlightModel povratniLet { get; set; }
        public int price { get; set; }
        public string duration { get; set; }
        public LuggageListingInfoModel luggage { get; set; }
        public string extra { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public int airlineId { get; set; }
        public string trip { get; set; }


        public List<string> stops { get; set; }

        public List<SeatListingInfoModel> seats { get; set; }
        public AddFlightModel() { }
    }
    public class EditFlightModel
    {
        public int id { get; set; }
        public string duration { get; set; }
        public string date { get; set; }
        public string date2 { get; set; }
        public int price { get; set; }

        public int lug { get; set; }
        public string extra { get; set; }
 
        public EditFlightModel() { }
    }
    public class FlightListingInfoModel
    {
        public int id { get; set; }
        public string idCompany { get; set; }

        public string departureDate { get; set; }
        public FlightListingInfoModel povratniLet { get; set; }
        public int price { get; set; }
        public string duration { get; set; }
        public LuggageListingInfoModel luggage { get; set; }
        public int numOfPassengers { get; set; }
        public string extra { get; set; }
        public DestinationListingInfoModel from { get; set; }
        public DestinationListingInfoModel to { get; set; }
        public int airlineId { get; set; }
        public Trip trip { get; set; }
        public int rate { get; set; }
        public bool isAdmin { get; set; }


        public List<DestinationListingInfoModel> stops { get; set; }
 
        public List<RaterListingInfoModel> raters { get; set; }
        public List<SeatListingInfoModel> seats { get; set; }
     
        public List<TicketListingInfoModel> soldTickets { get; set; }


        public FlightListingInfoModel() { }
    }

    public class FlightListingInfoModel2
    {
        public int id { get; set; }



        public FlightListingInfoModel2() { }
    }
    public class RaterListingInfoModel
    {
        public int id { get; set; }
        public int rate { get; set; }

        public RaterListingInfoModel() { }
    }
    public class TicketListingInfoModel
    {
        public int id { get; set; }
        public int b { get; set; }
        public int s { get; set; }
        public int g { get; set; }
        public int d { get; set; }
        public FlightTicket flight { get; set; }
        public Seat seat { get; set; }
        public int discount { get; set; }

        public bool gaveRate { get; set; }
        public bool gaveRateCompany { get; set; }



        public TicketListingInfoModel() { }
    }
    public class FlightTicket
    {
        public int id { get; set; }
        public string idCompany { get; set; }
        public Trip trip { get; set; }
        public int prise { get; set; }
        public string duration { get; set; }
        public DestinationListingInfoModel from { get; set; }
        public DestinationListingInfoModel to { get; set; }
        public string departureDate { get; set; }
        public FlightTicket() { }
    }

    public class SeatListingInfoModel
    {
        public int id { get; set; }
        public bool taken { get; set; }
        public bool isSelected { get; set; }
        public TravellerInfo traveller { get; set; }
        public Classes type { get; set; }

        public SeatListingInfoModel() { }
    }
    public class AddTravellerModel
    {
        public int seatId { get; set; }
        public TravellerInfo traveller { get; set; }

        public AddTravellerModel() { }
    }

    public class NewFastTicket
    {
        public int seatId { get; set; }
        public int discount { get; set; }

        public int flightId { get; set; }

        public NewFastTicket() { }
    }
    public class DisableSeatModel
    {
        public int seatId { get; set; }

        public DisableSeatModel() { }
    }
    public class FinishModel
    {
        public  Seat seat { get; set; }
        public int b { get; set; }
        public int s { get; set; }
        public int g { get; set; }
        public int d { get; set; }
        public FinishModel() { }
    }

    public class TravellerInfo
    {
        public string id { get; set; }
        public string idUser { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string passport { get; set; }

        public TravellerInfo() { }
    }
    public class LuggageListingInfoModel
    {

        public int weight { get; set; }
        public int quantity { get; set; }
        public string dimensions { get; set; }

        public LuggageListingInfoModel() { }
    }

    public class DestinationListingInfoModel
    {
		public int id { get; set; }
		public string name { get; set; }
		public string img { get; set; }

		public string description { get; set; }

		public DestinationListingInfoModel() { }
	}
    public class SearchCompaniesModelAirline
    {
        public string airline1Name { get; set; }
        public string airline2Name { get; set; }
        public string from { get; set; }
        public string to { get; set; }

        public SearchCompaniesModelAirline() { }
    }

    public class SearchFlightsModel {
        public string From { get; set; }
        public string To { get; set; }
        public string Prise { get; set; }
        public string Trip { get; set; }
        public string Clas { get; set; }

        public string Date { get; set; }
        public string Date2 { get; set; }
        public SearchFlightsModel() { }
    }

    public class AirCompanyModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public List<string> admins { get; set; }

        public AirCompanyModel() { }
    }

    public class RegisterCompanyModel
    {
        public string companyName { get; set; }
        public string address { get; set; }

        public RegisterCompanyModel() { }
    }

    public class IsAdminModel
    {
        public string id { get; set; }
        public IsAdminModel() { }
    }

    public class IsAdminModelRet
    {
        public int id { get; set; }
        public IsAdminModelRet() { }
    }
}
