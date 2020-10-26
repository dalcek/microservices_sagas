using AirlinesMicroservice.Models.ContextData;
using AirlinesMicroservice.Models.Model;
using Messages.Events;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlinesMicroservice.EventHandlers
{
    public class CarNotReservedHandler : IHandleMessages<CarNotReserved>
    {
        AirlineContext _context;
        public CarNotReservedHandler(AirlineContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<CarNotReservedHandler>();
        public async Task Handle(CarNotReserved message, IMessageHandlerContext context)
        {
            log.Info($"Received CarNotReserved, CombinedReservationId = ");

            //oslobodi sjedista

            foreach(var seatRes in message.tickets)
            {
                List<Airline> companies = _context.AirlineCompanies
                       .Include(comp => comp.FastTickets)
                       .Include(comp => comp.Destinations)
                       .Include(comp => comp.PopDestinaations)
                       .Include(comp => comp.Raters)
                       .Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                       .Include(fs => fs.Flights).ThenInclude(d => d.From)
                       .Include(fs => fs.Flights).ThenInclude(d => d.To)
                       .Include(fs => fs.Flights).ThenInclude(d => d.Stops)
                       .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                       .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                        .Include(fs => fs.Flights).ThenInclude(d => d.AllTickets)
                       .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                       .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                       .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                       .ToList();

                foreach (var u in companies)
                {
                    foreach (var u2 in u.Flights)
                    {
                        foreach (var u3 in u2.Seats)
                        {
                            if (seatRes.Item1 == u3.Id)
                            {
                                u3.Taken = false;
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                foreach (var c in companies)
                {
                    foreach (var f in c.Flights)
                    {
                        foreach (var rez in f.AllTickets)
                        {
                            if (rez.userId == message.userId)
                            {
                                if (rez.Id == seatRes.Item2)
                                {
                                    rez.userId = "-1";
                                    rez.reqTick = "-1";
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }

            }

            SeatNotReserved seatNotReserved = new SeatNotReserved();
            seatNotReserved.userId = message.userId;
            seatNotReserved.resId = message.resId;
            seatNotReserved.combinedReservationId = message.combinedReservationId;
            await context.Publish(seatNotReserved).ConfigureAwait(false);

        }
    }
}
