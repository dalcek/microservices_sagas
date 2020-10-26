using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class CancelLinkedCarFailed : IEvent
    {
        public int ticket { get; set; }
        public string userId { get; set; }
        public int idRez { get; set; }
        public int seatId { get; set; }
    }
}
