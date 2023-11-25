using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QueuePatternSample
{
    interface IMyQueue
    {
        /// <summary>
        /// Enqueues new items in the queue. Note that based on the 'BoundedCapacity' and the number of
        /// queues, this operation may need to wait before there is avialable room to add the new Job!
        /// If you do not set the 'BoundedCapacity' the queue capacity would be infinite, in this case
        /// in this case the 'Enqueue' method adds the items and returns immediately.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="millisecondTimeout"></param>
        /// <param name="cancellationToken"></param>
        bool Enqueue(MyQueueContext context, int millisecondTimeout = Timeout.Infinite, CancellationToken cancellationToken = default);
    }
    class MyQueue : BackgroundMultiBlockingTaskHostedService, IMyQueue
    {
        private readonly ILogger<MyQueue> logger;

        public MyQueue(ILogger<MyQueue> logger) : base(2, 1)
        {
            this.logger = logger;
        }
        async Task DoJob(MyQueueContext context, CancellationToken cancellationToken)
        {
            try
            {
                context.Logger.LogInformation("Job Started.!");
                var items = context.Items
                    .Select(x => new ContactModel { Name = x.Name.ToLowerInvariant() }).ToArray();
                await context.ServiceProvider
                    .GetService<IMyOrganizationServices>()
                    .WriteContacts(items);
                /// Simulate some delay
                /// 
                await Task.Delay(2000);
                context.Logger.LogInformation("Job Done.!");
            }
            catch (Exception err)
            {
                context.Logger.LogError(
                    $"An error occured while trying to do job. Error:{err.GetBaseException().Message}");

            }
            finally
            {
                context.Dispose();
            }
        }
        public bool Enqueue(MyQueueContext context, int millisecondTimeout = Timeout.Infinite, CancellationToken cancellationToken = default)
        {

            return base.Enqueue(token => DoJob(context, token), millisecondTimeout, cancellationToken);
        }
    }
}
