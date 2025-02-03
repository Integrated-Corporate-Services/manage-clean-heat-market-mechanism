using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.Identity.Common.Commands.RejectionFiles;

public class UploadRejectionFilesCommand : UploadDecisionFilesCommandBase
{
    public UploadRejectionFilesCommand(Guid organisationId, List<IFormFile> files) : base(organisationId, files)
    {
    }
}
