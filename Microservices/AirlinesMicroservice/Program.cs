using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace AirlinesMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    //var endpointConfiguration = new EndpointConfiguration("airlinesEndpoint");
                    //var transport = endpointConfiguration.UseTransport<LearningTransport>();
                    ////var routing = transport.Routing();
                    ////routing.RouteToEndpoint(typeof(DoSomething), "carsEndpoint");
                    ////transport.Routing().RouteToEndpoint(
                    ////    assembly: typeof(DoSomething).Assembly,
                    ////    destination: "sender");

                    ////endpointConfiguration.SendOnly();

                    //return endpointConfiguration;

                    var endpointConfiguration = new EndpointConfiguration("airlinesEndpoint");
                    //var transport = endpointConfiguration.UseTransport<LearningTransport>();
                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.ConnectionString("host=rabbitmq");
                    transport.UseConventionalRoutingTopology();
                    //var routing = transport.Routing();
                    //routing.RouteToEndpoint(typeof(DoSomething), "airlinesEndpoint");
                    //transport.Routing().RouteToEndpoint(
                    //    assembly: typeof(DoSomething).Assembly,
                    //    destination: "sender");

                    //endpointConfiguration.SendOnly();
                    endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                    endpointConfiguration.EnableInstallers();
                    endpointConfiguration.DefineCriticalErrorAction(CriticalErrorActions.RestartContainer);

                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
