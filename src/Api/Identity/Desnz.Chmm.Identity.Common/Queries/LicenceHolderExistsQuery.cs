using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class LicenceHolderExistsQuery : IRequest<ActionResult<LicenceHolderExistsDto>>
    {
        public Guid LicenceHolderId { get; private set; }

        public LicenceHolderExistsQuery(Guid licenceHolderId)
        {
            LicenceHolderId = licenceHolderId;
        }
    }
}
