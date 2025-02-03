namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories
{
    public interface IMcsTestDataCreateRepository : IMcsInstallationDataRepository
    {
        Task<int> GetMaxMidId();
    }
}