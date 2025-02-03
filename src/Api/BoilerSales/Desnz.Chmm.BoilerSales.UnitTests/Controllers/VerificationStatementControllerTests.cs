using Desnz.Chmm.BoilerSales.Api.Controllers;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Controllers
{
    public class VerificationStatementControllerTests : TestValidators
    {
        #region GetAnnualVerificationStatementFileNames

        /// <summary>
        /// Tests that the GetAnnualVerificationStatementFileNames method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_GetAnnualVerificationStatementFileNames_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var httpContextAccessor = new Mock<HttpContextAccessor>();
            var userService = new CurrentUserService(httpContextAccessor.Object);
            var controller = new VerificationStatementController(mediator.Object, userService);
            var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.GetAnnualVerificationStatementFileNames(organisationId, schemeYearId, null, cancellationToken);            // Assert
            mediator.Verify(mock => mock.Send(It.Is<GetAnnualVerificationStatementFileNamesQuery>(q => ValidateGetAnnualVerificationStatementFileNamesQuery(q, organisationId, schemeYearId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion GetAnnualVerificationStatementFileNames

        #region UploadAnnualVerificationStatement

        /// <summary>
        /// Tests that the UploadAnnualVerificationStatement method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_UploadAnnualVerificationStatement_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var httpContextAccessor = new Mock<HttpContextAccessor>();
            var userService = new CurrentUserService(httpContextAccessor.Object);
            var controller = new VerificationStatementController(mediator.Object, userService);
            var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            var formFiles = new List<IFormFile>();
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.UploadAnnualVerificationStatement(organisationId, schemeYearId, formFiles, null, cancellationToken);

            // Assert
            mediator.Verify(mock => mock.Send(It.Is<UploadAnnualVerificationStatementCommand>(q => ValidateUploadAnnualVerificationStatementCommand(q, organisationId, schemeYearId, formFiles)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion UploadAnnualVerificationStatement

        #region DeleteAnnualVerificationStatement

        /// <summary>
        /// Tests that the DeleteAnnualVerificationStatement method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_DeleteAnnualVerificationStatement_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var mockUserService = new Mock<ICurrentUserService>();
            var controller = new VerificationStatementController(mediator.Object, mockUserService.Object);
            var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            const string fileName = "BoilerSales2023.xlsx";
            var deleteAnnualBoilerSalesFile = new DeleteAnnualBoilerSalesFileDto(fileName);
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.DeleteAnnualVerificationStatement(organisationId, schemeYearId, deleteAnnualBoilerSalesFile, null, cancellationToken);

            // Assert
            mediator.Verify(mock => mock.Send(It.Is<DeleteAnnualVerificationStatementCommand>(q => ValidateDeleteAnnualVerificationStatementCommand(q, organisationId, schemeYearId, fileName)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion DeleteAnnualVerificationStatement
    }
}
