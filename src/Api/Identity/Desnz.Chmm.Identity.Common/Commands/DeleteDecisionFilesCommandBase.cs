using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

public class DeleteDecisionFilesCommandBase : IRequest<ActionResult>
{
    public DeleteDecisionFilesCommandBase(Guid organisationId, string fileName)
    {
        OrganisationId = organisationId;
        FileName = fileName;
    }

    public Guid OrganisationId { get; private set; }
    public string FileName { get; private set; }
}
