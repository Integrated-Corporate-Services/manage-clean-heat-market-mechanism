using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Infrastructure;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.Testing.Common;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Infrastructure.Repositories
{
    public class QuarterlyBoilerSalesRepositoryTests
    {
        #region Get

        [Fact]
        public async Task QuarterlyBoilerSalesRepository_Get_With_No_Parameters_Retrieves_All_Records()
        {
            // Arrange
            var dbContext = new Mock<IBoilerSalesContext>();
            var logger = new Mock<ILogger<QuarterlyBoilerSalesRepository>>();
            var repository = new QuarterlyBoilerSalesRepository(dbContext.Object, logger.Object);
            var schemeYearQuarterId = new Guid("11111111-1111-1111-1111-111111111111");
            var organisationTwoId = new Guid("22222222-2222-2222-2222-222222222222");
            var organisationThreeId = new Guid("33333333-3333-3333-3333-333333333333");
            var organisationFourId = new Guid("44444444-4444-4444-4444-444444444444");
            var boilerSalesList = new List<QuarterlyBoilerSales>()
            {
                new(organisationTwoId, SchemeYearConstants.Id, schemeYearQuarterId, 1000, 0, null, "test-user"),
                new(organisationThreeId, SchemeYearConstants.Id, schemeYearQuarterId, 500, 500, null, "test-user"),
                new(organisationFourId, SchemeYearConstants.Id, schemeYearQuarterId, 0, 1000, null, "test-user")
            };
            var boilerSalesDbSet = boilerSalesList.AsQueryable()
                .BuildMockDbSet();
            dbContext.Setup(mock => mock.QuarterlyBoilerSales)
                .Returns(boilerSalesDbSet.Object);

            // Act
            var result = await repository.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(boilerSalesList[0], result);
            Assert.Contains(boilerSalesList[1], result);
            Assert.Contains(boilerSalesList[2], result);
        }

        [Fact]
        public async Task QuarterlyBoilerSalesRepository_Get_Retrieves_Records_Specified_By_Query()
        {
            // Arrange
            var dbContext = new Mock<IBoilerSalesContext>();
            var logger = new Mock<ILogger<QuarterlyBoilerSalesRepository>>();
            var repository = new QuarterlyBoilerSalesRepository(dbContext.Object, logger.Object);
            var schemeYearId = new Guid("11111111-1111-1111-1111-111111111111");
            var organisationTwoId = new Guid("22222222-2222-2222-2222-222222222222");
            var organisationThreeId = new Guid("33333333-3333-3333-3333-333333333333");
            var organisationFourId = new Guid("44444444-4444-4444-4444-444444444444");
            var boilerSalesList = new List<QuarterlyBoilerSales>()
            {
                new(organisationTwoId, schemeYearId, SchemeYearConstants.QuarterOneId, 1000, 0, null, "test-user"),
                new(organisationThreeId, schemeYearId, SchemeYearConstants.QuarterOneId, 500, 500, null, "test-user"),
                new(organisationFourId, schemeYearId, SchemeYearConstants.QuarterOneId, 0, 1000, null, "test-user")
            };
            var boilerSalesDbSet = boilerSalesList.AsQueryable()
                .BuildMockDbSet();
            dbContext.Setup(mock => mock.QuarterlyBoilerSales)
                .Returns(boilerSalesDbSet.Object);

            // Act
            var result = await repository.Get(abs => abs.SchemeYearQuarterId == SchemeYearConstants.QuarterOneId && 
                                                    (abs.OrganisationId == organisationThreeId || abs.OrganisationId == organisationFourId));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(boilerSalesList[1], result);
            Assert.Contains(boilerSalesList[2], result);
        }

        [Fact]
        public async Task QuarterlyBoilerSalesRepository_Get_Returns_Null_If_No_Record_Found_By_Query()
        {
            // Arrange
            var dbContext = new Mock<IBoilerSalesContext>();
            var logger = new Mock<ILogger<QuarterlyBoilerSalesRepository>>();
            var repository = new QuarterlyBoilerSalesRepository(dbContext.Object, logger.Object);
            var schemeYearId = new Guid("11111111-1111-1111-1111-111111111111");
            var organisationTwoId = new Guid("22222222-2222-2222-2222-222222222222");
            var organisationThreeId = new Guid("33333333-3333-3333-3333-333333333333");
            var organisationFourId = new Guid("44444444-4444-4444-4444-444444444444");
            var boilerSalesList = new List<QuarterlyBoilerSales>()
            {
                new(organisationTwoId, schemeYearId, SchemeYearConstants.QuarterOneId, 1000, 0, null, "test-user"),
                new(organisationFourId, schemeYearId, SchemeYearConstants.QuarterOneId, 0, 1000, null, "test-user")
            };
            var boilerSalesDbSet = boilerSalesList.AsQueryable()
                .BuildMockDbSet();
            dbContext.Setup(mock => mock.QuarterlyBoilerSales)
                .Returns(boilerSalesDbSet.Object);

            // Act
            var result = await repository.Get(qbs => qbs.SchemeYearQuarterId == schemeYearId && qbs.OrganisationId == organisationThreeId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion Get
    }
}
