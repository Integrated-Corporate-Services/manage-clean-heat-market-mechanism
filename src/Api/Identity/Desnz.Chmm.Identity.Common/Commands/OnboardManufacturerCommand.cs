
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    public class OnboardManufacturerCommand : IRequest<ActionResult<Guid>>
    {
        public string OrganisationDetailsJson { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
