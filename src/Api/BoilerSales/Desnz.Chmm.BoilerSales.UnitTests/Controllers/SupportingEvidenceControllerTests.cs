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
    public class SupportingEvidenceControllerTests : TestValidators
    {
        #region GetAnnualSupportingEvidenceFileNames

        /// <summary>
        /// Tests that the GetAnnualSupportingEvidenceFileNames method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_GetAnnualSupportingEvidenceFileNames_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var controller = new SupportingEvidenceController(mediator.Object);
            var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.GetAnnualSupportingEvidenceFileNames(organisationId, schemeYearId, null, cancellationToken);
            // Assert
            mediator.Verify(mock => mock.Send(It.Is<GetAnnualSupportingEvidenceFileNamesQuery>(q => ValidateGetAnnualSupportingEvidenceFileNamesQuery(q, organisationId, schemeYearId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion GetAnnualSupportingEvidenceFileNames

        #region UploadAnnualSupportingEvidence

        /// <summary>
        /// Tests that the UploadAnnualSupportingEvidence method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_UploadAnnualSupportingEvidence_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var controller = new SupportingEvidenceController(mediator.Object);
            var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            var formFiles = new List<IFormFile>();
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.UploadAnnualSupportingEvidence(organisationId, schemeYearId, formFiles, null, cancellationToken);

            // Assert
            mediator.Verify(mock => mock.Send(It.Is<UploadAnnualSupportingEvidenceCommand>(q => ValidateUploadAnnualSupportingEvidenceCommand(q, organisationId, schemeYearId, formFiles)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion UploadAnnualSupportingEvidence

        #region DeleteAnnualSupportingEvidence

        /// <summary>
        /// Tests that the DeleteAnnualSupportingEvidence method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_DeleteAnnualSupportingEvidence_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var controller = new SupportingEvidenceController(mediator.Object);
            var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            const string fileName = "BoilerSales2023.xlsx";
            var deleteAnnualBoilerSalesFile = new DeleteAnnualBoilerSalesFileDto(fileName);
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.DeleteAnnualSupportingEvidence(organisationId, schemeYearId, deleteAnnualBoilerSalesFile, null, cancellationToken);

            // Assert
            mediator.Verify(mock => mock.Send(It.Is<DeleteAnnualSupportingEvidenceCommand>(q => ValidateDeleteAnnualSupportingEvidenceCommand(q, organisationId, schemeYearId, fileName)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion DeleteAnnualSupportingEvidence
    }
}
