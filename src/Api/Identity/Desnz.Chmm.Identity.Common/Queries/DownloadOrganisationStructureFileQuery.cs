using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class DownloadOrganisationStructureFileQuery : IRequest<ActionResult<Stream>>
    {
        public DownloadOrganisationStructureFileQuery(Guid organisationId, string fileName)
        {
            OrganisationId = organisationId;
            FileName = fileName;
        }

        public Guid OrganisationId { get; }
        public string FileName { get; }
    }
}
