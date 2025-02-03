using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.Services;

public class CreditLedgerCalculator_CalculateCarryOverValue_Tests
{
    private readonly Mock<ILogger<CreditLedgerCalculator>> _mockLogger;
    private readonly ICreditLedgerCalculator _calculator;
    

    public CreditLedgerCalculator_CalculateCarryOverValue_Tests()
    {        
        _mockLogger = new Mock<ILogger<CreditLedgerCalculator>>();
        _calculator = new CreditLedgerCalculator(
            _mockLogger.Object);
    }

    [Theory]
    [InlineData(100, 80, 10)]
    [InlineData(85, 80, 5)]
    [InlineData(10.0, 0, 1.0)]  //0.1
    [InlineData(9.0, 0, 1.0)]   //0.9
    [InlineData(8.0, 0, 1.0)]   //0.8
    [InlineData(7.5, 0, 1.0)]   //0.75
    [InlineData(7.4, 0, 0.5)]   //0.74
    [InlineData(7.0, 0, 0.5)]   //0.7
    [InlineData(5.5, 0, 0.5)]   //0.55
    [InlineData(4.5, 0, 0.5)]   //0.45
    [InlineData(4.0, 0, 0.5)]   //0.4
    [InlineData(3.5, 0, 0.5)]   //0.35
    [InlineData(3.0, 0, 0.5)]   //0.3
    [InlineData(2.5, 0, 0.5)]   //0.25
    [InlineData(2.0, 0, 0)]     //0.2
    [InlineData(1.4, 0, 0)]     //0.14
    [InlineData(1.1, 0, 0)]     //0.11

    public async Task GenerateTransactions(decimal credits, decimal obligation, decimal expectedCarryOver)
    {
        var balance = _calculator.CalculateCarryOver(credits, obligation, 10.0M);

        Assert.Equal(expectedCarryOver, balance);
    }

    [Theory]
    [InlineData(100, 10)]
    [InlineData(85, 8.5)]
    [InlineData(10.0, 1.0)]  //0.1
    [InlineData(9.0, 1.0)]   //0.9
    [InlineData(8.0, 1.0)]   //0.8
    [InlineData(7.5, 1.0)]   //0.75
    [InlineData(7.4, 0.5)]   //0.74
    [InlineData(7.0, 0.5)]   //0.7
    [InlineData(5.5, 0.5)]   //0.55
    [InlineData(4.5, 0.5)]   //0.45
    [InlineData(4.0, 0.5)]   //0.4
    [InlineData(3.5, 0.5)]   //0.35
    [InlineData(3.0, 0.5)]   //0.3
    [InlineData(2.5, 0.5)]   //0.25
    [InlineData(2.0, 0)]     //0.2
    [InlineData(1.4, 0)]     //0.14
    [InlineData(1.1, 0)]     //0.11

    public async Task GenerateTransactions2(decimal creditSum,  decimal expectedCarryOver)
    {

        var balance = _calculator.CalculateNewLicenceHoldersCarryOver(creditSum, 10.0M);

        Assert.Equal(expectedCarryOver, balance);
    }
}