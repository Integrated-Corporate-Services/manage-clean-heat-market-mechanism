using Desnz.Chmm.Common.Extensions;

namespace Desnz.Chmm.Common.Providers
{
    /// <summary>
    /// Provides access to DateTime.Now
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// The current date time
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// The date UtcNow
        /// </summary>
        public DateOnly UtcDateNow
        {
            get
            {
                var (date, _) = UtcNow;
                return date;
            }
        }

        public bool IsNowWithinRange(DateOnly startDate, DateOnly endDate, DateRange range = DateRange.Inclusive)
        {
            switch (range)
            {
                case DateRange.Inclusive:
                    return UtcDateNow >= startDate && UtcDateNow <= endDate;
                case DateRange.Exclusive:
                    return UtcDateNow > startDate && UtcDateNow < endDate;
                case DateRange.IncludeEnd:
                    return UtcDateNow > startDate && UtcDateNow <= endDate;
                case DateRange.IncludeStart:
                    return UtcDateNow >= startDate && UtcDateNow < endDate;
            }
            return UtcDateNow >= startDate && UtcDateNow <= endDate;
        }

        public bool IsWithinRange(DateOnly date, DateOnly startDate, DateOnly endDate, DateRange range = DateRange.Inclusive)
        {
            switch (range)
            {
                case DateRange.Inclusive:
                    return date >= startDate && date <= endDate;
                case DateRange.Exclusive:
                    return date > startDate && date < endDate;
                case DateRange.IncludeEnd:
                    return date > startDate && date <= endDate;
                case DateRange.IncludeStart:
                    return date >= startDate && date < endDate;
            }
            return date >= startDate && date <= endDate;
        }

        /// <summary>
        /// This is noop in production
        /// </summary>
        /// <param name="date"></param>
        public void OverrideDate(DateOnly date)
        { }
    }
}
