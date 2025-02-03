using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.Controllers;

public class ConfigurationControllerTests
{
    private readonly SchemeYearConfigurationController controller;
    private readonly Mock<IMediator> mediator;

    private readonly Guid organisationId = Guid.NewGuid();
    private readonly Guid schemeYearId = Guid.NewGuid();
    private readonly Guid noteId = Guid.NewGuid();
    private readonly string fileName = "TEST.pdf";
    private readonly CancellationToken cancellationToken = new CancellationToken();

    public ConfigurationControllerTests()
    {
        mediator = new Mock<IMediator>();
        controller = new SchemeYearConfigurationController(mediator.Object);
    }
}