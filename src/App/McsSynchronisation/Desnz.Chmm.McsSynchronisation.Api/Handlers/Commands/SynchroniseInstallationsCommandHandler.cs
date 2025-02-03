using AutoMapper;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Api.Services;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IDateTimeProvider = Desnz.Chmm.Common.Providers.IDateTimeProvider;

namespace Desnz.Chmm.McsSynchronisation.Api.Handlers.Commands;

public class SynchroniseInstallationsCommandHandler : InstallationRequestBase<SynchroniseInstallationsCommand, ActionResult>
{
    private readonly IDateTimeProvider _datetimeProvider;

    public SynchroniseInstallationsCommandHandler(
        ILogger<SynchroniseInstallationsCommandHandler> logger,
        IMapper mapper,
        IMcsInstallationDataRepository mcsDataRepository,
        ILicenceHolderService licenceHolderService,
        ICreditLedgerService creditLedgerService,
        IIdentityService identityService,
        IOptions<ApiKeyPolicyConfig> apiKeyPolicyConfig,
        IHttpContextAccessor httpContextAccessor,
        IDateTimeProvider datetimeProvider,
        IMcsMidService mcsMidService
    ) : base(logger,
        mapper,
        mcsDataRepository,
        licenceHolderService,
        creditLedgerService,
        identityService,
        apiKeyPolicyConfig,
        httpContextAccessor,
        datetimeProvider,
        mcsMidService)
    {
        _datetimeProvider = datetimeProvider;
    }

    public override async Task<ActionResult> Handle(SynchroniseInstallationsCommand command, CancellationToken cancellationToken)
    {
        var (startDate, endDate) = GetPreviousWeekPeriod(_datetimeProvider.UtcNow);

        var exists = await DoesAlreadyExist(startDate, endDate);
        if (exists)
        {
            return McsDataAlreadyExists(startDate, endDate);
        }

        return await SynchroniseInstallations(startDate, endDate, null, null);
    }

    protected (DateTime, DateTime) GetPreviousWeekPeriod(DateTime today)
    {
        DateTime currentWeekStartDate = GetCurrentWeekStartDate(today);
        var startDate = currentWeekStartDate.AddDays(-7);
        var endDate = startDate.AddDays(7).AddMilliseconds(-1);
        return (startDate, endDate);
    }

    private static DateTime GetCurrentWeekStartDate(DateTime today)
    {
        return today.Date.AddDays(-(int)today.DayOfWeek);
    }
}
