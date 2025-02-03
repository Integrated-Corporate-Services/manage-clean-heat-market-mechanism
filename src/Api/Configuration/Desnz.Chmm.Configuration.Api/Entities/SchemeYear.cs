using Desnz.Chmm.Common.Entities;
using System;
using System.Linq;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    /// <summary>
    /// Configuration for a scheme year
    /// </summary>
    public class SchemeYear : Entity
    {
        #region Properties
        /// <summary>
        /// The name of the year for display
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The scheme year number
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Start date of the scheme year
        /// </summary>
        public DateOnly StartDate { get; private set; }

        /// <summary>
        /// End date of the scheme year
        /// </summary>
        public DateOnly EndDate { get; private set; }

        /// <summary>
        /// Start date of the trading window
        /// </summary>
        public DateOnly TradingWindowStartDate {get;private set;}

        /// <summary>
        /// End date of the trading window
        /// </summary>
        public DateOnly TradingWindowEndDate {get; private set;}

        /// <summary>
        /// Start date of the credit generation window
        /// </summary>
        public DateOnly CreditGenerationWindowStartDate {get; private set; }

        /// <summary>
        /// End date of the credit generation window
        /// </summary>
        public DateOnly CreditGenerationWindowEndDate { get; private set; }

        /// <summary>
        /// End date for submitting boiler sales
        /// </summary>
        public DateOnly BoilerSalesSubmissionEndDate { get; private set; }

        /// <summary>
        /// The date of surrender day
        /// </summary>
        public DateOnly SurrenderDayDate { get; private set; }

        /// <summary>
        /// The Id of the previous scheme year
        /// </summary>
        public Guid? PreviousSchemeYearId { get; private set; }

        /// <summary>
        /// Quarters assocated with this year
        /// </summary>
        public IReadOnlyCollection<SchemeYearQuarter> Quarters => _quarters;

        public CreditWeighting? CreditWeightings { get; private set; }

        //public ObligationCalculations? ObligationCalculations { get; private set; }
        public ObligationCalculations? ObligationCalculations { get; private set; }
        #endregion

        #region Constructors
        public SchemeYear(string name,
                          int year,
                          DateOnly startDate,
                          DateOnly endDate,
                          DateOnly tradingWindowStartDate,
                          DateOnly tradingWindowEndDate,
                          DateOnly creditGenerationWindowStartDate,
                          DateOnly creditGenerationWindowEndDate,
                          DateOnly boilerSalesSubmissionEndDate,
                          DateOnly surrenderDayDate,
                          List<SchemeYearQuarter> quarters,
                          ObligationCalculations? obligationCalculations = null,
                          CreditWeighting? creditWeightings = null,
                          Guid? previousSchemeYearId = null)
        {
            Name = name;
            Year = year;
            StartDate = startDate;
            EndDate = endDate;
            TradingWindowStartDate = tradingWindowStartDate;
            TradingWindowEndDate = tradingWindowEndDate;
            CreditGenerationWindowStartDate = creditGenerationWindowStartDate;
            CreditGenerationWindowEndDate = creditGenerationWindowEndDate;
            BoilerSalesSubmissionEndDate = boilerSalesSubmissionEndDate;
            SurrenderDayDate = surrenderDayDate;
            PreviousSchemeYearId = previousSchemeYearId;
            CreditWeightings = creditWeightings;
            ObligationCalculations = obligationCalculations;
            _quarters = quarters;
        }

        /// <summary>
        /// Default constuctor
        /// </summary>
        protected SchemeYear() : base() { }
        #endregion

        #region Public methods

        public List<AlternativeSystemFuelTypeWeightingValue> GenerateFuelTypeWeightingValues()
        {
            if (CreditWeightings == null)
                return new List<AlternativeSystemFuelTypeWeightingValue>();

            var values = CreditWeightings.AlternativeSystemFuelTypeWeightings.Select(i => new
            {
                i.AlternativeSystemFuelTypeWeightingValue.Value,
                i.AlternativeSystemFuelTypeWeightingValue.Type
            }).Distinct();

            return values.Select(i => new AlternativeSystemFuelTypeWeightingValue(i.Value, i.Type)).ToList();
        }

        public SchemeYear GenerateNext(List<AlternativeSystemFuelTypeWeightingValue> weightingValues)
        {
            var creditWeightings = CreditWeightings is null ? null :
                                    new CreditWeighting(CreditWeightings.TotalCapacity,
                                                        CreditWeightings.HeatPumpTechnologyTypeWeightings.Select(h => new HeatPumpTechnologyTypeWeighting(h.Code, h.Value)).ToList(),
                                                        CreditWeightings.AlternativeSystemFuelTypeWeightings.Select(a => new AlternativeSystemFuelTypeWeighting(a.Code, weightingValues.Single(i => i.Value == a.AlternativeSystemFuelTypeWeightingValue.Value))).ToList());

            var obligationCalculations = ObligationCalculations is null ? null: 
                                        new ObligationCalculations(ObligationCalculations.PercentageCap,
                                                                    ObligationCalculations.TargetMultiplier,
                                                                    ObligationCalculations.GasCreditsCap,
                                                                    ObligationCalculations.OilCreditsCap,
                                                                    ObligationCalculations.GasBoilerSalesThreshold,
                                                                    ObligationCalculations.OilBoilerSalesThreshold,
                                                                    ObligationCalculations.TargetRate,
                                                                    ObligationCalculations.CreditCarryOverPercentage);

            var next = new SchemeYear(
                (Year + 1).ToString(),
                Year + 1,
                StartDate.AddYears(1),
                EndDate.AddYears(1),
                TradingWindowStartDate.AddYears(1),
                TradingWindowEndDate.AddYears(1),
                CreditGenerationWindowStartDate.AddYears(1),
                CreditGenerationWindowEndDate.AddYears(1),
                BoilerSalesSubmissionEndDate.AddYears(1),
                SurrenderDayDate.AddYears(1),
                Quarters.Select(q => new SchemeYearQuarter(q.Name, q.StartDate.AddYears(1), q.EndDate.AddYears(1))).ToList(),
                obligationCalculations,
                creditWeightings,
                Id);

            return next;
        }

        #endregion

        #region Private fields
        private List<SchemeYearQuarter> _quarters;
        #endregion
    }
}
