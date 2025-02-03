using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class CalculateCreditsTestCommand : IRequest<ActionResult<List<HeatPumpInstallationDto>>>
{
    /// <summary>
    /// Upload annual supporting evidence files to the given manufacturer
    /// </summary>
    public CalculateCreditsTestCommand(IEnumerable<HeatPumpInstallationDto> installations)
    {
        Installations = installations;
    }

    public IEnumerable<HeatPumpInstallationDto> Installations { get; private set; }
}