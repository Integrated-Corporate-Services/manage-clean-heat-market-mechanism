using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Quarterly;

public class EditQuarterlyBoilerSalesCopyFilesCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<EditQuarterlyBoilerSalesCopyFilesCommandHandler>> _mockLogger;
    private readonly Mock<IBoilerSalesFileCopyService> _mockFileService;
    private readonly Mock<IQuarterlyBoilerSalesRepository> _mockQuarterlyBoilerSalesRepository;
    private readonly Mock<IRequestValidator> _mockRequestValidator;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;

    private readonly EditQuarterlyBoilerSalesCopyFilesCommandHandler _handler;

    public EditQuarterlyBoilerSalesCopyFilesCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<EditQuarterlyBoilerSalesCopyFilesCommandHandler>>();
        _mockFileService = new Mock<IBoilerSalesFileCopyService>(MockBehavior.Strict);
        _mockQuarterlyBoilerSalesRepository = new Mock<IQuarterlyBoilerSalesRepository>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        _mockRequestValidator = new Mock<IRequestValidator>(MockBehavior.Strict);

        var schemеYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            EndDate = SchemeYearConstants.EndDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate,
            Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto {
                Id = SchemeYearConstants.QuarterOneId,
                EndDate = SchemeYearConstants.QuarterOneEndDate
            } }
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemеYearData);

        _dateTimeProvider = new DateTimeOverrideProvider();
        var date = SchemeYearConstants.EndDate.AddDays(1); // OK date is a day after the end date
        _dateTimeProvider.OverrideDate(date);

        _handler = new EditQuarterlyBoilerSalesCopyFilesCommandHandler(
            _mockLogger.Object,
            _mockFileService.Object,
            _mockQuarterlyBoilerSalesRepository.Object,
            _mockRequestValidator.Object);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_QuarterlyBoilerSales_IsNotFound_OOOOOOOOOOO()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        ActionResult? errorMessage = Responses.BadRequest("error_message");

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(_organisationId,
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<CustomValidator<SchemeYearDto>>()))
            .Returns(Task.FromResult<ActionResult?>(errorMessage));


        var command = new EditQuarterlyBoilerSalesCopyFilesCommand { OrganisationId = _organisationId, SchemeYearId = _schemeYearId, SchemeYearQuarterId = _schemeYearQuarterOneId };
        var expectedResult = Responses.BadRequest("error_message");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]

    public async Task ShouldReturnBadRequest_When_QuarterlyBoilerSales_IsNotFound(bool returnsNull)
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(_organisationId,
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<CustomValidator<SchemeYearDto>>()))
            .Returns(Task.FromResult<ActionResult?>(null));

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(returnsNull ? null : new List<QuarterlyBoilerSales>());

        var command = new EditQuarterlyBoilerSalesCopyFilesCommand { OrganisationId = _organisationId, SchemeYearId = _schemeYearId, SchemeYearQuarterId = _schemeYearQuarterOneId };
        var expectedResult = Responses.BadRequest($"Failed to get quarterly boiler sales data for organisation {_organisationId}, year {_schemeYearId} and quarter {_schemeYearQuarterOneId}");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData(BoilerSalesStatus.Default)]
    [InlineData(BoilerSalesStatus.Due)]
    [InlineData(BoilerSalesStatus.AwaitingReview)]
    [InlineData(BoilerSalesStatus.InReview)]
    [InlineData(BoilerSalesStatus.WaitingForPeerReview)]
    [InlineData(BoilerSalesStatus.WaitingForInformation)]
    [InlineData(BoilerSalesStatus.InPeerReview)]
    public async Task ShouldReturnCreatedResult_When_QuarterlyBoilerSales_Status_Is_Invalid(string status)
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;
        var sales = new QuarterlyBoilerSalesTest(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 12, 12, null);
        sales.SetStatus(status);

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                sales
            });

        var expectedResult = Responses.BadRequest($"Quarterly boiler sales data for: {_organisationId} has an invalid status: {status}");

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(_organisationId,
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<CustomValidator<SchemeYearDto>>()))
            .Returns(Task.FromResult<ActionResult?>(null));

        var command = new EditQuarterlyBoilerSalesCopyFilesCommand { OrganisationId = _organisationId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnCreatedResult_When_Valid()
    {
        // Arrange
        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(It.IsAny<Guid>(),
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<CustomValidator<SchemeYearDto>>()))
            .Returns(Task.FromResult<ActionResult?>(null));

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 0, null)
            });

        _mockFileService.Setup(x => x.PrepareForEditing(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.FromResult(new List<string?>()));

        var expectedResult = Responses.Ok();

        var command = new EditQuarterlyBoilerSalesCopyFilesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturn_500_When_PreparingS3FilesForEditing_Fails()
    {
        // Arrange
        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(It.IsAny<Guid>(),
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<Guid>(),
                                                                             It.IsAny<CustomValidator<SchemeYearDto>>()))
            .Returns(Task.FromResult<ActionResult?>(null));

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 0, null)
            });

        _mockFileService.Setup(x => x.PrepareForEditing(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.FromResult(new List<string?> { "ERROR"}));

        var expectedResult = new ObjectResult("Error preparing S3 files for editing: exception: ERROR")
        {
            StatusCode = 500
        };

        var command = new EditQuarterlyBoilerSalesCopyFilesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
