using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class SeatNotReserved : IEvent
    {
        public string combinedReservationId { get; set; }
        public string userId { get; set; }
        public int resId { get; set; }
    }
}
