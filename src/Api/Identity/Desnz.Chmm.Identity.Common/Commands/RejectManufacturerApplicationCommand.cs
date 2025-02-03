using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

public class RejectManufacturerApplicationCommand : IRequest<ActionResult>
{
    public Guid? OrganisationId { get; set; }
    public string? Comment { get; set; }
    public List<IFormFile>? AccountDecisionFiles { get; set; }
}
