using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class DownloadRejectionCommentsFileQuery : IRequest<ActionResult<Stream>>
    {
        public DownloadRejectionCommentsFileQuery(Guid organisationId, string fileName)
        {
            OrganisationId = organisationId;
            FileName = fileName;
        }

        public Guid OrganisationId { get; }
        public string FileName { get; }
    }
}
