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
    public class CancelLinkedCarFailedHandler
    {
        AirlineContext _context;
        public CancelLinkedCarFailedHandler(AirlineContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<CarNotReservedHandler>();
        public async Task Handle(CancelLinkedCarFailed message, IMessageHandlerContext context)
        {
            log.Info($"Received CancelLinkedCarFailed, CombinedReservationId = ");

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


            foreach (var c in companies)
            {
                foreach (var f in c.Flights)
                {
                    foreach (var rez in f.AllTickets)
                    {
                        if (rez.Id == message.idRez)
                        {
                            rez.userId = message.userId;
                            rez.reqTick = message.ticket.ToString();
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            foreach (var u in companies)
            {
                foreach (var u2 in u.Flights)
                {
                    foreach (var u3 in u2.Seats)
                    {
                        if (message.seatId == u3.Id)
                        {
                            u3.Taken = true;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

        }
    }
}
