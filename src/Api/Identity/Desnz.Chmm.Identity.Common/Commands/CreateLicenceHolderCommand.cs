using Desnz.Chmm.Identity.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    /// <summary>
    /// A single licence holder to create
    /// </summary>
    public class CreateLicenceHolderCommand : LicenceHolderInformationDto, IRequest<ActionResult<Guid>>
    { }
}
