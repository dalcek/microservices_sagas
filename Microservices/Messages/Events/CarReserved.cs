using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class CarReserved : IEvent
    {
        public string combinedReservationId { get; set; }
        public List<Tuple<int, int>> tickets { get; set; }
        public Tuple<int, int> carRes { get; set; }
        public string userId { get; set; }
        public string pointsS { get; set; }
        public int resId { get; set; }
    }
}
