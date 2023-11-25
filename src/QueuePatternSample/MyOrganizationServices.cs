using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueuePatternSample
{
    interface IMyOrganizationServices
    {
        Task<IEnumerable<ContactModel>> ReadContacts(int page, int pageLength = 100);
        Task WriteContacts(IEnumerable<ContactModel> contacts);
    }
    class MyOrganizationServices : IMyOrganizationServices
    {
        public async Task<IEnumerable<ContactModel>> ReadContacts(int page, int pageLength = 100)
        {
            await Task.CompletedTask;
            return Enumerable.Range(0, pageLength)
                .Select(x => new ContactModel { Id = Guid.NewGuid().ToString(), Name = $"item {page*pageLength+x}" });

            
        }

        public async Task WriteContacts(IEnumerable<ContactModel> contacts)
        {
            await Task.Delay(3000);
            //throw new NotImplementedException();
        }
    }
}
