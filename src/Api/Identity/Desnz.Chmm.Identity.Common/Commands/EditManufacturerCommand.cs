using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

public class EditManufacturerCommand : IRequest<ActionResult>
{
    public Guid? OrganisationId { get; set; }
    public string OrganisationDetailsJson { get; set; }
    public string? Comment { get; set; }
    public List<IFormFile>? OrganisationStructureFiles { get; set; }
    public List<IFormFile>? AccountApprovalFiles { get; set; }
}
