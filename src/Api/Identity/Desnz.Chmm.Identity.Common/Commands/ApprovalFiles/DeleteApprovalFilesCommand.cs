namespace Desnz.Chmm.Identity.Common.Commands.ApprovalFiles;

public class DeleteApprovalFilesCommand : DeleteDecisionFilesCommandBase
{
    public DeleteApprovalFilesCommand(Guid organisationId, string fileName) : base(organisationId, fileName)
    {
    }
}
