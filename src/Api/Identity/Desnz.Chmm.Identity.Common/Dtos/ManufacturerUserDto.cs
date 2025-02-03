using Desnz.Chmm.Common.Constants;

namespace Desnz.Chmm.Identity.Common.Dtos;

public class ManufacturerUserDto
{
    public Guid? Id { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? CreatedBy { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string JobTitle { get; set; }
    public string TelephoneNumber { get; set; }
    public string? ResponsibleOfficerOrganisationName { get; set; }
    public bool? IsResponsibleOfficer { get; set; }
    public List<ChmmRoleDto> ChmmRoles { get; set; }
}
