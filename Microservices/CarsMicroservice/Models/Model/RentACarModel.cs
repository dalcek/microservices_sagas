using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CarsMicroservice.Models.Model
{
    public class RentACarModel
    {
    }

    public class RentACarListingInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MainLocation { get; set; }
        public string Img { get; set; }
        public int Rating { get; set; }

        public RentACarListingInfoModel() { }
    }

    public class RentACarProfileInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address MainLocation { get; set; }
        public List<Address> Locations { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public List<ExtraAmenity> Extras { get; set; }

        public RentACarProfileInfoModel() { }
    }

    public class SearchCompaniesModel
    {
        public string companyName { get; set; }
        public string location { get; set; }
        public string from { get; set; }
        public string to { get; set; }

        public SearchCompaniesModel() { }
    }

    public class IdModel
    {
        public int id { get; set; }
        public IdModel() { }
    }
    public class IsAdminModel
    {
        public string id { get; set; }
        public IsAdminModel() { }
    }
    public class IdModelCarComp
    {
        public int idComp { get; set; }
        public int idCar { get; set; }
        public IdModelCarComp() { }
    }

    public class IdModel3
    {
        public int id { get; set; }
        public int id2 { get; set; }
        public IdModel3() { }
    }

    public class SearchCarModel
    {
        public int companyId { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string location { get; set; }
        public string returnLocation { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string year { get; set; }
        public string price { get; set; }
        public string passengers { get; set; }

        public SearchCarModel() { }
    }

    public class SearchCarModelHome
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string location { get; set; }
        public string returnLocation { get; set; }
        public string type { get; set; }

        public SearchCarModelHome() { }
    }

    public class CarModel
    {
        public int id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public double price { get; set; }
        public string type { get; set; }
        public int passengers { get; set; }
        public string location { get; set; }
        public string image { get; set; }
        public int avrageRating { get; set; }
        public int companyId { get; set; }

        public CarModel()
        {

        }
    }

    public class CarModelAdmin
    {
        public int id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public double price { get; set; }
        public string type { get; set; }
        public int passengers { get; set; }
        public string location { get; set; }
        public string image { get; set; }
        public int avrageRating { get; set; }
        public List<string> rentedDates { get; set; }
        public List<QuickReservationModel> quickReservations { get; set; }

        public CarModelAdmin()
        {

        }
    }

    public class QuickReservationModel
    {
        public int id { get; set; }
        public int carId { get; set; }
        public string from { get; set; }
        public string to { get; set; }

        public QuickReservationModel()
        {

        }
    }

    public class ExtraAmenityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public bool OneTimePayment { get; set; }
        public bool Selected { get; set; }

        public ExtraAmenityModel()
        {

        }
    }

    public class ReservationModel
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

        public ReservationModel() { }
    }

    public class CarReservationModel
    {
        public int id { get; set; }
        public CarModel car { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string company { get; set; }
        public int companyId { get; set; }
        public double total { get; set; }
        public int ratedCar { get; set; }
        public int ratedCompany { get; set; }
        public string pickUpLocation { get; set; }
        public string pickUpTime { get; set; }
        public string dropOffLocation { get; set; }
        public string dropOffTime { get; set; }
        public string timeStamp { get; set; }
        public List<ExtraAmenity> Extras { get; set; }

        public CarReservationModel() { }
    }

    public class RemoveReservationModel
    {
        public int resId { get; set; }
        public int compId { get; set; }

        public RemoveReservationModel() { }
    }

    public class RateCarCompanyModel
    {
        public int compId { get; set; }
        public int resId { get; set; }
        public int star { get; set; }

        public RateCarCompanyModel() { }
    }

    public class RateCarModel
    {
        public int compId { get; set; }
        public int resId { get; set; }
        public int star { get; set; }
        public int carId { get; set; }

        public RateCarModel() { }
    }

    public class CompanyInfoModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public string logo { get; set; }
        public LocationsModel locations { get; set; }
        public List<CarModelAdmin> cars { get; set; }
        public List<ExtraAmenity> extras { get; set; }
        public bool activated { get; set; }
        public List<int> ratings { get; set; }
        public List<int> dailyReservations { get; set; }
        public List<int> weeklyReservations { get; set; }
        public List<int> monthlyReservations { get; set; }
        public List<string> dailyLabels { get; set; }
        public List<string> weeklyLabels { get; set; }
        public List<string> monthlyLabels { get; set; }

        public CompanyInfoModel() { }
    }

    public class LocationsModel
    {
        public Address mainLocation { get; set; }
        public List<Address> locations { get; set; }

        public LocationsModel() { }
    }


    public class UpdateCompanyInfoModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public string logo { get; set; }
        public int compId { get; set; }

        public UpdateCompanyInfoModel() { }
    }

    public class NewLocationModel 
    {
        public int compId { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        
        public NewLocationModel() { }
    }

    public class EditLocationModel
    {
        public int compId { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int locId { get; set; }

        public EditLocationModel() { }
    }

    public class UpdateCarModel
    {
        public int carId { get; set; }
        public int companyId { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public double price { get; set; }
        public string type { get; set; }
        public int passen { get; set; }
        public string loc { get; set; }

        public UpdateCarModel()
        {

        }
    }

    public class NewCarModel 
    {
        public int companyId { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public double price { get; set; }
        public string type { get; set; }
        public int passen { get; set; }
        public string loc { get; set; }
        public string image { get; set; }

        public NewCarModel() { }
    }

    public class UpdateAmenityModel
    {
        public int companyId { get; set; }
        public int amenityId { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string image { get; set; }

        public UpdateAmenityModel() { }
    }

    public class AddAmenityModel
    {
        public int companyId { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string image { get; set; }
        public string payment { get; set; }

        public AddAmenityModel() { }
    }

    public class CarCompanyModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public List<string> admins { get; set; }

        public CarCompanyModel() { }
    }

    public class RegisterCompanyModel
    {
        public string companyName { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public RegisterCompanyModel() { }

    }

    public class RegisterAirCompanyModel
    {
        public string companyName { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public RegisterAirCompanyModel() { }

    }

    public class AddDiscountRangeModel
    {
        public int companyId { get; set; }
        public int carId { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public AddDiscountRangeModel() { }
    }

    public class HelpModel
    {
        public int id { get; set; }
        public  HelpModel() { }
    }

    public class PlusRentModel
    {
        public string location { get; set; }
        public string returnLocation { get; set; }
        public string from { get; set; }
        public string to { get; set; }

        public PlusRentModel() { }
    }

    public class ReturnAmenitiesModel
    {
        public List<ExtraAmenityModel> amenities { get; set; }
        public int compId { get; set; }
        ReturnAmenitiesModel() { }
    }

    public class ProfitModel
    {
        public string date1 { get; set; }
        public string date2 { get; set; }
        public int company { get; set; }

        ProfitModel() { }
    }

    public class AvailableCarsModel
    {
        public string from { get; set; }
        public string to { get; set; }
        public int company { get; set; }
    }

    public class RemoveLocationModel
    {
        public int id { get; set; }
        public int id2 { get; set; }
        public string newAddr { get; set; }
    }



}
