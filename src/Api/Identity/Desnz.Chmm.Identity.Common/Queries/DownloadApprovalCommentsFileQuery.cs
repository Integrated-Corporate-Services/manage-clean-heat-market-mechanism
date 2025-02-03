using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class DownloadApprovalCommentsFileQuery : IRequest<ActionResult<Stream>>
    {
        public DownloadApprovalCommentsFileQuery(Guid organisationId, string fileName)
        {
            OrganisationId = organisationId;
            FileName = fileName;
        }

        public Guid OrganisationId { get; }
        public string FileName { get; }
    }
}
