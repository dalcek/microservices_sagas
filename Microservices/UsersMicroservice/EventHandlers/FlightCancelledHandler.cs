using Messages.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class FlightCancelledHandler : IHandleMessages<FlightCancelled>
    {
        ApplicationUsersContext _context;
        UserManager<User> _userManager;
        public FlightCancelledHandler(ApplicationUsersContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        static ILog log = LogManager.GetLogger<FlightCancelledHandler>();
        public async Task Handle(FlightCancelled message, IMessageHandlerContext context)
        {
            log.Info($"Received FlightCancelled");

            // doda poene korisniku i ako uspije to je kraj, postavi onu rezervaciju na successfull

            var users = _context.Users.Include(us => us.CombinedReservations).ToList();

            string userId = message.userId;

            var user = await _userManager.FindByIdAsync(userId);

            if(user != null)
            {
                user.Points -= 5;

                await _userManager.UpdateAsync(user);

                _context.Update(user);
                _context.SaveChanges();
            }
            
        }
    }
}
