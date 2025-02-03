using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.ValueObjects;
using MediatR;
using System.Diagnostics;

namespace Desnz.Chmm.Common.Mediator.Behaviours;
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IAuditService _auditService;

    private static readonly IList<string> _excludeAuditTypes =
    [
        "Clear manufacturer note files",
        "Edit quarterly boiler sales copy files",
        "Edit annual boiler sales copy files",
        "Create quarterly obligation"
    ];

    public AuditBehavior(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        ResponseDetails details = ResponseDetails.Default;

        stopwatch.Start();

        try
        {
            var response = await next();

            details = _auditService.ProcessResponse(response);
            return response;
        }
        catch (Exception ex)
        {
            details = _auditService.ProcessException(ex);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            var auditDetails = _auditService.GenerateAuditDetails(request, details);
            if (!_excludeAuditTypes.Contains(auditDetails.FriendlyName))
            {
                await _auditService.LogAuditItem(auditDetails, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}

