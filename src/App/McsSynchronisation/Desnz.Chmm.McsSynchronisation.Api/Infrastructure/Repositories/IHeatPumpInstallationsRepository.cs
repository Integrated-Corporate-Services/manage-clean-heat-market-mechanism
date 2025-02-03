using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;

public interface IHeatPumpInstallationsRepository : IRepository
{
    Task Create(IEnumerable<HeatPumpInstallation> products, bool saveChanges = true);
    Task<Guid> Create(HeatPumpInstallation product, bool saveChanges = true);
    Task<HeatPumpInstallation?> Get(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool withTracking = false);
    Task<List<HeatPumpInstallation>> GetAll(Expression<Func<HeatPumpInstallation, bool>>? condition = null, bool withTracking = false);
}
