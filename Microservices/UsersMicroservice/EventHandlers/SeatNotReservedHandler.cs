using Messages.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
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
    public class SeatNotReservedHandler : IHandleMessages<SeatNotReserved>
    {
        ApplicationUsersContext _context;
        UserManager<User> _userManager;
        public SeatNotReservedHandler(ApplicationUsersContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        static ILog log = LogManager.GetLogger<SeatNotReservedHandler>();
        public async Task Handle(SeatNotReserved message, IMessageHandlerContext context)
        {
            log.Info($"Received SeatNotReserved, CombinedReservationId = ");

            // postavi na unsuccessfull, mora da se odradi
            string userId = message.userId;
            var users = _context.ApplicationUsers.Include(u => u.CombinedReservations).ToList();

            var user = await _userManager.FindByIdAsync(userId);

            foreach (var res in user.CombinedReservations)
            {
                if (res.Id == message.resId)
                {
                    res.Status = "Denied";
                    await _userManager.UpdateAsync(user);
                    _context.Update(user);
                    _context.SaveChanges();
                    return;
                }
            }

        }
    }
}
