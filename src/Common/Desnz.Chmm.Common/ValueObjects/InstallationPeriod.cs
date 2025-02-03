using MediatR;

namespace Desnz.Chmm.Common.ValueObjects
{
    public class InstallationPeriod<TResponse> : IRequest<TResponse>
    {
        public InstallationPeriod(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime EndDate { get; }
        public DateTime StartDate { get; }
    }
}
