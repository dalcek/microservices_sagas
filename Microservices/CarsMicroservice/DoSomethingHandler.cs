using CarsMicroservice.Models.ContextData;
using Messages;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarsMicroservice
{
    public class DoSomethingHandler : IHandleMessages<DoSomething>
    {
        RentACarContext _context;
        public DoSomethingHandler(RentACarContext context)
        {
            _context = context;
        }
        static ILog log = LogManager.GetLogger<DoSomethingHandler>();
        public Task Handle(DoSomething message, IMessageHandlerContext context)
        {
            // Do something with the message here
            log.Info($"Received PlaceOrder, OrderId = {message.SomeProperty}");
            var companies = _context.RentACarCompanies.ToList();
            foreach(var c in companies) {
                
            }
            return Task.CompletedTask;
        }
    }
}
