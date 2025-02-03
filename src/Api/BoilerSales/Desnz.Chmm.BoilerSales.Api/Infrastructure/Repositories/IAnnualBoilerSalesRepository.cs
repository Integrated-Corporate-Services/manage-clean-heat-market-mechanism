using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.Common;
using System.Linq.Expressions;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;

public interface IAnnualBoilerSalesRepository : IRepository
{
    Task<AnnualBoilerSales?> Get(Expression<Func<AnnualBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false);
    Task<Guid> Create(AnnualBoilerSales annualBoilerSales, bool saveChanges = true);
    Task Update(AnnualBoilerSales annualBoilerSales, bool saveChanges = true);
    Task Delete(AnnualBoilerSales annualBoilerSales, bool saveChanges = true);
    Task<List<AnnualBoilerSales>> GetAll(Expression<Func<AnnualBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false);
}
