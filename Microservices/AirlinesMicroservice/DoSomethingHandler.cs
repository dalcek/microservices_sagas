using AirlinesMicroservice.Models.ContextData;
using Messages;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlinesMicroservice
{
    public class DoSomethingHandler : IHandleMessages<DoSomething>
    {
        AirlineContext _context;
        public DoSomethingHandler(AirlineContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<DoSomethingHandler>();
        public Task Handle(DoSomething message, IMessageHandlerContext context)
        {
            // Do something with the message here
            log.Info($"Received PlaceOrder, OrderId = {message.SomeProperty}");
            var airlines = _context.AirlineCompanies.ToList();
            return Task.CompletedTask;
        }
    }
}
