using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Testing.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.UnitTests.Services;

public class CarryForwardObligationCalculatorTests
{
    private readonly Mock<ILogger<CarryForwardObligationCalculator>> _mockLogger;
    private readonly ICarryForwardObligationCalculator _calculator;
    private readonly Guid _userId;
    private readonly Guid _organisationId;

    public CarryForwardObligationCalculatorTests()
    {
        _mockLogger = new Mock<ILogger<CarryForwardObligationCalculator>>();
        _calculator = new CarryForwardObligationCalculator(_mockLogger.Object);

        _userId = Guid.NewGuid();
        _organisationId = Guid.NewGuid();
    }

    [Theory] // Obligation  | Credits  | Credit Cap  | % Cap  | Multiplier | Expected
    [InlineData(       440,     400.0,          300,    35.0,          1.0,        40 )]
    [InlineData(       440,      50.0,          300,    35.0,          1.0,       300 )]
    [InlineData(       440,     300.0,          300,    35.0,          1.0,       140 )]
    [InlineData(        57,    5389.5,          300,    35.0,          1.0,         0 )]
    [InlineData(       400,     600.0,          300,    35.0,          1.0,         0 )]
    [InlineData(       800,     495.0,          300,    35.0,          1.0,       300 )]
    [InlineData(       878,     555.0,          300,    35.0,          1.0,       307 )]
    [InlineData(         0,       0.0,           50,    35.0,          1.0,         0 )]
    [InlineData(       440,     100.0,          300,    35.0,          1.0,       300 )]
    [InlineData(       120,      80.0,           50,    35.0,          1.0,        40 )]
    [InlineData(        80,      30.0,           50,    35.0,          1.0,        50 )]
    [InlineData(         0,       0.0,            0,    35.0,          1.0,         0 )]
    [InlineData(       440,     300.0,          300,    35.0,          1.2,       168 )]
    [InlineData(     10000,    2000.0,	        300,    35.0,          1.0,      3500 )]
    [InlineData(       360,       0.0,	         50,    35.0,          1.0,       126 )]
    [InlineData(     14307,    3057.0,	        300,    35.0,          1.0,      5007 )]
    public void Can_Calculate_CarryForwardObligation_For_SingleSchemeYear(int obligation, decimal creditsTotal, int creditsCap, decimal percentageCap, decimal targetMultiplier, int expectedObligation)
    {
        var currentYearObligation = new Transaction(_userId, TransactionType.AnnualSubmission, _organisationId, SchemeYearConstants.Id, obligation, new DateTime(2024, 1, 10));
        IEnumerable<Transaction> obligationTransactions = new List<Transaction> { currentYearObligation };

        var cfObligation = _calculator.Calculate(obligationTransactions,
                                                 creditsTotal,
                                                 creditsCap,
                                                 percentageCap,
                                                 targetMultiplier);
        Assert.Equal(expectedObligation, cfObligation);
    }

    [Theory] // Obligation  | Adjustment  | Brought Forward  | Credits  | Credit Cap | % Cap  | Multiplier | Expected
    [InlineData(     20200,            0,              5000,         0,          300,   35.0,          1.0,      7070)]
    [InlineData(       883,            0,                 5,       555,          300,   35.0,          1.0,       309)]
    [InlineData(      1320,           80,               140,       300,          300,   35.0,          1.0,       490)]
    public void Can_Calculate_CarryForwardObligation_For_MultipleSchemeYear(int obligation,
                                                                            int adjustment,
                                                                            int broughtForward,
                                                                            decimal creditsTotal,
                                                                            int creditsCap,
                                                                            decimal percentageCap,
                                                                            decimal targetMultiplier,
                                                                            int expectedObligation)
    {
        var currentYearObligation = new Transaction(_userId, TransactionType.AnnualSubmission, _organisationId, SchemeYearConstants.Year2025Id, obligation, new DateTime(2025, 1, 10));
        var broughtFowardObligation = new Transaction(_userId, TransactionType.BroughtFowardFromPreviousYear, _organisationId, SchemeYearConstants.Year2025Id, broughtForward, new DateTime(2024, 1, 10));
        var adminAdjustment = new Transaction(_userId, TransactionType.AdminAdjustment, _organisationId, SchemeYearConstants.Year2025Id, adjustment, new DateTime(2025, 1, 11));

        IEnumerable<Transaction> obligationTransactions = new List<Transaction> { currentYearObligation, broughtFowardObligation, adminAdjustment };
        var currentYear = new Configuration.Common.Dtos.SchemeYearDto { Id = SchemeYearConstants.Year2025Id };


        var cfObligation = _calculator.Calculate(obligationTransactions,
                                                 creditsTotal,
                                                 creditsCap,
                                                 percentageCap,
                                                 targetMultiplier);
        Assert.Equal(expectedObligation, cfObligation);
    }


    [Fact]
    public void Can_Calculate_When_Wrong_SchemeYear()
    {
        var creditsTotal = 300.0m;
        var creditsCap = 300;
        var percentageCap = 35.0m;
        var targetMultiplier = 1.0m;

        IEnumerable<Transaction> obligationTransactions = new List<Transaction> ();



        var cfObligation = _calculator.Calculate(obligationTransactions,
                                                 creditsTotal,
                                                 creditsCap,
                                                 percentageCap,
                                                 targetMultiplier);
        Assert.Equal(0, cfObligation);
    }


    [Fact]
    public void Can_Calculate_Carried_Over()
    {
        var creditsTotal = 50.0m;
        var creditsCap = 300;
        var percentageCap = 35.0m;
        var targetMultiplier = 1.0m;

        IEnumerable<Transaction> obligationTransactions = new List<Transaction>
        {
            new Transaction(_userId, TransactionType.AnnualSubmission, _organisationId, SchemeYearConstants.Id, 60, DateTime.Now),
            new Transaction(_userId, TransactionType.AdminAdjustment, _organisationId, SchemeYearConstants.Id, 40, DateTime.Now)
        };

        var cfObligation = _calculator.Calculate(obligationTransactions,
                                                 creditsTotal,
                                                 creditsCap,
                                                 percentageCap,
                                                 targetMultiplier);
        Assert.Equal(50, cfObligation);
    }

}