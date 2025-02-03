namespace Desnz.Chmm.Testing.Common;

public static class SchemeYearConstants
{
    public static readonly Guid Year2025Id = new("6aa95a01-5283-4975-a360-84a515b17360");


    public const int Year = 2024;

    public static readonly string Name = Year.ToString();
    public static readonly Guid Id = new("d525e380-4aee-40e9-a7f0-1784d8cb49d9");
    public static readonly DateOnly StartDate = new(Year, 01, 01);
    public static readonly DateOnly EndDate = new(Year, 12, 31);

    public static readonly string QuarterOneName = "Quarter 1";
    public static readonly Guid QuarterOneId = new("eb186587-f5d7-422e-b90b-c46b9196c957");
    public static readonly DateOnly QuarterOneStartDate = new(Year, 01, 01);
    public static readonly DateOnly QuarterOneEndDate = new(Year, 03, 31);

    public static readonly string QuarterTwoName = "Quarter 2";
    public static readonly Guid QuarterTwoId = new("e0371e3e-962f-4482-9164-3b2bf8d583b5");
    public static readonly DateOnly QuarterTwoStartDate = new(Year, 04, 01);
    public static readonly DateOnly QuarterTwoEndDate = new(Year, 06, 30);

    public static readonly string QuarterThreeName = "Quarter 3";
    public static readonly Guid QuarterThreeId = new("aeeeec15-2a2b-4011-8940-64ed10ebde33");
    public static readonly DateOnly QuarterThreeStartDate = new(Year, 07, 01);
    public static readonly DateOnly QuarterThreeEndDate = new(Year, 09, 30);

    public static readonly string QuarterFourName = "Quarter 4";
    public static readonly Guid QuarterFourId = new("d42090a4-3589-4009-bc17-9e8146ab1b15");
    public static readonly DateOnly QuarterFourStartDate = new(Year, 10, 01);
    public static readonly DateOnly QuarterFourEndDate = new(Year, 12, 31);

    public static DateOnly TradingWindowStartDate = new(Year, 10, 01);
    public static DateOnly TradingWindowEndDate = new(Year + 1, 09, 30);

    public static DateOnly BoilerSalesSubmissionEndDate = new(Year + 1, 03, 31);

    public static DateOnly CreditGenerationWindowStartDate = new(Year, 04, 01);
    public static DateOnly CreditGenerationWindowEndDate = new(Year + 1, 03, 31);

    public static DateOnly SurrenderDayDate = new(Year + 1, 10, 01);

    public static int GasBoilerSalesThreshold = 19999;
    public static int OilBoilerSalesthreshold = 999;
    public static decimal TargetRate = 4.0m;
    public static decimal CreditCarryOverPercentage = 0.1m;

    public static readonly Guid[] SchemeYearQuarterIds = new Guid[]
    {
        QuarterOneId,
        QuarterTwoId,
        QuarterThreeId,
        QuarterFourId
    };

}
