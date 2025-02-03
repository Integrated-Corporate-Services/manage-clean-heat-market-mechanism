using Desnz.Chmm.Common.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Commands
{

    public class InstallationRequestCommand : InstallationPeriod<ActionResult>
    {
        public InstallationRequestCommand(DateTime startDate, DateTime endDate, int[]? technologyTypeIds = null, int[]? isNewBuildIds = null) : base(startDate, endDate)
        {
            TechnologyTypeIds = technologyTypeIds;
            IsNewBuildIds = isNewBuildIds;
        }

        public int[]? TechnologyTypeIds { get; }

        public int[]? IsNewBuildIds { get; }
    }
}
