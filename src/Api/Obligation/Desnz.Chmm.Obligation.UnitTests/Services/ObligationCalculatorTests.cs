using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Services;

public class ObligationCalculatorTests
{
    private readonly Mock<ILogger<ObligationCalculator>> _mockLogger;
    private readonly IObligationCalculator _calculator;
    private readonly int _gasBoilerSalesThreshold;
    private readonly int _oilBoilerSalesthreshold;
    private readonly decimal _targetRate;

    public ObligationCalculatorTests()
    {
        _mockLogger = new Mock<ILogger<ObligationCalculator>>();
        _calculator = new ObligationCalculator(_mockLogger.Object);
        _gasBoilerSalesThreshold = 19999;
        _oilBoilerSalesthreshold = 999;
        _targetRate = 4.0m;
    }

    /*
	{
		"Year": "2024",
		"Thresholds": {
			"TargetRate": "4.0",
			"Gas": "19999",
			"Oil": "999"
		}
	}
     */
    [Theory]
    [InlineData(0, 0, 0)]           //0.0
    [InlineData(9, 99, 0)]          //0.0
    [InlineData(19999, 999, 0)]     //0.0
    [InlineData(20000, 1000, 0)]    //0.08
    [InlineData(20005, 1005, 0)]    //0.48
    [InlineData(20005, 1006, 1)]    //0.52
    [InlineData(20199, 1049, 10)]   //10.0
    [InlineData(20199, 1050, 10)]   //10.04
    [InlineData(30000, 1050, 402)]  //402.08
    [InlineData(20050, 0, 2)]       //2.04
    [InlineData(0, 1099, 4)]        //4.00
    [InlineData(30000, 20000, 1160)]        //1160.08
    [InlineData(60000, 0, 1600)]        //1600.04
    [InlineData(0, 10000, 360)]        //360.04
    [InlineData(0, 40000, 1560)]        //360.04
    public void Can_Calculate_Annual_Obligation(int gas, int oil, int expectedObligation)
    {
        var obligation = _calculator.Calculate(gas, oil, _gasBoilerSalesThreshold, _oilBoilerSalesthreshold, _targetRate);
        Assert.Equal(expectedObligation, obligation);
    }

    [Theory]
    [InlineData(0, 0, 0)]           //0.0
    [InlineData(9, 99, 0)]          //0.0
    [InlineData(19999, 999, 0)]     //0.0
    [InlineData(20000, 1000, 0)]    //0.08
    [InlineData(20005, 1005, 0)]    //0.48
    [InlineData(20005, 1006, 1)]    //0.52
    [InlineData(20199, 1049, 10)]   //10.0
    [InlineData(20199, 1050, 10)]   //10.04
    [InlineData(30000, 1050, 402)]  //402.08
    [InlineData(20050, 0, 2)]       //2.04
    [InlineData(0, 1099, 4)]        //4.00

    public void Can_Calculate_Quarterly_Obligation_No_Previous_Quarters(int gas, int oil, int expectedObligation)
    {
        var salesToDate = new List<SalesNumbersDto>();
        var obligationsToDate = new List<int>();
        var obligation = _calculator.Calculate(gas, oil, salesToDate, obligationsToDate, _gasBoilerSalesThreshold, _oilBoilerSalesthreshold, _targetRate);
        Assert.Equal(expectedObligation, obligation);
    }

    [Theory]
    [InlineData(0, 0, 0)]           //0.0
    [InlineData(9, 99, 0)]          //0.0
    [InlineData(19999, 999, 2)]     //2.4
    [InlineData(30000, 1050, 404)]  //404.48
    [InlineData(20050, 0, 2)]       //2.44
    [InlineData(0, 1099, 6)]        //6.00
    public void Can_Calculate_Quarterly_Obligation_One_Previous_Quarter_BelowThreshold(int gas, int oil, int expectedObligation)
    {
        var salesToDate = new List<SalesNumbersDto> { new SalesNumbersDto { Gas = 10, Oil = 50} };
        int[] obligationsToDate =  { 0 };
        var obligation = _calculator.Calculate(gas, oil, salesToDate, obligationsToDate, _gasBoilerSalesThreshold, _oilBoilerSalesthreshold, _targetRate);
        Assert.Equal(expectedObligation, obligation);
    }

    [Theory]
    [InlineData(0, 0, 0)]           //0.0
    [InlineData(9, 99, 4)]          //1164.4
    [InlineData(30000, 1050, 1242)] //2402.08
    [InlineData(20050, 0, 802)]     //1962.08
    [InlineData(0, 1099, 44)]       //1204.04
    public void Can_Calculate_Quarterly_Obligation_One_Previous_Quarter_AboveThreshold(int gas, int oil, int expectedObligation)
    {
        var salesToDate = new List<SalesNumbersDto> { new SalesNumbersDto { Gas = 30000, Oil = 20000 } };
        int[] obligationsToDate = { 1160 };
        var obligation = _calculator.Calculate(gas, oil, salesToDate, obligationsToDate, _gasBoilerSalesThreshold, _oilBoilerSalesthreshold, _targetRate);
        Assert.Equal(expectedObligation, obligation);
    }

