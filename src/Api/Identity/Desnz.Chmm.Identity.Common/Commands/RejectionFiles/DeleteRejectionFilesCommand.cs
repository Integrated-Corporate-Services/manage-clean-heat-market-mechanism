namespace Desnz.Chmm.Identity.Common.Commands.RejectionFiles;

public class DeleteRejectionFilesCommand : DeleteDecisionFilesCommandBase
{
    public DeleteRejectionFilesCommand(Guid organisationId, string fileName) : base(organisationId, fileName)
    {
    }
}
