using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace QueuePatternSample
{
    class ProducerService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ProducerService> logger;

        public ProducerService(IServiceProvider serviceProvider, ILogger<ProducerService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var page = 0;
            var pagelength = 100;

            return Task.Run(async () =>
            {
                await Task.Delay(5000);
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var items = await this.serviceProvider
                            .GetService<IMyOrganizationServices>()
                            .ReadContacts(page, pagelength);
                        /// Create a disposable context
                        /// to add to queue.
                        var context = new MyQueueContext(this.serviceProvider, items, page.ToString());
                        if (!this.serviceProvider
                            .GetService<IMyQueue>()
                            .Enqueue(context))
                        {
                            throw new Exception("Failed to Enqueue Items");
                        }

                        page++;
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while producing items. Err:{err.GetBaseException().Message}");

                    }
                }



            });
        }
    }
}