    [Fact]
    public void Can_Calculate_4th_Quarterly_Obligation_All_Previous_Quarters_BelowThreshold()
    {
        var salesToDate = new List<SalesNumbersDto> { new SalesNumbersDto { Gas = 15000, Oil = 0 },
                                                      new SalesNumbersDto { Gas = 15000, Oil = 0 },
                                                      new SalesNumbersDto { Gas = 15000, Oil = 0 }};
        int[] obligationsToDate = { 0, 400, 600 };

        var obligation = _calculator.Calculate(15000, 0, salesToDate, obligationsToDate, _gasBoilerSalesThreshold, _oilBoilerSalesthreshold, _targetRate);
        Assert.Equal(600, obligation);
    }

    [Fact]
    public void Can_Calculate_4th_Quarterly_Obligation_All_Previous_Quarters_AboveThreshold()
    {
        var salesToDate = new List<SalesNumbersDto> { new SalesNumbersDto { Gas = 0, Oil = 10000 },
                                                      new SalesNumbersDto { Gas = 0, Oil = 10000 },
                                                      new SalesNumbersDto { Gas = 0, Oil = 10000 }};
        int[] obligationsToDate = { 360, 400, 400 };

        var obligation = _calculator.Calculate(0, 10000, salesToDate, obligationsToDate, _gasBoilerSalesThreshold, _oilBoilerSalesthreshold, _targetRate);
        Assert.Equal(400, obligation);
    }

    [Fact]
    public void Correctly_Generates_Obligation_Summary()
    {
        var userId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction(userId, TransactionConstants.TransactionType.BroughtFowardFromPreviousYear, organisationId, schemeYearId, 1, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.CarryForwardToNextYear, organisationId, schemeYearId, -2, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, 4, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, 8, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.AnnualSubmission, organisationId, schemeYearId, 16, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.QuarterlySubmission, organisationId, schemeYearId, 32, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.Redeemed, organisationId, schemeYearId, -64, DateTime.Now),
        };

        var calculator = new ObligationCalculator(new Mock<ILogger<ObligationCalculator>>().Object);

        var output = calculator.GenerateSummary(transactions);

        Assert.Equal(/*    2 */ 2, output.ObligationAmendments?.Count);
        Assert.Equal(/*   12 */ 4 + 8, output.ObligationAmendments?.Sum(i => i.Value));  // The two Admin Adjustments

        Assert.Equal(/*   61 */ 1 + 4 + 8 + 16 + 32, output.FinalObligations);           // Brought forward + Admin Adjustments + Annual + Quarterly
        Assert.Equal(/*   48 */ 16 + 32, output.GeneratedObligations);                   // Annual + Quarterly

        Assert.Equal(/*   -2 */ -2, output.ObligationsCarriedOver);                      // Carry forward
        Assert.Equal(/*    1 */ 1, output.ObligationsBroughtForward);                    // Brought forward
        Assert.Equal(/*  -64 */ -64, output.ObligationsPaidOff);                         // Redeemed
        Assert.Equal(/*  127 */ 1 + 4 + 8 + 16 + 32 - 2 - 64, output.RemainingObligations);                       // Final Obligations - Carry forward - Redeemed
    }

    [Fact]
    public void Correctly_Generates_Obligation_2_Summary()
    {
        var userId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction(userId, TransactionConstants.TransactionType.BroughtFowardFromPreviousYear, organisationId, schemeYearId, 0, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.CarryForwardToNextYear, organisationId, schemeYearId, -300, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.AnnualSubmission, organisationId, schemeYearId, 857, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.Redeemed, organisationId, schemeYearId, -457, DateTime.Now),
        };

        var calculator = new ObligationCalculator(new Mock<ILogger<ObligationCalculator>>().Object);

        var output = calculator.GenerateSummary(transactions);

        Assert.Equal(0, output.ObligationAmendments?.Count);
        Assert.Equal(0, output.ObligationAmendments?.Sum(i => i.Value));  // The two Admin Adjustments

        Assert.Equal(857, output.FinalObligations);           // Brought forward + Admin Adjustments + Annual + Quarterly
        Assert.Equal(857, output.GeneratedObligations);                   // Annual + Quarterly

        Assert.Equal(-300, output.ObligationsCarriedOver);                      // Carry forward
        Assert.Equal(0, output.ObligationsBroughtForward);                    // Brought forward
        Assert.Equal(-457, output.ObligationsPaidOff);                         // Redeemed
        Assert.Equal(100, output.RemainingObligations);                       // Final Obligations - Carry forward - Redeemed
    }

    [Fact]
    public void CalculateCurrentObligationBalance_ShouldReturnCorrectBalance_WithValidTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction(userId, TransactionConstants.TransactionType.BroughtFowardFromPreviousYear, organisationId, schemeYearId, 0, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.CarryForwardToNextYear, organisationId, schemeYearId, -300, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.AnnualSubmission, organisationId, schemeYearId, 857, DateTime.Now),
            new Transaction(userId, TransactionConstants.TransactionType.Redeemed, organisationId, schemeYearId, -457, DateTime.Now),
        };

        var calculator = new ObligationCalculator(new Mock<ILogger<ObligationCalculator>>().Object);

        // Act
        var result = calculator.CalculateCurrentObligationBalance(transactions);

        // Assert
        Assert.Equal(100, result);
    }

    [Fact]
    public void CalculateCurrentObligationBalance_ShouldReturnZero_WithEmptyTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>();
        var calculator = new ObligationCalculator(new Mock<ILogger<ObligationCalculator>>().Object);

        // Act
        var result = calculator.CalculateCurrentObligationBalance(transactions);

        // Assert
        Assert.Equal(0, result);
    }

}