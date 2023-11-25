using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace QueuePatternSample
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHostBuilder().Build().Run();
        }

        static IHostBuilder GetHostBuilder()
        {

            return new HostBuilder()
                .ConfigureLogging(cfg => cfg.AddConsole())
                .ConfigureServices((c, s) =>
                {
                    s.AddScoped<IMyOrganizationServices, MyOrganizationServices>();
                    s.AddSingleton<MyQueue>();
                    s.AddHostedService(sp => sp.GetService<MyQueue>());
                    s.AddSingleton<IMyQueue>(sp => sp.GetService<MyQueue>());
                    s.AddHostedService<ProducerService>();


                });

        }
    }
}
