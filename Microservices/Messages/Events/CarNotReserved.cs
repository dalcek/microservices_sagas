using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class CarNotReserved : IEvent
    {
        public string combinedReservationId { get; set; }
        public List<Tuple<int, int>> tickets { get; set; }
        public string userId { get; set; }
        public int resId { get; set; }
    }
}
