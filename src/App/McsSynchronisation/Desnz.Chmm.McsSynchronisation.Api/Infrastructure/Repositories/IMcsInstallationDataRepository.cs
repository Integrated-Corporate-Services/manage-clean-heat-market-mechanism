
using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories
{
    public interface IMcsInstallationDataRepository : IRepository
    {
        Task<InstallationRequest?> GetRequest(DateTime startDate, DateTime endDate);
        Task<IList<HeatPumpInstallation>> GetAll(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool includeHeatPumpProducts = true, bool withTracking = false);
        Task AddInstallationRequests(InstallationRequest installationRequest);
        Task AddInstallations(IEnumerable<HeatPumpInstallation> installations);
        Task AppendProducts(IEnumerable<HeatPumpProduct> products);
        Task RemoveInstallations(IEnumerable<HeatPumpInstallation> installationsToRemove);
        Task AddInstallationProducts(IEnumerable<HeatPumpInstallationProduct> installationProducts);
    }
}