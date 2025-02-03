using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Common.Queries
{
    public class DownloadRequestDataFileQuery : IRequest<ActionResult<Stream>>
    {
        /// <summary>
        /// Request to download a specific MCS synchronisation request
        /// </summary>
        /// <param name="requestId">The Id of the request to download data for</param>
        public DownloadRequestDataFileQuery(Guid requestId)
        {
            RequestId = requestId;
        }

        /// <summary>
        /// MCS Sync request Id
        /// </summary>
        public Guid RequestId { get; private set; }
    }
}
