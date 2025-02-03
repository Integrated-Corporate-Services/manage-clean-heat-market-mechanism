namespace Desnz.Chmm.Configuration.Common.Dtos
{
    public class SchemeYearDto
    {
        public Guid Id { get; set; }
        public Guid? PreviousSchemeYearId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly TradingWindowStartDate { get; set; }
        public DateOnly TradingWindowEndDate { get; set; }
        public DateOnly CreditGenerationWindowStartDate { get; set; }
        public DateOnly CreditGenerationWindowEndDate { get; set; }
        public DateOnly BoilerSalesSubmissionEndDate { get; set; }
        public DateOnly SurrenderDayDate { get; set; }
        public List<SchemeYearQuarterDto>? Quarters { get; set; }
        public SchemeYearQuarterDto QuarterOne { get { return Quarters.OrderBy(i => i.StartDate).FirstOrDefault(); } }
        public SchemeYearQuarterDto QuarterTwo { get { return Quarters.OrderBy(i => i.StartDate).Skip(1).FirstOrDefault(); } }
        public SchemeYearQuarterDto QuarterThree { get { return Quarters.OrderBy(i => i.StartDate).Skip(2).FirstOrDefault(); } }
        public SchemeYearQuarterDto QuarterFour { get { return Quarters.OrderBy(i => i.StartDate).Skip(3).FirstOrDefault(); } }
    }
}
