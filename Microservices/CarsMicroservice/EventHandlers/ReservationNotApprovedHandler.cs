using CarsMicroservice.Models.ContextData;
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
    public class ReservationNotApprovedHandler : IHandleMessages<ReservationNotApproved>
    {
        private RentACarContext _context;
        public ReservationNotApprovedHandler(RentACarContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<ReservationNotApprovedHandler>();
        public async Task Handle(ReservationNotApproved message, IMessageHandlerContext context)
        {
            log.Info($"Received ReservationNotApproved, CombinedReservationId = ");

            //rezervacija nije uspijesno sacuvana, ponisti rezervaciju automobila

            string userId = message.userId;

            var companies = _context.RentACarCompanies
            .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
            .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
            .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
            .ToList();

            var company = _context.RentACarCompanies.Find(message.carRes.Item1);
            var reserv = company.Reservations.Find(res => res.Id == message.carRes.Item2);

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

            company.Reservations.Remove(reserv);
            _context.Update(company);
            await _context.SaveChangesAsync();

            _context.Remove(reserv);
            await _context.SaveChangesAsync();

            CarNotReserved carNotReserved = new CarNotReserved();
            carNotReserved.combinedReservationId = message.combinedReservationId;
            carNotReserved.resId = message.resId;
            carNotReserved.tickets = message.tickets;
            carNotReserved.userId = message.userId;
            await context.Publish(carNotReserved).ConfigureAwait(false);
        }
    }
}
