using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    /// <summary>
    /// Configuration valules for a quarter of a scheme year
    /// </summary>
    public class SchemeYearQuarter : Entity
    {
        #region Properties
        /// <summary>
        /// Name of the quarter
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Start date of the quarter
        /// </summary>
        public DateOnly StartDate { get; private set; }

        /// <summary>
        /// End date of the quarter
        /// </summary>
        public DateOnly EndDate { get; private set; }

        /// <summary>
        /// Id of the scheme year this quarter is assocated with
        /// </summary>
        public Guid SchemeYearId { get; private set; }

        /// <summary>
        /// The scheme year this quarter is associated with
        /// </summary>
        public SchemeYear SchemeYear { get; private set; }
        #endregion

        #region Constructors
        public SchemeYearQuarter(string name, DateOnly startDate, DateOnly endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        protected SchemeYearQuarter() : base()
        { }
        #endregion
    }
}
