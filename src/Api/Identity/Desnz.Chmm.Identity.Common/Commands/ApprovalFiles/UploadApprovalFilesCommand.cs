using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.Identity.Common.Commands.ApprovalFiles;

public class UploadApprovalFilesCommand : UploadDecisionFilesCommandBase
{
    public UploadApprovalFilesCommand(Guid organisationId, List<IFormFile> files) : base(organisationId, files)
    {
    }
}
