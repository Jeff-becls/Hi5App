using System.Collections.Generic;
using System.Threading.Tasks;

using Hi5WpfEntry.Core.Models;

namespace Hi5WpfEntry.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetContentGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
