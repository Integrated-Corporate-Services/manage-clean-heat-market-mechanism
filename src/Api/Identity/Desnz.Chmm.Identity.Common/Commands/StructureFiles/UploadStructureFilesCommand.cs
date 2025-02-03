using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.Identity.Common.Commands.StructureFiles;

public class UploadStructureFilesCommand : UploadDecisionFilesCommandBase
{
    public UploadStructureFilesCommand(Guid organisationId, List<IFormFile> files) : base(organisationId, files)
    {
    }
}
