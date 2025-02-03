using Desnz.Chmm.BoilerSales.Api.Controllers;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for the Boiler Sales Controller class
    /// </summary>
    public class BoilerSalesControllerTests : TestValidators
    {
        #region GetAnnualBoilerSales

        /// <summary>
        /// Tests that the GetAnnualBoilerSales method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_GetAnnualBoilerSales_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var organisationId = new Guid("99999999-9999-9999-9999-999999999999");
            var mediator = new Mock<IMediator>();
            var httpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new("OrganisationId", organisationId.ToString()) })) };
            var httpContextAccessor = new HttpContextAccessor() { HttpContext = httpContext };
            var controller = new BoilerSalesController(mediator.Object);
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.GetAnnualBoilerSales(organisationId, schemeYearId, cancellationToken);

            // Assert
            mediator.Verify(mock => mock.Send(It.Is<GetAnnualBoilerSalesQuery>(q => ValidateGetAnnualBoilerSalesQuery(q, organisationId, schemeYearId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion GetAnnualBoilerSales

        #region GetQuarterlyBoilerSales

        /// <summary>
        /// Tests that the GetQuarterlyBoilerSales method makes the expected call to the Mediator class
        /// </summary>
        [Fact]
        public async Task BoilerSalesController_GetQuarterlyBoilerSales_Should_Make_Call_To_Mediator()
        {
            // Arrange
            var organisationId = new Guid("99999999-9999-9999-9999-999999999999");
            var mediator = new Mock<IMediator>();
            var httpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new("OrganisationId", organisationId.ToString()) })) };
            var httpContextAccessor = new HttpContextAccessor() { HttpContext = httpContext };
            var controller = new BoilerSalesController(mediator.Object);
            var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
            var cancellationToken = CancellationToken.None;

            // Act
            await controller.GetQuarterlyBoilerSales(organisationId, schemeYearId, cancellationToken);

            // Assert
            mediator.Verify(mock => mock.Send(It.Is<GetQuarterlyBoilerSalesQuery>(q => ValidateGetQuarterlyBoilerSalesQuery(q, organisationId, schemeYearId)), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion GetQuarterlyBoilerSalesForManufacturer
    }
}