using CarsMicroservice.Models.ContextData;
using Messages.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarsMicroservice.EventHandlers
{
    public class FlightCancelledHandler : IHandleMessages<FlightCancelled>
    {
        RentACarContext _context;
        public FlightCancelledHandler(RentACarContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<FlightCancelledHandler>();
        public async Task Handle(FlightCancelled message, IMessageHandlerContext context)
        {
            log.Info($"Received FlightCancelled");

            // doda poene korisniku i ako uspije to je kraj, postavi onu rezervaciju na successfull

            var companies = _context.RentACarCompanies
           .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
           .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
           .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
           .ToList();

            foreach(var company in companies)
            {
                foreach(var res in company.Reservations)
                {
                    if(res.SeatReservationId == message.ticket)
                    {
                        foreach (var car in company.Cars)
                        {
                            if (car.ID == res.Car.ID)
                            {
                                foreach (var date in res.Dates)
                                {
                                    if (car.RentedDates.Contains(date))
                                        car.RentedDates.Remove(date);
                                }
                            }
                        }

                        using (var dbContextTransaction = _context.Database.BeginTransaction())
                        {
                            company.Reservations.Remove(res);
                            _context.Update(company);
                            try
                            {
                                _context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                dbContextTransaction.Rollback();
                                CancelLinkedCarFailed failedEvent = new CancelLinkedCarFailed();
                                failedEvent.userId = message.userId;
                                failedEvent.ticket = message.ticket;
                                failedEvent.idRez = message.idRez;
                                await context.Publish(failedEvent);
                                return;
                            }

                            _context.Remove(res);
                            try
                            {
                                _context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                dbContextTransaction.Rollback();
                                CancelLinkedCarFailed failedEvent = new CancelLinkedCarFailed();
                                failedEvent.userId = message.userId;
                                failedEvent.ticket = message.ticket;
                                failedEvent.idRez = message.idRez;
                                await context.Publish(failedEvent);
                                return;
                            }
                        }

                        CarReservationCancelled cancelled = new CarReservationCancelled();
                        cancelled.userId = message.userId;
                        await context.Publish(cancelled);

                        return;
                    }
                }
            }
        }
    }
}
