namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories
{
    public class McsTestDataCreateRepository : McsInstallationDataRepository, IMcsTestDataCreateRepository
    {
        public McsTestDataCreateRepository(McsSynchronisationContext context) : base(context)
        {
        }

        public async Task<int> GetMaxMidId()
        {
            return await Task.Run(() =>
            {
                var installationMaxId = _context.HeatPumpInstallations.Any() ?
                                            _context.HeatPumpInstallations.Max(x => x.MidId ?? 0) :
                                            0;

                var productMaxId = _context.HeatPumpProducts.Any() ?
                                            _context.HeatPumpProducts.Max(x => x.Id) :
                                            0;

                return installationMaxId > productMaxId ? installationMaxId : productMaxId;
            });
        }
    }
}