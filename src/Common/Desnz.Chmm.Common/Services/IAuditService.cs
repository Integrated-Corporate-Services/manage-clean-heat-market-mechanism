using Desnz.Chmm.Common.ValueObjects;

namespace Desnz.Chmm.Common.Services
{
    public interface IAuditService
    {
        ResponseDetails ProcessResponse<TResponse>(TResponse response);
        ResponseDetails ProcessException(Exception ex);
        AuditDetails GenerateAuditDetails<TRequest>(TRequest request, ResponseDetails responseDetails);
        Task LogAuditItem(AuditDetails details, long duration);
    }
}
