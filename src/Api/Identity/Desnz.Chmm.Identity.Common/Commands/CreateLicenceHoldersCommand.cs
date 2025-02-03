using Desnz.Chmm.Identity.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands
{
    /// <summary>
    /// A collection of licence holders to create
    /// </summary>
    public class CreateLicenceHoldersCommand : IRequest<ActionResult<List<Guid>>>
    {
        /// <summary>
        /// A collection of licence holders to create
        /// </summary>
        public IEnumerable<LicenceHolderInformationDto> LicenceHolders { get; set; }
    }
}
