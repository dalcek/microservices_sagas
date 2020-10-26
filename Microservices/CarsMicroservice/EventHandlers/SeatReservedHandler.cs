using CarsMicroservice.Models.ContextData;
using CarsMicroservice.Models.Model;
using Messages.Events;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarsMicroservice.EventHandlers
{
    public class SeatReservedHandler : IHandleMessages<SeatReserved>
    {
        private RentACarContext _context;
        public SeatReservedHandler(RentACarContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<SeatReservedHandler>();
        public async Task Handle(SeatReserved message, IMessageHandlerContext context)
        {
            // Do something with the message here
            log.Info($"Received SeatReserved, CombinedReservationId = {message.combinedReservationId}");


            string userId = message.userId;
            string pointsStr = message.pointsS;
            int points = Int32.Parse(pointsStr);

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

            var company = _context.RentACarCompanies.Find(message.car.company);
            var car = _context.RentACarCompanies.Find(message.car.company).Cars.Find(car => car.ID == message.car.car);
            if (car.Removed)
            {
                CarNotReserved carNotReserved = new CarNotReserved();
                await context.Publish(carNotReserved).ConfigureAwait(false);
                return;
            }

            if (CheckAvailability(car, message.car.from, message.car.to))
            {
                CarReservation res = new CarReservation();
                res.PickUpLocation = message.car.pickUpAddr;
                res.ReturnLocation = message.car.dropOffAddr;
                res.PickUpTime = message.car.fromTime;
                res.ReturnTime = message.car.toTime;
                res.RatedCar = 0;
                res.RatedCompany = 0;
                res.Car = car;
                res.Dates = GetDates(message.car.from, message.car.to);
                res.Extras = new List<ExtraAmenity>();
                res.TimeStamp = DateTime.Now;
                res.ResUser = userId;
                // sacuvamo id karte da se moze otkazati i rez auta ako se otkaze let
                res.SeatReservationId = message.mainSeatId;
                foreach (var am in company.extras)
                {
                    if (message.car.extras.Contains(am.Id))
                        res.Extras.Add(am);
                }

                res.TotalPrice = res.Dates.Count * res.Car.PricePerDay;

                if (points > message.car.bronze && points < message.car.silver)
                    res.TotalPrice = res.TotalPrice - (res.TotalPrice * message.car.percent / 100);
                else if (points > message.car.silver && points < message.car.gold)
                    res.TotalPrice = res.TotalPrice - (res.TotalPrice * (message.car.percent * 2) / 100);
                else if (points > message.car.gold)
                    res.TotalPrice = res.TotalPrice - (res.TotalPrice * (message.car.percent * 3) / 100);

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
                    // auto vise nije dostupan, rollback
                    CarNotReserved carNotReserved = new CarNotReserved();
                    await context.Publish(carNotReserved).ConfigureAwait(false);
                    return;
                }
                catch (Exception e)
                {
                    // auto vise nije dostupan, rollback
                    CarNotReserved carNotReserved = new CarNotReserved();
                    await context.Publish(carNotReserved).ConfigureAwait(false);
                    return;


                }

                CarReserved carReserved = new CarReserved();
                carReserved.combinedReservationId = message.combinedReservationId;
                carReserved.pointsS = message.pointsS;
                carReserved.userId = message.userId;
                carReserved.tickets = message.tickets;
                carReserved.carRes = new Tuple<int, int>(company.ID, res.Id);
                carReserved.resId = message.resId;
                await context.Publish(carReserved).ConfigureAwait(false);
            }
            else
            {
                // auto vise nije dostupan, rollback
                CarNotReserved carNotReserved = new CarNotReserved();
                await context.Publish(carNotReserved).ConfigureAwait(false);
                return;
            }

            
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
    }
}
