using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Entities
{
    public class AnnualBoilerSalesTests
    {
        [Fact]
        public void When_Approving_Boiler_Sales_Status_Is_Approved()
        {
            // Arrange
            var salesData = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, null);

            // Act
            salesData.Approve();

            //Assert
            Assert.Equal(BoilerSalesStatus.Approved, salesData.Status);
        }
    }
}
