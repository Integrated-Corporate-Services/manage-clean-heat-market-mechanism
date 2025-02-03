using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.Common;
using System.Linq.Expressions;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;

public interface IQuarterlyBoilerSalesRepository : IRepository
{
    Task<List<QuarterlyBoilerSales>?> Get(Expression<Func<QuarterlyBoilerSales, bool>>? condition = null, bool includeFiles = false, bool includeChanges = false, bool withTracking = false);
    Task<Guid> Create(QuarterlyBoilerSales quarterlyBoilerSales, bool saveChanges = true);
    Task<List<QuarterlyBoilerSales>?> GetAllNonAnnual(Guid schemeYearId);
    Task Update(QuarterlyBoilerSales quarterlyBoilerSales, bool saveChanges = true);
}
