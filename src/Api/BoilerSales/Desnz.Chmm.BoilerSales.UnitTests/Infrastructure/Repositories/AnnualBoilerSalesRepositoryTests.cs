using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Infrastructure;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Infrastructure.Repositories
{
    public class AnnualBoilerSalesRepositoryTests
    {
        #region Get

        /// <summary>
        /// Tests that the Get method with no parameters returns the single available record
        /// </summary>
        [Fact]
        public async Task AnnualBoilerSalesRepository_Get_With_No_Parameters_Retrieves_Single_Record()
        {
            // Arrange
            var dbContext = new Mock<IBoilerSalesContext>();
            var logger = new Mock<ILogger<AnnualBoilerSalesRepository>>();
            var repository = new AnnualBoilerSalesRepository(dbContext.Object, logger.Object);
            var schemeYearId = new Guid("11111111-1111-1111-1111-111111111111");
            var organisationId = new Guid("22222222-2222-2222-2222-222222222222");
            var boilerSalesList = new List<AnnualBoilerSales>()
            {
                new(schemeYearId, organisationId, 1000, 0, null, "test-user")
            };
            var boilerSalesDbSet = boilerSalesList.AsQueryable()
                .BuildMockDbSet();
            dbContext.Setup(mock => mock.AnnualBoilerSales)
                .Returns(boilerSalesDbSet.Object);

            // Act
            var result = await repository.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(schemeYearId, result.SchemeYearId);
            Assert.Equal(organisationId, result.OrganisationId);
            Assert.Equal(1000, result.Gas);
            Assert.Equal(0, result.Oil);
            Assert.Equal("Submitted", result.Status);
            Assert.Equal("test-user", result.CreatedBy);
            Assert.NotNull(result.Files);
            Assert.Empty(result.Files);
        }

        /// <summary>
        /// Tests that the Get method returns only those records specified by the given LINQ query
        /// </summary>
        [Fact]
        public async Task AnnualBoilerSalesRepository_Get_Retrieves_Record_Specified_By_Query()
        {
            // Arrange
            var dbContext = new Mock<IBoilerSalesContext>();
            var logger = new Mock<ILogger<AnnualBoilerSalesRepository>>();
            var repository = new AnnualBoilerSalesRepository(dbContext.Object, logger.Object);
            var schemeYearId = new Guid("11111111-1111-1111-1111-111111111111");
            var organisationTwoId = new Guid("22222222-2222-2222-2222-222222222222");
            var organisationThreeId = new Guid("33333333-3333-3333-3333-333333333333");
            var organisationFourId = new Guid("44444444-4444-4444-4444-444444444444");
            var boilerSalesList = new List<AnnualBoilerSales>()
            {
                new(schemeYearId, organisationTwoId, 1000, 0, null, "test-user"),
                new(schemeYearId, organisationThreeId, 500, 500, null, "test-user"),
                new(schemeYearId, organisationFourId, 0, 1000, null, "test-user")
            };
            var boilerSalesDbSet = boilerSalesList.AsQueryable()
                .BuildMockDbSet();
            dbContext.Setup(mock => mock.AnnualBoilerSales)
                .Returns(boilerSalesDbSet.Object);

            // Act
            var result = await repository.Get(abs => abs.SchemeYearId == schemeYearId && abs.OrganisationId == organisationThreeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(schemeYearId, result.SchemeYearId);
            Assert.Equal(organisationThreeId, result.OrganisationId);
            Assert.Equal(500, result.Gas);
            Assert.Equal(500, result.Oil);
            Assert.Equal("Submitted", result.Status);
            Assert.Equal("test-user", result.CreatedBy);
            Assert.NotNull(result.Files);
            Assert.Empty(result.Files);
        }

        /// <summary>
        /// Tests that the Get method returns null if no record matches the given LINQ query
        /// </summary>
        [Fact]
        public async Task AnnualBoilerSalesRepository_Get_Returns_Null_If_No_Record_Found_By_Query()
        {
            // Arrange
            var dbContext = new Mock<IBoilerSalesContext>();
            var logger = new Mock<ILogger<AnnualBoilerSalesRepository>>();
            var repository = new AnnualBoilerSalesRepository(dbContext.Object, logger.Object);
            var schemeYearId = new Guid("11111111-1111-1111-1111-111111111111");
            var organisationTwoId = new Guid("22222222-2222-2222-2222-222222222222");
            var organisationThreeId = new Guid("33333333-3333-3333-3333-333333333333");
            var organisationFourId = new Guid("44444444-4444-4444-4444-444444444444");
            var boilerSalesList = new List<AnnualBoilerSales>()
            {
                new(schemeYearId, organisationTwoId, 1000, 0, null, "test-user"),
                new(schemeYearId, organisationFourId, 0, 1000, null, "test-user")
            };
            var boilerSalesDbSet = boilerSalesList.AsQueryable()
                .BuildMockDbSet();
            dbContext.Setup(mock => mock.AnnualBoilerSales)
                .Returns(boilerSalesDbSet.Object);

            // Act
            var result = await repository.Get(abs => abs.SchemeYearId == schemeYearId && abs.OrganisationId == organisationThreeId);

            // Assert
            Assert.Null(result);
        }

        #endregion Get
    }
}
