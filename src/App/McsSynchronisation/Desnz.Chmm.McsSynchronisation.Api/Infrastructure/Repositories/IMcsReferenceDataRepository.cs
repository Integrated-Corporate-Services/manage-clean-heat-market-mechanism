using Desnz.Chmm.Common;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories
{
    public interface IMcsReferenceDataRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IList<AirTypeTechnology>> GetAllAirTypeTechnologies(Expression<Func<AirTypeTechnology, bool>>? condition = null, bool withTracking = false);
        Task<IList<AlternativeSystemFuelType>> GetAllAlternativeSystemFuelTypes(Expression<Func<AlternativeSystemFuelType, bool>>? condition = null, bool withTracking = false);
        Task<IList<AlternativeSystemType>> GetAllAlternativeSystemTypes(Expression<Func<AlternativeSystemType, bool>>? condition = null, bool withTracking = false);
        Task<IList<InstallationAge>> GetAllInstallationAges(Expression<Func<InstallationAge, bool>>? condition = null, bool withTracking = false);
        Task<IList<Manufacturer>> GetAllManufacturers(Expression<Func<Manufacturer, bool>>? condition = null, bool withTracking = false);
        Task<IList<NewBuildOption>> GetAllNewBuildOptions(Expression<Func<NewBuildOption, bool>>? condition = null, bool withTracking = false);
        Task<IList<RenewableSystemDesign>> GetAllRenewableSystemDesigns(Expression<Func<RenewableSystemDesign, bool>>? condition = null, bool withTracking = false);
        Task<IList<TechnologyType>> GetAllTechnologyTypes(Expression<Func<TechnologyType, bool>>? condition = null, bool withTracking = false);
    }
}