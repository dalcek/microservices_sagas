using Messages.Model;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class ReservationCreated : IEvent
    {
        public string combinedReservationId { get; set; }

        public CarReservation car { get; set; }
        public List<TicketReservation> tickets { get; set; }

        public string userId { get; set; }

        public string pointsS { get; set; }

        public int resId { get; set; }

    }

    
}
