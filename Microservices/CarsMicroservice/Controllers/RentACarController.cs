using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarsMicroservice.Models.ContextData;
using CarsMicroservice.Models.Model;
using Messages;
using Messages.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NServiceBus;

namespace CarsMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentACarController : ControllerBase
    {
        private RentACarContext _context;
        private IMessageSession _messageSession;
        public IConfiguration Configuration { get; }

        public RentACarController(RentACarContext context, IConfiguration configuration, IMessageSession messageSession)
        {
            _context = context;
            _messageSession = messageSession;
            Configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllActivatedCompanies")]
        public IEnumerable<RentACarListingInfoModel> GetAllActivatedCompanies()
        {
            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<RentACarListingInfoModel> retCompanies = new List<RentACarListingInfoModel>();
            var comps = _context.RentACarCompanies.Include(comp => comp.MainLocation).ToList();

            foreach (var comp in comps)
            {
                if (comp.Activated)
                {
                    retCompanies.Add(new RentACarListingInfoModel() { Id = comp.ID, Img = comp.Image, MainLocation = comp.MainLocation.FullAddress, Name = comp.Name, Rating = comp.AvrageRating });
                }
            }
            return retCompanies;
        }

        [HttpGet]
        [Route("TestEndpoints")]
        public async void TestEndpoints()
        {
            DoSomething message = new DoSomething();
            message.SomeProperty = "nesto";
            await _messageSession.Send("airlinesEndpoint", message)
                .ConfigureAwait(false);
            //await _messageSession.SendLocal(message)
            //    .ConfigureAwait(false);
        }

        [HttpPost]
        [Route("SearchCompanies")]
        public IEnumerable<RentACarListingInfoModel> SearchCompanies(SearchCompaniesModel searchModel)
        {
            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<RentACarListingInfoModel> retCompanies = new List<RentACarListingInfoModel>();

            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.RentedDates)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.Ratings)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.QuickReservations)
                    .ThenInclude(qr => qr.Dates)
                .Include(comp => comp.extras).ToList();

            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Name.ToLower() == searchModel.companyName.ToLower() || searchModel.companyName == "" || companies[i].Name.ToLower().Contains(searchModel.companyName.ToLower()))
                {
                    if (searchModel.location == "")
                    {
                        bool available = false;
                        if (searchModel.from == "")
                            available = true;
                        else
                        {
                            foreach (var car in companies[i].Cars)
                            {
                                if (!car.Removed && CheckAvailability(car, searchModel.from, searchModel.to))
                                    available = true;
                            }
                        }
                        if (available)
                            retCompanies.Add(new RentACarListingInfoModel() { Id = companies[i].ID, Img = companies[i].Image, MainLocation = companies[i].MainLocation.FullAddress, Name = companies[i].Name, Rating = companies[i].AvrageRating });
                    }
                    else
                    {
                        int found = 0;
                        string address = "";
                        for (int j = 0; j < companies[i].Locations.Count; j++)
                        {
                            if (companies[i].Locations[j].FullAddress.ToLower().Contains(searchModel.location.ToLower()))
                            {
                                found = 1;
                                address = companies[i].Locations[j].FullAddress;
                                break;
                            }
                        }
                        if (found != 0)
                        {
                            bool available = false;
                            if (searchModel.from == "")
                                available = true;
                            else
                            {
                                foreach (var car in companies[i].Cars)
                                {
                                    if (!car.Removed && CheckAvailability(car, searchModel.from, searchModel.to))
                                        available = true;
                                }
                            }

                            if (available)
                                retCompanies.Add(new RentACarListingInfoModel() { Id = companies[i].ID, Img = companies[i].Image, MainLocation = address, Name = companies[i].Name, Rating = companies[i].AvrageRating });
                        }
                    }

                }
            }

            return retCompanies;
        }

        [HttpPost]
        [Route("GetCompanyProfile")]
        public RentACarProfileInfoModel GetCompanyProfile(IdModel model)
        {
            RentACarProfileInfoModel company = new RentACarProfileInfoModel();
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.extras).ToList();
            //int Id;
            //if (!Int32.TryParse(id, out Id))
            //    return null;

            foreach (var comp in companies)
            {
                if (comp.ID == model.id)
                {
                    company.Id = model.id;
                    company.Name = comp.Name;
                    company.Img = comp.Image;
                    company.Locations = comp.Locations;
                    company.MainLocation = comp.MainLocation;
                    company.Rating = comp.AvrageRating;
                    company.Description = comp.Description;
                    company.Extras = comp.extras;
                    return company;
                }

            }

            return null;
        }

        [HttpPost]
        [Route("SearchCars")]
        public List<CarModel> SearchCars(SearchCarModel searchModel)
        {
            List<CarModel> ret = new List<CarModel>();
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.RentedDates)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.Ratings)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.QuickReservations)
                    .ThenInclude(qr => qr.Dates)
                .Include(comp => comp.extras)
                .ToList();

            var comp = _context.RentACarCompanies.Find(searchModel.companyId);
            if (comp == null)
                return ret;

            foreach (var car in comp.Cars)
            {
                if (!car.Removed)
                {
                    if (car.Location.ToLower().Contains(searchModel.location.ToLower().Trim()) || searchModel.location == "")
                    {
                        if (CheckBranch(comp, searchModel.returnLocation) || searchModel.returnLocation == "")
                        {
                            if (CheckAvailability(car, searchModel.dateFrom, searchModel.dateTo))
                            {
                                if (CheckQuickReservations(comp, car, searchModel.dateFrom, searchModel.dateTo))
                                {
                                    if (car.Year.ToString() == searchModel.year || searchModel.year == "")
                                    {
                                        if (car.Type.ToLower() == searchModel.type.ToLower() || searchModel.type == "")
                                        {
                                            if (car.Brand.ToLower() == searchModel.brand.ToLower() || searchModel.brand == "")
                                            {
                                                if (car.Model.ToLower() == searchModel.model.ToLower() || searchModel.model == "")
                                                {
                                                    if (searchModel.price != "")
                                                    {
                                                        try
                                                        {
                                                            if (car.PricePerDay <= Int32.Parse(searchModel.price))
                                                                ret.Add(new CarModel() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = GetTotalPrice(car, searchModel.dateFrom, searchModel.dateTo), type = car.Type, year = car.Year });
                                                        }
                                                        catch (Exception e)
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {
                                                        ret.Add(new CarModel() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = GetTotalPrice(car, searchModel.dateFrom, searchModel.dateTo), type = car.Type, year = car.Year });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return ret;
        }

        [HttpPost]
        [Route("GetCarInfo")]
        public CarModel GetCarInfo(IdModelCarComp idModelCarComp)
        {
            CarModel car = new CarModel();
            var companies = _context.RentACarCompanies.Include(comp => comp.Cars).ToList();
            foreach (var comp in companies)
            {
                if (comp.ID == idModelCarComp.idComp)
                {
                    foreach (var carr in comp.Cars)
                    {
                        if (carr.ID == idModelCarComp.idCar)
                        {
                            car.avrageRating = carr.AvrageRating;
                            car.brand = carr.Brand;
                            car.id = carr.ID;
                            car.image = carr.Image;
                            car.location = carr.Location;
                            car.model = carr.Model;
                            car.passengers = carr.Passengers;
                            car.price = carr.PricePerDay;
                            car.type = carr.Type;
                            car.year = carr.Year;
                            break;
                        }
                    }
                }
            }

            return car;
        }

        [HttpPost]
        [Route("SearchCarsHome")]
        public List<CarModel> SearchCarsHome(SearchCarModelHome searchModel)
        {
            List<CarModel> ret = new List<CarModel>();
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.RentedDates)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.Ratings)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.QuickReservations)
                    .ThenInclude(qr => qr.Dates)
                .Include(comp => comp.extras)
                .ToList();

            foreach (var comp in companies)
            {
                foreach (var car in comp.Cars)
                {
                    if (!car.Removed)
                    {
                        if (car.Location.ToLower().Contains(searchModel.location.ToLower().Trim()) || searchModel.location == "")
                        {
                            if (CheckBranch(comp, searchModel.returnLocation) || searchModel.returnLocation == "")
                            {
                                if (CheckAvailability(car, searchModel.dateFrom, searchModel.dateTo))
                                {
                                    if (car.Type.ToLower() == searchModel.type.ToLower() || searchModel.type == "")
                                    {
                                        ret.Add(new CarModel() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = GetTotalPrice(car, searchModel.dateFrom, searchModel.dateTo), type = car.Type, year = car.Year, companyId = comp.ID });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Thread.Sleep(5000);
            return ret;
        }

        [HttpGet]
        [Route("GetCompanyAmenities")]
        public List<ExtraAmenityModel> GetCompanyAmenities(int id)
        {
            List<ExtraAmenityModel> ret = new List<ExtraAmenityModel>();

            var companies = _context.RentACarCompanies
                .Include(comp => comp.extras)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == id)
                {
                    foreach (var am in comp.extras)
                    {
                        ret.Add(new ExtraAmenityModel() { Id = am.Id, Image = am.Image, Name = am.Name, OneTimePayment = am.OneTimePayment, Price = am.Price, Selected = false });
                    }

                }
            }
            return ret;
        }

        [HttpGet]
        [Route("GetCompanyInfoAdmin")]
        public CompanyInfoModel GetCompanyInfoAdmin(int id)
        {
            CompanyInfoModel model = new CompanyInfoModel();

            var companies = _context.RentACarCompanies.ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == id)
                {
                    model.name = comp.Name;
                    model.description = comp.Description;
                    model.logo = comp.Image;
                    return model;
                }
            }
            return null;
        }

        [HttpPost]
        [Route("GetDiscountedCars")]
        public List<CarModel> GetDiscountedCars(PlusRentModel model)
        {
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar).ThenInclude(car => car.RentedDates)
                .ToList();

            List<DateTime> datumi = GetDateTimeList(model.from, model.to);
            List<CarModel> cars = new List<CarModel>();

            foreach (var comp in companies)
            {
                if (CheckBranch(comp, model.location) && CheckBranch(comp, model.returnLocation))
                {
                    foreach (var res in comp.QuickReservations)
                    {
                        if (res.DiscountedCar.Location.ToLower().Trim().Contains(model.location.Trim().ToLower()))
                        {
                            if (!res.DiscountedCar.Removed)
                            {
                                List<DateTime> datumiRes = GetDateTimeList(res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr);
                                bool okay = true;
                                foreach (var d in datumi)
                                {
                                    if (!datumiRes.Contains(d))
                                        okay = false;
                                }
                                if (okay)
                                {
                                    if (CheckAvailability(res.DiscountedCar, res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr))
                                    {
                                        cars.Add(new CarModel() { avrageRating = res.DiscountedCar.AvrageRating, brand = res.DiscountedCar.Brand, id = res.DiscountedCar.ID, image = res.DiscountedCar.Image, location = res.DiscountedCar.Location, model = res.DiscountedCar.Model, passengers = res.DiscountedCar.Passengers, price = res.DiscountedCar.PricePerDay * datumi.Count, type = res.DiscountedCar.Type, year = res.DiscountedCar.Year });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return cars;
        }

        [HttpGet]
        [Route("GetDiscountedCarsForCompany")]
        public List<CarModel> GetDiscountedCarsForCompany(int id)
        {
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar).ThenInclude(car => car.RentedDates)
                .ToList();

            List<CarModel> cars = new List<CarModel>();

            var company = _context.RentACarCompanies.Find(id);

            foreach (var qr in company.QuickReservations)
            {
                if (cars.Find(car => car.id == qr.DiscountedCar.ID) == null)
                {
                    cars.Add(new CarModel() { avrageRating = 0, brand = qr.DiscountedCar.Brand, id = qr.DiscountedCar.ID, image = qr.DiscountedCar.Image, location = qr.DiscountedCar.Location, model = qr.DiscountedCar.Model, passengers = qr.DiscountedCar.Passengers, price = qr.DiscountedCar.PricePerDay, type = qr.DiscountedCar.Type, year = qr.DiscountedCar.Year });
                }
            }
            return cars;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateCompanyInfo")]
        public async Task<object> UpdateCompanyInfo(UpdateCompanyInfoModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.compId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies.ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.compId)
                {
                    comp.Name = model.name;
                    comp.Description = model.description;
                    comp.Image = model.logo;
                    comp.Activated = true;
                    _context.Update(comp);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        return BadRequest(new { message = "Car has been changed. Please reload the page." });
                    }

                    return Ok();
                }
            }

            return BadRequest(new { message = "Company not found." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddCarCompanyLocation")]
        public async Task<object> AddCarCompanyLocation(NewLocationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.compId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.compId)
                {
                    Address newLoc = new Address();
                    newLoc.FullAddress = model.address;
                    newLoc.Latitude = model.latitude;
                    newLoc.Longitude = model.longitude;

                    if (comp.Locations == null)
                    {
                        comp.Locations = new List<Address>();
                    }
                    comp.Locations.Add(newLoc);
                    _context.Update(comp);
                    await _context.SaveChangesAsync();
                    return comp.Locations;
                }
            }
            return BadRequest(new { message = "Company not found." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("EditCarCompanyLocation")]
        public async Task<object> EditCarCompanyLocation(EditLocationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.compId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Reservations)
                .Include(comp => comp.Cars)
                .ToList();

            var company = await _context.RentACarCompanies.FindAsync(model.compId);

            if (company == null)
                return BadRequest(new { message = "Company not found." });

            Address addr = _context.RentACarCompanies.Find(model.compId).Locations.Find(loc => loc.Id == model.locId);
            if (addr == null)
                return BadRequest(new { message = "Address not found." });

            foreach (var res in company.Reservations)
            {
                if (res.PickUpLocation.Trim().ToLower().Contains(model.address.ToLower().Trim()) || res.ReturnLocation.Trim().ToLower().Contains(model.address.ToLower().Trim()))
                    return BadRequest(new { message = "There is a reservation made using this address, can not be edited" });
            }

            foreach (var car in company.Cars)
            {
                if (car.Location == addr.FullAddress)
                    car.Location = model.address;
            }

            addr.Latitude = model.latitude;
            addr.Longitude = model.longitude;
            addr.FullAddress = model.address;

            _context.Update(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Data has been modified. Please reload the page." });
            }

            return company.Locations;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveCarCompanyLocation")]
        public async Task<object> RemoveCarCompanyLocation(RemoveLocationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.id2)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Reservations)
                .Include(comp => comp.Cars)
                .ToList();

            var company = await _context.RentACarCompanies.FindAsync(model.id2);

            if (company == null)
                return BadRequest(new { message = "Company not found." });

            Address addr = _context.RentACarCompanies.Find(model.id2).Locations.Find(loc => loc.Id == model.id);
            if (addr == null)
                return BadRequest(new { message = "Address not found." });

            foreach (var res in company.Reservations)
            {
                if (res.PickUpLocation.Trim().ToLower().Contains(addr.FullAddress.ToLower().Trim()) || res.ReturnLocation.Trim().ToLower().Contains(addr.FullAddress.ToLower().Trim()))
                    return BadRequest(new { message = "Reservation was made using this address, can not remove." });
            }

            foreach (var car in company.Cars)
            {
                if (car.Location == addr.FullAddress)
                    car.Location = model.newAddr;
            }

            company.Locations.Remove(addr);

            _context.Update(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Data has been modified. Please reload the page." });
            }

            return company.Locations;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateCar")]
        public async Task<object> UpdateCar(UpdateCarModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.companyId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies
                   .Include(comp => comp.MainLocation)
                   .Include(comp => comp.Locations)
                   .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                   .ToList();
            var comp = _context.RentACarCompanies.Find(model.companyId);
            if (comp == null)
                return BadRequest(new { message = "Company not found." });
            var carr = _context.RentACarCompanies.Find(model.companyId).Cars.Find(car => car.ID == model.carId);
            if (carr == null || carr.Removed)
                return BadRequest(new { message = "Car is already removed." });

            foreach (var d in carr.RentedDates)
            {
                DateTime pom = DateTime.Parse(d.DateStr);
                if (pom > DateTime.Now)
                    return BadRequest(new { message = "Not able to edit car that is rented." });
            }

            carr.Brand = model.brand;
            carr.Location = model.loc;
            carr.Model = model.model;
            carr.Passengers = model.passen;
            carr.PricePerDay = model.price;
            carr.Type = model.type;
            carr.Year = model.year;

            _context.Update(carr);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Car has been modified, please reload the page." });
            }

            List<CarModelAdmin> cars = new List<CarModelAdmin>();
            foreach (var car in comp.Cars)
            {
                if (!car.Removed)
                {
                    List<string> rented = new List<string>();
                    foreach (var date in car.RentedDates)
                    {
                        rented.Add(date.DateStr);
                    }
                    cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented });
                }

            }

            return cars;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveCar")]
        public async Task<object> RemoveCar(HelpModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }


            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .ToList();

            var comp = _context.RentACarCompanies.Find(model.id);
            if (comp == null)
                return BadRequest(new { message = "Company not found." });
            var carr = _context.RentACarCompanies.Find(model.id).Cars.Find(car => car.ID == model.id);
            if (carr == null)
                return BadRequest(new { message = "Car not found." });
            if (!carr.Removed)
            {
                foreach (var d in carr.RentedDates)
                {
                    DateTime pom = DateTime.Parse(d.DateStr);
                    if (pom > DateTime.Now)
                        return BadRequest(new { message = "Not able to remove car that is rented." });
                }
                carr.Removed = true;
                _context.Update(carr);
                await _context.SaveChangesAsync();
            }


            List<CarModelAdmin> cars = new List<CarModelAdmin>();
            foreach (var car in comp.Cars)
            {
                if (!car.Removed)
                {
                    List<string> rented = new List<string>();
                    foreach (var date in car.RentedDates)
                    {
                        rented.Add(date.DateStr);
                    }
                    cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented });
                }

            }

            return cars;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewCar")]
        public async Task<object> AddNewCar(NewCarModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.companyId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.companyId)
                {
                    Car newCar = new Car();
                    newCar.AvrageRating = 0;
                    newCar.Brand = model.brand;
                    newCar.CompanyId = model.companyId;
                    newCar.Image = model.image;
                    newCar.Location = model.loc;
                    newCar.Model = model.model;
                    newCar.Passengers = model.passen;
                    newCar.PricePerDay = model.price;
                    newCar.Ratings = new List<Rating>();
                    newCar.RentedDates = new List<Date>();
                    newCar.Type = model.type;
                    newCar.Year = model.year;

                    comp.Cars.Add(newCar);
                    _context.Update(comp);
                    await _context.SaveChangesAsync();

                    List<CarModelAdmin> cars = new List<CarModelAdmin>();
                    foreach (var car in comp.Cars)
                    {
                        if (!car.Removed)
                        {
                            List<string> rented = new List<string>();
                            foreach (var date in car.RentedDates)
                            {
                                rented.Add(date.DateStr);
                            }
                            cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented });
                        }

                    }

                    return cars;


                }
            }
            return BadRequest(new { message = "Company not found." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateAmenity")]
        public async Task<object> UpdateAmenity(UpdateAmenityModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.companyId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies
                .Include(comp => comp.extras)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Extras)
                .ToList();

            var company = await _context.RentACarCompanies.FindAsync(model.companyId);
            var am = _context.RentACarCompanies.Find(model.companyId).extras.Find(am => am.Id == model.amenityId);

            foreach (var res in company.Reservations)
            {
                if (res.Extras.Find(ame => ame.Id == am.Id) != null)
                {
                    return BadRequest(new { message = "Amenity is in reservation" });
                }
            }

            am.Name = model.name;
            am.Price = model.price;
            am.Image = model.image;
            _context.Update(am);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Data had already been modified, please reload the page." });
            }
            return company.extras;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveAmenity")]
        public async Task<object> RemoveAmenity(IdModel3 model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.id2)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies
                .Include(comp => comp.extras)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Extras)
                .ToList();

            var company = await _context.RentACarCompanies.FindAsync(model.id2);
            var am = _context.RentACarCompanies.Find(model.id2).extras.Find(am => am.Id == model.id);

            foreach (var res in company.Reservations)
            {
                if (res.Extras.Find(ame => ame.Id == am.Id) != null)
                {
                    return BadRequest(new { message = "Amenity is in reservation" });
                }
            }

            company.extras.Remove(am);
            _context.Update(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Data had already been modified, please reload the page." });
            }
            return company.extras;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddAmenity")]
        public async Task<object> AddAmenity(AddAmenityModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }
            //var user = await _userManager.FindByIdAsync(userId);

            //if (user.CompanyId != model.companyId)
            //{
            //    return BadRequest(new { message = "Not your company." });
            //}

            var companies = _context.RentACarCompanies
                .Include(comp => comp.extras)
                .ToList();

            foreach (var comp in companies)
            {
                if (model.companyId == comp.ID)
                {
                    ExtraAmenity amenity = new ExtraAmenity();
                    amenity.Image = model.image;
                    amenity.Name = model.name;
                    amenity.Price = model.price;
                    if (model.payment == "One Time Payment")
                    {
                        amenity.OneTimePayment = true;
                    }
                    else
                    {
                        amenity.OneTimePayment = false;
                    }
                    comp.extras.Add(amenity);
                    _context.Update(comp);
                    await _context.SaveChangesAsync();
                    return comp.extras;
                }
            }

            return BadRequest(new { message = "Something went wrong, please reload and try again." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("SaveNewDiscountRange")]
        public async Task<object> SaveNewDiscountRange(AddDiscountRangeModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.companyId)
                {
                    foreach (var carr in comp.Cars)
                    {
                        if (carr.ID == model.carId)
                        {
                            if (!CheckAvailability(carr, model.dateFrom, model.dateTo))
                            {
                                return BadRequest(new { message = "Car is rented for specified date range." });
                            }
                            else
                            {
                                List<Date> dates = GetDates(model.dateFrom, model.dateTo);
                                comp.QuickReservations.Add(new QuickReservation() { Dates = dates, DiscountedCar = carr });
                                _context.Update(comp);
                                await _context.SaveChangesAsync();

                                List<CarModelAdmin> cars = new List<CarModelAdmin>();
                                foreach (var car in comp.Cars)
                                {
                                    if (!car.Removed)
                                    {
                                        List<string> rented = new List<string>();
                                        foreach (var date in car.RentedDates)
                                        {
                                            rented.Add(date.DateStr);
                                        }
                                        List<QuickReservationModel> discount = new List<QuickReservationModel>();
                                        foreach (var res in comp.QuickReservations)
                                        {
                                            if (res.DiscountedCar.ID == car.ID)
                                            {
                                                discount.Add(new QuickReservationModel() { id = res.Id, carId = res.DiscountedCar.ID, from = res.Dates[0].DateStr, to = res.Dates[res.Dates.Count - 1].DateStr });
                                            }
                                        }
                                        cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented, quickReservations = discount });
                                    }

                                }

                                return cars;
                            }
                        }
                    }
                }
            }
            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveDiscountRange")]
        public async Task<object> RemoveDiscountRange(HelpModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .ToList();

            foreach (var comp in companies)
            {
                foreach (var res in comp.QuickReservations)
                {
                    if (res.Id == model.id)
                    {
                        foreach (var carr in comp.Cars)
                        {
                            if (res.DiscountedCar.ID == carr.ID)
                            {
                                if (CheckAvailability(carr, res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr))
                                {
                                    comp.QuickReservations.Remove(res);
                                    _context.Update(comp);
                                    try
                                    {
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (DbUpdateConcurrencyException ex)
                                    {
                                        return BadRequest(new { message = "Car has been modified, please reload the page." });
                                    }

                                    List<CarModelAdmin> cars = new List<CarModelAdmin>();
                                    foreach (var car in comp.Cars)
                                    {
                                        if (!car.Removed)
                                        {
                                            List<string> rented = new List<string>();
                                            foreach (var date in car.RentedDates)
                                            {
                                                rented.Add(date.DateStr);
                                            }
                                            List<QuickReservationModel> discount = new List<QuickReservationModel>();
                                            foreach (var ress in comp.QuickReservations)
                                            {
                                                if (ress.DiscountedCar.ID == car.ID)
                                                {
                                                    discount.Add(new QuickReservationModel() { id = ress.Id, carId = ress.DiscountedCar.ID, from = ress.Dates[0].DateStr, to = ress.Dates[ress.Dates.Count - 1].DateStr });
                                                }
                                            }
                                            cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented, quickReservations = discount });
                                        }

                                    }

                                    return cars;
                                }
                                else
                                {
                                    return BadRequest(new { message = "Car is rented during specified time, not able to delete now." });
                                }
                            }
                        }
                    }
                }
            }

            return BadRequest(new { message = "Something went wrong, please try again later." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetProfit")]
        public async Task<object> GetProfit(ProfitModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Reservations).ToList();

            var comp = await _context.RentACarCompanies.FindAsync(model.company);

            double total = 0;
            foreach (var res in comp.Reservations)
            {
                if (res.TimeStamp > DateTime.Parse(model.date1) && res.TimeStamp < DateTime.Parse(model.date2))
                {
                    total += res.TotalPrice;
                }
            }
            return total;
        }

        [HttpPost]
        [Route("AvailableCarsAdmin")]
        public async Task<object> AvailableCarsAdmin(AvailableCarsModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<int> ret = new List<int>();

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.RentedDates)
                .ToList();

            var company = await _context.RentACarCompanies.FindAsync(model.company);

            foreach (var car in company.Cars)
            {
                if (CheckAvailability(car, model.from, model.to))
                {
                    ret.Add(car.ID);
                }
            }

            return ret;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RateCarCompany")]
        public async Task<object> RateCarCompany(RateCarCompanyModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.compId)
                {
                    if (comp.Ratings == null)
                        comp.Ratings = new List<Rating>();
                    comp.Ratings.Add(new Rating() { Rate = model.star });
                }
                int sum = 0;
                foreach (var r in comp.Ratings)
                {
                    sum += r.Rate;
                }
                comp.AvrageRating = sum / comp.Ratings.Count;
                foreach (var res in comp.Reservations)
                {
                    if (res.Id == model.resId)
                    {
                        res.RatedCompany = model.star;
                    }
                }
                _context.Update(comp);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RateCar")]
        public async Task<object> RateCar(RateCarModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.Ratings)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
                .ToList();

            var comp = _context.RentACarCompanies.Find(model.compId);
            var car = _context.RentACarCompanies.Find(model.compId).Cars.Find(car => car.ID == model.carId);

            if (car.Ratings == null)
                car.Ratings = new List<Rating>();
            car.Ratings.Add(new Rating() { Rate = model.star });
            int sum = 0;
            foreach (var r in car.Ratings)
            {
                sum += r.Rate;
            }
            car.AvrageRating = sum / car.Ratings.Count;

            foreach (var res in comp.Reservations)
            {
                if (res.Id == model.resId)
                {
                    res.RatedCar = model.star;
                }
            }
            _context.Update(comp);
            await _context.SaveChangesAsync();
            return Ok();

        }

        //drugi dio metode za uitavanje admina
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadAdminCompany")]
        public async Task<object> LoadAdminCompany(int id)
        {
            CompanyInfoModel model = new CompanyInfoModel();

            var companies = _context.RentACarCompanies
                .Include(comp => comp.Locations)
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.extras)
                .Include(comp => comp.QuickReservations).ThenInclude(qr => qr.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(qr => qr.Dates)
                .Include(comp => comp.Reservations)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == id)
                {
                    model.name = comp.Name;
                    model.description = comp.Description;
                    model.logo = comp.Image;
                    model.locations = new LocationsModel();
                    model.locations.mainLocation = comp.MainLocation;
                    model.locations.locations = new List<Address>();
                    model.activated = comp.Activated;
                    foreach (var loc in comp.Locations)
                    {
                        model.locations.locations.Add(loc);
                    }
                    model.ratings = new List<int>();
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    foreach (var res in comp.Reservations)
                    {
                        if (res.RatedCompany != 0)
                        {
                            model.ratings[res.RatedCompany - 1]++;
                        }
                    }

                    model.cars = new List<CarModelAdmin>();
                    foreach (var car in comp.Cars)
                    {
                        if (!car.Removed)
                        {
                            List<string> rented = new List<string>();
                            foreach (var date in car.RentedDates)
                            {
                                rented.Add(date.DateStr);
                            }
                            List<QuickReservationModel> discount = new List<QuickReservationModel>();
                            foreach (var res in comp.QuickReservations)
                            {
                                if (res.DiscountedCar.ID == car.ID)
                                {
                                    discount.Add(new QuickReservationModel() { id = res.Id, carId = res.DiscountedCar.ID, from = res.Dates[0].DateStr, to = res.Dates[res.Dates.Count - 1].DateStr });
                                }
                            }
                            model.cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented, quickReservations = discount });

                        }
                    }

                    List<ExtraAmenity> extras = new List<ExtraAmenity>();
                    extras = comp.extras;
                    model.extras = extras;

                    //find statistical data
                    //daily
                    List<int> dailyRes = new List<int>();
                    List<string> dailyLabels = new List<string>();
                    DateTime refDate = DateTime.Now;
                    for (int i = 0; i < 7; i++)
                    {
                        int count = 0;
                        foreach (var res in comp.Reservations)
                        {
                            if (res.TimeStamp.Day == refDate.Day && res.TimeStamp.Month == refDate.Month && res.TimeStamp.Year == refDate.Year)
                                count++;
                        }
                        dailyRes.Add(count);
                        dailyLabels.Add(refDate.ToShortDateString().ToString());
                        refDate = refDate.AddDays(-1);
                    }

                    List<int> weeklyRes = new List<int>();
                    List<string> weeklyLabels = new List<string>();
                    refDate = DateTime.Now;
                    DateTime startWeek = StartOfWeek(refDate, DayOfWeek.Monday);
                    //DayOfWeek day = refDate.DayOfWeek;
                    //DateTime tillDate = DateTime.Now;
                    for (int i = 0; i < 7; i++)
                    {
                        int count = 0;
                        foreach (var res in comp.Reservations)
                        {
                            if (StartOfWeek(res.TimeStamp, DayOfWeek.Monday) == startWeek)
                                count++;
                        }
                        weeklyRes.Add(count);
                        weeklyLabels.Add(startWeek.ToShortDateString().ToString());
                        startWeek = startWeek.AddDays(-7);
                    }

                    List<int> monthlyRes = new List<int>();
                    List<string> monthlyLabels = new List<string>();
                    refDate = DateTime.Now;
                    for (int i = 0; i < 7; i++)
                    {
                        int count = 0;
                        foreach (var res in comp.Reservations)
                        {
                            if (res.TimeStamp.Month == refDate.Month && res.TimeStamp.Year == refDate.Year)
                                count++;
                        }
                        monthlyRes.Add(count);
                        monthlyLabels.Add(GetMonthName(refDate.Month));
                        refDate = refDate.AddMonths(-1);
                    }
                    model.dailyReservations = dailyRes.Reverse<int>().ToList();
                    model.weeklyReservations = weeklyRes.Reverse<int>().ToList();
                    model.monthlyReservations = monthlyRes.Reverse<int>().ToList();
                    model.dailyLabels = dailyLabels.Reverse<string>().ToList();
                    model.weeklyLabels = weeklyLabels.Reverse<string>().ToList();
                    model.monthlyLabels = monthlyLabels.Reverse<string>().ToList();

                    return model;
                }
            }
            return null;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RegisterCarCompany")]
        public async Task<object> RegisterCarCompany(RegisterCompanyModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            Address addr1 = new Address() { FullAddress = model.address, Latitude = model.latitude, Longitude = model.longitude };
            RentACar company1 = new RentACar();
            company1.Activated = false;
            company1.AvrageRating = 0;
            company1.Cars = new List<Car>();
            company1.Description = "";
            company1.extras = new List<ExtraAmenity>();
            company1.Image = "";
            company1.Locations = new List<Address>();
            company1.MainLocation = addr1;
            company1.Name = model.companyName;
            company1.QuickReservations = new List<QuickReservation>();
            company1.Ratings = new List<Rating>();
            company1.extras = new List<ExtraAmenity>();

            _context.RentACarCompanies.Add(company1);
            await _context.SaveChangesAsync();

            List<CarCompanyModel> carCompanies = new List<CarCompanyModel>();

            var companies = _context.RentACarCompanies.ToList();

            foreach (var comp in companies)
            {
                CarCompanyModel com = new CarCompanyModel();
                com.name = comp.Name;
                com.image = comp.Image;
                com.id = comp.ID;
                com.admins = new List<string>();
                carCompanies.Add(com);
            }
            return carCompanies;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RentCar")]
        public async Task<object> RentCar(ReservationModel reservation)
        {
            var options = new DbContextOptionsBuilder<RentACarContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            string pointsStr = User.Claims.First(c => c.Type == "Points").Value;
            int points = Int32.Parse(pointsStr);

            if (userRole != "RegisteredUser")
            {
                return BadRequest(new { message = "Forbidden action for this role." });
            }


            _context.RentACarCompanies
                        .Include(comp => comp.MainLocation)
                        .Include(comp => comp.Locations)
                        .Include(comp => comp.Cars)
                            .ThenInclude(car => car.RentedDates)
                        .Include(comp => comp.Cars)
                            .ThenInclude(car => car.Ratings)
                        .Include(comp => comp.Ratings)
                        .Include(comp => comp.QuickReservations)
                            .ThenInclude(qr => qr.Dates)
                        .Include(comp => comp.extras)
                        .ToList();

            var company = _context.RentACarCompanies.Find(reservation.company);
            var car = _context.RentACarCompanies.Find(reservation.company).Cars.Find(car => car.ID == reservation.car);
            if (car.Removed)
                return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle." });

            if (CheckAvailability(car, reservation.from, reservation.to))
            {
                if (reservation.quickRes == 0)
                {
                    if (!CheckQuickReservations(company, car, reservation.from, reservation.to))
                        return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle." });
                }
                CarReservation res = new CarReservation();
                res.PickUpLocation = reservation.pickUpAddr;
                res.ReturnLocation = reservation.dropOffAddr;
                res.PickUpTime = reservation.fromTime;
                res.ReturnTime = reservation.toTime;
                res.RatedCar = 0;
                res.RatedCompany = 0;
                res.Car = car;
                res.Dates = GetDates(reservation.from, reservation.to);
                res.Extras = new List<ExtraAmenity>();
                res.TimeStamp = DateTime.Now;
                res.ResUser = userId;
                foreach (var am in company.extras)
                {
                    if (reservation.extras.Contains(am.Id))
                        res.Extras.Add(am);
                }

                //formiranje cijene

                res.TotalPrice = res.Dates.Count * res.Car.PricePerDay;

                if (points > reservation.bronze && points < reservation.silver)
                    res.TotalPrice = res.TotalPrice - (res.TotalPrice * reservation.percent / 100);
                else if (points > reservation.silver && points < reservation.gold)
                    res.TotalPrice = res.TotalPrice - (res.TotalPrice * (reservation.percent * 2) / 100);
                else if (points > reservation.gold)
                    res.TotalPrice = res.TotalPrice - (res.TotalPrice * (reservation.percent * 3) / 100);

                foreach (var ex in res.Extras)
                {
                    if (ex.OneTimePayment)
                        res.TotalPrice += ex.Price;
                    else
                        res.TotalPrice += ex.Price * res.Dates.Count;
                }


                if (company.Reservations == null)
                {
                    company.Reservations = new List<CarReservation>();
                }
                company.Reservations.Add(res);

                //user.Points = user.Points + 5;

                if (car.RentedDates == null)
                {
                    car.RentedDates = new List<Date>();
                }
                foreach (var d in res.Dates)
                {
                    car.RentedDates.Add(d);
                }

                _context.Update(company);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict!" });
                }
                catch (Exception e)
                {
                    return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict 2!" });
                }
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle." });
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GiveUpCarReservation")]
        public async Task<object> GiveUpCarReservation(RemoveReservationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var companies = _context.RentACarCompanies
            .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
            .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
            .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
            .ToList();

            var company = _context.RentACarCompanies.Find(model.compId);
            var reserv = company.Reservations.Find(res => res.Id == model.resId);

            foreach (var car in company.Cars)
            {
                if (car.ID == reserv.Car.ID)
                {
                    foreach (var date in reserv.Dates)
                    {
                        if (car.RentedDates.Contains(date))
                            car.RentedDates.Remove(date);
                    }
                }
            }

            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                company.Reservations.Remove(reserv);
                _context.Update(company);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    dbContextTransaction.Rollback();
                    return BadRequest(new { message = "Reservation could not be removed" });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return BadRequest(new { message = ex.Message });
                }

                _context.Remove(reserv);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    dbContextTransaction.Rollback();
                    return BadRequest(new { message = "Reservation could not be removed" });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return BadRequest(new { message = ex.Message });
                }

                await dbContextTransaction.CommitAsync();

                CarReservationCancelled cancelled = new CarReservationCancelled();
                cancelled.userId = userId;
                await _messageSession.Publish(cancelled);
            }

            List<CarReservationModel> retList = new List<CarReservationModel>();
            foreach (var comp in companies)
            {
                foreach (var res in comp.Reservations)
                {
                    if (res.ResUser == userId)
                    {
                        string companyName = comp.Name;
                        int companyId = comp.ID;
                        retList.Add(new CarReservationModel()
                        {
                            car = new CarModel()
                            {
                                avrageRating = res.Car.AvrageRating,
                                brand = res.Car.Brand,
                                id = res.Car.ID,
                                image = res.Car.Image,
                                location = res.Car.Location,
                                model = res.Car.Model,
                                passengers = res.Car.Passengers,
                                price = res.Car.PricePerDay,
                                type = res.Car.Type,
                                year = res.Car.Year
                            },
                            company = companyName,
                            companyId = companyId,
                            dateFrom = res.Dates[0].DateStr,
                            dateTo = res.Dates[res.Dates.Count - 1].DateStr,
                            dropOffLocation = res.ReturnLocation,
                            dropOffTime = res.ReturnTime,
                            id = res.Id,
                            pickUpLocation = res.PickUpLocation,
                            pickUpTime = res.PickUpTime,
                            ratedCar = res.RatedCar,
                            ratedCompany = res.RatedCompany,
                            timeStamp = res.TimeStamp.ToShortDateString(),
                            total = res.Dates.Count * res.Car.PricePerDay,
                            Extras = res.Extras
                        });
                    }
                }

            }
            return retList;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadMyReservations")]
        public async Task<object> LoadMyReservations()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var companies = _context.RentACarCompanies
                    .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                    .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
                    .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
                    .ToList();


            List<CarReservationModel> retList = new List<CarReservationModel>();
            foreach (var comp in companies)
            {
                foreach (var res in comp.Reservations)
                {
                    if (res.ResUser == userId)
                    {
                        string companyName = comp.Name;
                        int companyId = comp.ID;
                        retList.Add(new CarReservationModel()
                        {
                            car = new CarModel()
                            {
                                avrageRating = res.Car.AvrageRating,
                                brand = res.Car.Brand,
                                id = res.Car.ID,
                                image = res.Car.Image,
                                location = res.Car.Location,
                                model = res.Car.Model,
                                passengers = res.Car.Passengers,
                                price = res.Car.PricePerDay,
                                type = res.Car.Type,
                                year = res.Car.Year
                            },
                            company = companyName,
                            companyId = companyId,
                            dateFrom = res.Dates[0].DateStr,
                            dateTo = res.Dates[res.Dates.Count - 1].DateStr,
                            dropOffLocation = res.ReturnLocation,
                            dropOffTime = res.ReturnTime,
                            id = res.Id,
                            pickUpLocation = res.PickUpLocation,
                            pickUpTime = res.PickUpTime,
                            ratedCar = res.RatedCar,
                            ratedCompany = res.RatedCompany,
                            timeStamp = res.TimeStamp.ToShortDateString(),
                            total = res.Dates.Count * res.Car.PricePerDay,
                            Extras = res.Extras
                        });
                    }
                }

            }
            return retList;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadWebsiteAdminCompanies")]
        public async Task<object> LoadWebsiteAdminCompanies()
        {
            var companies = _context.RentACarCompanies.ToList();

            List<CarCompanyModel> carCompanies = new List<CarCompanyModel>();

            foreach (var comp in companies)
            {
                CarCompanyModel com = new CarCompanyModel();
                com.name = comp.Name;
                com.image = comp.Image;
                com.id = comp.ID;
                com.admins = new List<string>();
                carCompanies.Add(com);
            }

            return carCompanies;

        }


        private List<Date> GetDates(string from, string to)
        {
            List<Date> dates = new List<Date>();

            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            dates.Add(new Date() { DateStr = fromDate.ToShortDateString() });

            while (fromDate < toDate)
            {
                DateTime d = fromDate.AddDays(1);
                dates.Add(new Date() { DateStr = d.ToShortDateString() });
                fromDate = fromDate.AddDays(1);
            }

            return dates;
        }

        private bool CheckAvailability(Car car, string from, string to)
        {
            bool available = true;
            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            foreach (var date in car.RentedDates)
            {
                DateTime dt = DateTime.Parse(date.DateStr);
                if (dt >= fromDate && dt <= toDate)
                {
                    available = false;
                    break;
                }
            }

            return available;
        }

        private bool CheckBranch(RentACar company, string location)
        {

            if (company.MainLocation.FullAddress.ToLower().Trim().Contains(location.Trim().ToLower()))
                return true;
            bool exists = false;
            foreach (var loc in company.Locations)
            {
                if (loc.FullAddress.ToLower().Trim().Contains(location.Trim().ToLower()))
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        private double GetTotalPrice(Car car, string from, string to)
        {
            DateTime dtfrom = DateTime.Parse(from);
            DateTime dtTo = DateTime.Parse(to);
            double days = (dtTo - dtfrom).TotalDays;
            return car.PricePerDay * days;

        }

        private bool CheckQuickReservations(RentACar comp, Car car, string from, string to)
        {
            bool available = true;
            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            List<DateTime> dates = GetDateTimeList(from, to);

            foreach (var res in comp.QuickReservations)
            {
                if (res.DiscountedCar.ID == car.ID)
                {
                    foreach (var d in dates)
                    {
                        List<DateTime> pom = GetDateTimeList(res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr);
                        if (pom.Contains(d))
                        {
                            return false;
                        }
                    }
                }
            }

            return available;
        }

        private List<DateTime> GetDateTimeList(string from, string to)
        {
            List<DateTime> dates = new List<DateTime>();

            DateTime fromDate1 = DateTime.Parse(from);
            string datefrom = fromDate1.ToShortDateString();
            DateTime fromDate = DateTime.Parse(datefrom);

            DateTime toDate1 = DateTime.Parse(to);
            string dateto = toDate1.ToShortDateString();
            DateTime toDate = DateTime.Parse(dateto);

            dates.Add(fromDate);

            while (fromDate < toDate)
            {
                DateTime d = fromDate.AddDays(1);
                dates.Add(d);
                fromDate = fromDate.AddDays(1);
            }

            return dates;
        }

        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private string GetMonthName(int m)
        {
            if (m == 1)
                return "January";
            else if (m == 2)
                return "February";
            else if (m == 3)
                return "March";
            else if (m == 4)
                return "April";
            else if (m == 5)
                return "May";
            else if (m == 6)
                return "June";
            else if (m == 7)
                return "July";
            else if (m == 8)
                return "August";
            else if (m == 9)
                return "September";
            else if (m == 10)
                return "October";
            else if (m == 11)
                return "November";
            else
                return "Decebmer";
        }
    }
}