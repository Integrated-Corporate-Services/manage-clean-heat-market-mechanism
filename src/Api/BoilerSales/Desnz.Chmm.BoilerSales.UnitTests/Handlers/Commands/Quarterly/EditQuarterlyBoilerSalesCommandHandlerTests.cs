using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Commands;
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

public class QuarterlyBoilerSalesTest : QuarterlyBoilerSales
{
    public QuarterlyBoilerSalesTest(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId, int gas, int oil, List<QuarterlyBoilerSalesFile>? files, string? createdBy = null) : base(organisationId, schemeYearId, schemeYearQuarterId, gas, oil, files, createdBy)
    {
    }

    public void SetStatus(string status)
    {
        Status = status;
    }
}
public class EditQuarterlyBoilerSalesCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<EditQuarterlyBoilerSalesCommandHandler>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IBoilerSalesFileCopyService> _mockFileService;
    private readonly Mock<IQuarterlyBoilerSalesRepository> _mockQuarterlyBoilerSalesRepository;
    private readonly Mock<IObligationService> _mockObligationService;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;

    private readonly EditQuarterlyBoilerSalesCommandHandler _handler;

    public EditQuarterlyBoilerSalesCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<EditQuarterlyBoilerSalesCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IBoilerSalesFileCopyService>(MockBehavior.Strict);
        _mockQuarterlyBoilerSalesRepository = new Mock<IQuarterlyBoilerSalesRepository>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
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
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        _dateTimeProvider = new DateTimeOverrideProvider();
        var date = SchemeYearConstants.EndDate.AddDays(1); // OK date is a day after the end date
        _dateTimeProvider.OverrideDate(date);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new EditQuarterlyBoilerSalesCommandHandler(
            _mockLogger.Object,
            _mockFileService.Object,
            _mockQuarterlyBoilerSalesRepository.Object,
            _mockObligationService.Object,
            _dateTimeProvider,
            validator);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]

    public async Task ShouldReturnBadRequest_When_QuarterlyBoilerSales_IsNotFound(bool returnsNull)
    {
        // Arrange
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(returnsNull ? null : new List<QuarterlyBoilerSales>());

        Guid organisationId = Guid.NewGuid(), schemeYearId = Guid.NewGuid(), schemeYearQuarterId = Guid.NewGuid();
        var command = new EditQuarterlyBoilerSalesCommand(organisationId, schemeYearId, schemeYearQuarterId, 5_000, 6_000);
        var expectedResult = Responses.NotFound($"Failed to get Quarterly boiler sales data for quarter with Id: {schemeYearQuarterId}");

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

        _mockFileService.Setup(x => x.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId))
            .ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingQuarterlySession(new List<string>(), new List<string>()));
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Update(It.IsAny<QuarterlyBoilerSales>(), true)).Returns(Task.CompletedTask);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));

        Guid organisationId = Guid.NewGuid(), schemeYearId = Guid.NewGuid(), schemeYearQuarterId = Guid.NewGuid();
        var command = new EditQuarterlyBoilerSalesCommand(organisationId, schemeYearId, schemeYearQuarterId, 5_000, 6_000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_SubmitQuarterlyObligation_Fails()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 12,12,null)
            });


        Guid organisationId = Guid.NewGuid(), schemeYearId = Guid.NewGuid(), schemeYearQuarterId = Guid.NewGuid();
        var command = new EditQuarterlyBoilerSalesCommand(organisationId, schemeYearId, schemeYearQuarterId, 5_000, 6_000);
        var expectedResult = Responses.BadRequest($"Failed to create quarterly obligation for organisation with Id: {_organisationId}, problem: null");


        _mockFileService.Setup(x => x.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId))
            .ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingQuarterlySession(new List<string> { "fileName1" }, new List<string>()));
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Update(It.IsAny<QuarterlyBoilerSales>(), true)).Returns(Task.CompletedTask);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnCreatedResult_When_Valid()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 12,12,null)
            });

        var expectedResult = Responses.Ok();

        _mockFileService.Setup(x => x.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId))
            .ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingQuarterlySession(new List<string> { "fileName1" }, new List<string>()));
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Update(It.IsAny<QuarterlyBoilerSales>(), true)).Returns(Task.CompletedTask);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));

        Guid organisationId = Guid.NewGuid(), schemeYearId = Guid.NewGuid(), schemeYearQuarterId = Guid.NewGuid();
        var command = new EditQuarterlyBoilerSalesCommand(organisationId, schemeYearId, schemeYearQuarterId, 5_000, 6_000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnError_When_ConcludingS3FilesForEditing_Fails()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>(),
                                                             It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 12,12,null)
            });

        var expectedResult = new ObjectResult("Error concluding S3 files for editing: exception: Error 1")
        {
            StatusCode = 500
        };

        _mockFileService.Setup(x => x.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId))
            .ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingQuarterlySession(null,  new List<string> { "Error 1" }));
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Update(It.IsAny<QuarterlyBoilerSales>(), true)).Returns(Task.CompletedTask);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));

        Guid organisationId = Guid.NewGuid(), schemeYearId = Guid.NewGuid(), schemeYearQuarterId = Guid.NewGuid();
        var command = new EditQuarterlyBoilerSalesCommand(organisationId, schemeYearId, schemeYearQuarterId, 5_000, 6_000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }
}
