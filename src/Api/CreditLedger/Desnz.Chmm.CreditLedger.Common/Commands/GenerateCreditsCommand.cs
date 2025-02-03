using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class GenerateCreditsCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Generates credits for the entire requested installations lot
    /// </summary>
    /// <param name="installationRequestId">Scheme year to upload for</param>
    public IEnumerable<McsInstallationDto> Installations { get; private set; }

    public GenerateCreditsCommand(IEnumerable<McsInstallationDto> installations)
    {
        Installations = installations;
    }
}
