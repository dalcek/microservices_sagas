using Messages.Model;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class SeatReserved : IEvent
    {
        public string combinedReservationId { get; set; }
        public CarReservation car { get; set; }
        public List<Tuple<int, int>> tickets { get; set; }
        public string userId { get; set; }
        public int mainSeatId { get; set; }
        public string pointsS { get; set; }
        public int resId { get; set; }
    }
}
