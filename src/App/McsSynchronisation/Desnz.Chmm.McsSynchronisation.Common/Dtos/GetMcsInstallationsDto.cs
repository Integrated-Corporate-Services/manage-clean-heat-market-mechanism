
namespace Desnz.Chmm.McsSynchronisation.Common.Dtos
{
    public class GetMcsInstallationsDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int[]? TechnologyTypeIDs { get; set; }
        public int[]? IsNewBuildIDs { get; set; }

        public GetMcsInstallationsDto(DateTime startDate, DateTime endDate, int[]? technologyTypeIDs, int[]? isNewBuildIDs) 
        {
            StartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
            EndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ss");
            TechnologyTypeIDs = technologyTypeIDs;
            IsNewBuildIDs = isNewBuildIDs;
        }
    }
}
