using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace UsersMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(sp => 
                        sp.AddSingleton<IHostedService>(new ProceedIfRabbitMqIsAlive("rabbitmq")))
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("usersEndpoint");
                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.ConnectionString("host=rabbitmq");
                    transport.UseConventionalRoutingTopology();
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
