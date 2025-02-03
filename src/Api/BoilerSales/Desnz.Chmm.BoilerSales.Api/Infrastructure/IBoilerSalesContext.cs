using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.Common;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure
{
    /// <summary>
    /// Interface for Boiler Sales DB context
    /// </summary>
    public interface IBoilerSalesContext : IUnitOfWork
    {
        /// <summary>
        /// Details of annual boiler sales
        /// </summary>
        DbSet<AnnualBoilerSales> AnnualBoilerSales { get; }

        /// <summary>
        /// Files relating to annual boiler sales
        /// </summary>
        DbSet<AnnualBoilerSalesFile> AnnualBoilerSalesFiles { get; }

        /// <summary>
        /// Changes/adjustments to annual boiler sales figures
        /// </summary>
        DbSet<AnnualBoilerSalesChange> AnnualBoilerSalesChanges { get; }

        /// <summary>
        /// Details of quarterly boiler sales
        /// </summary>
        DbSet<QuarterlyBoilerSales> QuarterlyBoilerSales { get; }

        /// <summary>
        /// Files relating to quarterly boiler sales
        /// </summary>
        DbSet<QuarterlyBoilerSalesFile> QuarterlyBoilerSalesFiles { get; }

        /// <summary>
        /// Changes/adjustments to quarterly sales figures
        /// </summary>
        DbSet<QuarterlyBoilerSalesChange> QuarterlyBoilerSalesChanges { get; }
    }
}
