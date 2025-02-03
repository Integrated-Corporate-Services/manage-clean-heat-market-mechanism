namespace Desnz.Chmm.Identity.Common.Commands.StructureFiles;

public class DeleteStructureFilesCommand : DeleteDecisionFilesCommandBase
{
    public DeleteStructureFilesCommand(Guid organisationId, string fileName) : base(organisationId, fileName)
    {
    }
}
