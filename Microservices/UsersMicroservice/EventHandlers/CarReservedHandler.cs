using Messages.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersMicroservice.Models.ContextData;
using UsersMicroservice.Models.Model;

namespace UsersMicroservice.EventHandlers
{
    public class CarReservedHandler : IHandleMessages<CarReserved>
    {
        ApplicationUsersContext _context;
        private UserManager<User> _userManager;
        public CarReservedHandler(ApplicationUsersContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        static ILog log = LogManager.GetLogger<CarReservedHandler>();
        public async Task Handle(CarReserved message, IMessageHandlerContext context)
        {
            log.Info($"Received CarReserved, CombinedReservationId = {message.combinedReservationId}");

            // doda poene korisniku i ako uspije to je kraj, postavi onu rezervaciju na successfull

            var users = _context.Users.Include(us => us.CombinedReservations).ToList();

            string userId = message.userId;

            var user = await _userManager.FindByIdAsync(userId);
            user.Points += 10;

            foreach (var res in user.CombinedReservations)
            {
                if (res.Id == message.resId)
                {
                    res.Status = "Approved";
                }
            }

            await _userManager.UpdateAsync(user);

            _context.Update(user);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                foreach (var res in user.CombinedReservations)
                {
                    if (res.Id == message.resId)
                    {
                        res.Status = "Denied";
                        await _userManager.UpdateAsync(user);

                        _context.Update(user);
                        _context.SaveChanges();
                    }
                }

                ReservationNotApproved notApproved = new ReservationNotApproved();
                notApproved.combinedReservationId = message.combinedReservationId;
                notApproved.carRes = message.carRes;
                notApproved.resId = message.resId;
                notApproved.tickets = message.tickets;
                notApproved.userId = message.userId;
                await context.Publish(notApproved).ConfigureAwait(false);
            }

        }
    }
}
