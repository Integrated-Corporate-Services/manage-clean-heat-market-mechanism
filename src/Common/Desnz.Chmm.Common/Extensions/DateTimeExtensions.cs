namespace Desnz.Chmm.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static void Deconstruct(this DateTime value, out DateOnly date, out TimeOnly time)
        {
            date = new DateOnly(value.Year, value.Month, value.Day);
            time = new TimeOnly(value.TimeOfDay.Ticks);
        }
    }
}
