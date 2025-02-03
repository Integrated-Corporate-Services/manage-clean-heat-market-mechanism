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

public class InstallationRequestCommandHandler : InstallationRequestBase<InstallationRequestCommand, ActionResult>
{
    public InstallationRequestCommandHandler(
        ILogger<InstallationRequestCommandHandler> logger,
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
    }

    public override async Task<ActionResult> Handle(InstallationRequestCommand command, CancellationToken cancellationToken)
    {
        return await SynchroniseInstallations(command.StartDate, command.EndDate, command.TechnologyTypeIds, command.IsNewBuildIds);
    }
}
