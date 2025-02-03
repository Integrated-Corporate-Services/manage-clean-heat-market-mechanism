using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Handlers.Queries;

public class GetInstallationCreditsQueryHandler : BaseRequestHandler<GetInstallationCreditsQuery, ActionResult<List<HeatPumpInstallationCreditsDto>>>
{
    private readonly IInstallationCreditRepository _creditLedgerRepository;

    public GetInstallationCreditsQueryHandler(
        ILogger<BaseRequestHandler<GetInstallationCreditsQuery, ActionResult<List<HeatPumpInstallationCreditsDto>>>> logger,
        IInstallationCreditRepository creditLedgerRepository) : base(logger)
    {
        _creditLedgerRepository = creditLedgerRepository;
    }

    public override async Task<ActionResult<List<HeatPumpInstallationCreditsDto>>> Handle(GetInstallationCreditsQuery query, CancellationToken cancellationToken)
    {
       
        var installationCredits = await _creditLedgerRepository.Get(DateOnly.FromDateTime(query.StartDate), DateOnly.FromDateTime(query.EndDate));

        var installationCreditsSums = installationCredits
            .GroupBy(x => x.HeatPumpInstallationId)
            .Select(y => new HeatPumpInstallationCreditsDto(y.First().HeatPumpInstallationId, y.Sum(z => z.Value)))
            .ToList();

        return installationCreditsSums;
    }
}