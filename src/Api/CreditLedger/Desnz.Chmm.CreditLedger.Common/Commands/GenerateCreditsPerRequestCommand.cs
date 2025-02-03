using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class GenerateCreditsPerRequestCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Generates credits for the entire requested installations lot
    /// </summary>
    /// <param name="installationRequestId">Scheme year to upload for</param>
    public GenerateCreditsPerRequestCommand(Guid installationRequestId)
    {
        InstallationRequestId = installationRequestId;
    }
    public Guid InstallationRequestId { get; private set; }
}
