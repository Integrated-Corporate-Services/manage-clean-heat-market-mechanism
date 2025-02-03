namespace Desnz.Chmm.CreditLedger.Common.Dtos;

public class LicenceOwnershipDto
{
    public required Guid OrganisationId { get; init; }
    public required Guid LicenceHolderId { get; init; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
