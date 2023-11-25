using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QueuePatternSample
{
    /// <summary>
    /// A context to be used in queue this contains:
    ///     Items: The items that should be worked on.
    ///     ServiceProvider: The "Scoped" serviceprovider to be used to get services.
    ///     Id: Some sort of id for this job. It can be used to identify this job.
    /// </summary>
    class MyQueueContext : IDisposable
    {
        private IServiceScope scope;
        public IServiceProvider ServiceProvider => this.scope.ServiceProvider;
        public IEnumerable<ContactModel> Items { get; }
        public ILogger Logger { get; private set; }
        public string Id { get; set; }
        public MyQueueContext(IServiceProvider serviceProvider, IEnumerable<ContactModel> items, string id)
        {
            /// Create an scope for this context.
            /// 
            this.scope = serviceProvider.CreateScope();
            this.Items = items;
            this.Id = id;
            this.Logger = this.ServiceProvider.GetService<ILoggerFactory>().CreateLogger($"Job ({id})");
            

        }
        public IMyOrganizationServices OrganizationServices => this.ServiceProvider.GetService<IMyOrganizationServices>();
        public void Dispose()
        {
            this.scope?.Dispose();
        }
    }
}
