using AirlinesMicroservice.Models.ContextData;
using AirlinesMicroservice.Models.Model;
using Messages.Events;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AirlinesMicroservice.EventHandlers
{
    public class ReservationCreatedHandler : IHandleMessages<ReservationCreated>
    {
        AirlineContext _context;
        public ReservationCreatedHandler(AirlineContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<ReservationCreatedHandler>();
        public async Task Handle(ReservationCreated message, IMessageHandlerContext context)
        {
            log.Info($"Received ReservationCreated, CombinedReservationId = {message.combinedReservationId}");

            //napravi rezervaciju za sjedista

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

            string userid = message.userId;
            int points = Int32.Parse(message.pointsS);

            Discount dis = new Discount();
            dis.BronzeTier = message.tickets[0].b;
            dis.SilverTier = message.tickets[0].s;
            dis.DiscountPercent = message.tickets[0].d;
            dis.GoldTier = message.tickets[0].g;

            List<Tuple<int, int>> tickets = new List<Tuple<int, int>>();
            int mainSeatId = 0;

            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                foreach (var model in message.tickets)
                {
                    foreach (var u in companies)
                    {
                        foreach (var u2 in u.Flights)
                        {
                            foreach (var u3 in u2.Seats)
                            {
                                if (u3.Id == model.seat.Id)  // ako je to sediste iz rezervacije
                                {
                                    if(!u3.Taken)
                                    {
                                        u3.Traveller = new Traveller() { Email = model.seat.Traveller.Email, FirstName = model.seat.Traveller.FirstName, IdUser = model.seat.Traveller.IdUser, LastName = model.seat.Traveller.LastName, Passport = model.seat.Traveller.Passport };
                                        u3.Taken = true;

                                        try
                                        {
                                            _context.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            await dbContextTransaction.RollbackAsync();
                                            SeatNotReserved resFailed = new SeatNotReserved();
                                            resFailed.combinedReservationId = message.combinedReservationId;
                                            resFailed.userId = message.userId;
                                            resFailed.resId = message.resId;
                                            await context.Publish(resFailed).ConfigureAwait(false);
                                            return;
                                        }


                                        Ticket t = new Ticket() { Discount = 0, Flight = u2, Seat = u3 };

                                        if (userid == model.seat.Traveller.IdUser)
                                        {
                                            t.userId = userid;
                                            if (points > dis.BronzeTier && points < dis.SilverTier)
                                            {
                                                t.Discount = (Int32)(dis.DiscountPercent) + t.Discount;
                                            }

                                            else if (points > dis.SilverTier && points < dis.GoldTier)
                                                t.Discount = (Int32)(dis.DiscountPercent * 2) + t.Discount;
                                            else
                                                t.Discount = (Int32)(dis.DiscountPercent * 3) + t.Discount;

                                            if (u2.SoldTickets == null)
                                                u2.SoldTickets = new List<SoldTicket>();

                                            u2.SoldTickets.Add(new SoldTicket() { ticket = t });
                                            if (u2.AllTickets == null)
                                                u2.AllTickets = new List<Ticket>();
                                            u2.AllTickets.Add(t);

                                            try
                                            {
                                                _context.SaveChanges();
                                            }
                                            catch (Exception e)
                                            {
                                                await dbContextTransaction.RollbackAsync();
                                                SeatNotReserved resFailed = new SeatNotReserved();
                                                resFailed.combinedReservationId = message.combinedReservationId;
                                                resFailed.userId = message.userId;
                                                resFailed.resId = message.resId;
                                                await context.Publish(resFailed).ConfigureAwait(false);
                                                return;
                                            }
                                            mainSeatId = t.Id;
                                        }
                                        else
                                        {
                                            t.reqTick = model.seat.Traveller.IdUser;
                                            u2.AllTickets.Add(t);

                                            try
                                            {
                                                _context.SaveChanges();
                                            }
                                            catch (Exception e)
                                            {
                                                await dbContextTransaction.RollbackAsync();
                                                SeatNotReserved resFailed = new SeatNotReserved();
                                                resFailed.combinedReservationId = message.combinedReservationId;
                                                resFailed.userId = message.userId;
                                                resFailed.resId = message.resId;
                                                await context.Publish(resFailed).ConfigureAwait(false);
                                                return;
                                            }

                                        }
                                        tickets.Add(new Tuple<int, int>(u3.Id, t.Id));
                                        if (message.userId == model.seat.Traveller.IdUser)
                                            mainSeatId = t.Id;
                                    }
                                    else
                                    {
                                        await dbContextTransaction.RollbackAsync();
                                        SeatNotReserved resFailed = new SeatNotReserved();
                                        resFailed.combinedReservationId = message.combinedReservationId;
                                        resFailed.userId = message.userId;
                                        resFailed.resId = message.resId;
                                        await context.Publish(resFailed).ConfigureAwait(false);
                                        return;
                                    }
                                } 
                            }
                        }
                    }
                }

                await dbContextTransaction.CommitAsync();
            }
            
            SeatReserved seatReserved = new SeatReserved();
            seatReserved.combinedReservationId = message.combinedReservationId;
            seatReserved.car = message.car;
            seatReserved.userId = message.userId;
            seatReserved.pointsS = message.pointsS;
            seatReserved.tickets = tickets;
            seatReserved.resId = message.resId;
            seatReserved.mainSeatId = mainSeatId;
            await context.Publish(seatReserved).ConfigureAwait(false);
        }
    }
}
