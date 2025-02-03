namespace Desnz.Chmm.Identity.Common.Dtos.Organisation
{
    public class OrganisationApprovalCommentsDto : OrganisationCommentsBaseDto
    {
    }

    public class OrganisationRejectionCommentsDto : OrganisationCommentsBaseDto
    {
        public string RejectedBy { get; set; }
        public DateTime RejectedOn { get; set; }
    }

    public class OrganisationCommentsBaseDto
    {
        public string? Comments { get; set; }
        public string[] FileNames { get; set; }
    }
}
