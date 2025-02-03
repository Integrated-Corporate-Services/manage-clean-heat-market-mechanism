using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.McsSynchronisation.Api.Entities
{
    public class InstallationRequest
    {
        public InstallationRequest()
        {
            Id = Guid.NewGuid();                
        }
        public Guid Id { get; private set; }

        public DateTime RequestDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int[]? TechnologyTypeIds { get; set; }

        public int[]? IsNewBuildIds { get; set; }

        public List<HeatPumpInstallation> HeatPumpInstallations { get; set; }

        internal InstallationRequestSummaryDto ToSummaryDto(IList<TechnologyType> technologyTypes, IList<NewBuildOption> newBuildOptions)
        {
            var techTypes = TechnologyTypeIds?.Select(t => technologyTypes.Single(i => i.Id == t).Description);
            var newBuilds = IsNewBuildIds?.Select(n => newBuildOptions.Single(i => i.Id == n).Description);

            return new InstallationRequestSummaryDto(Id, RequestDate, StartDate, EndDate, techTypes, newBuilds);
        }
    }
}
