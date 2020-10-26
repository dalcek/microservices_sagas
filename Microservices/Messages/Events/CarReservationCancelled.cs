using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.Events
{
    public class CarReservationCancelled : IEvent
    {
        public string userId { get; set; }
    }
}
