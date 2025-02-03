using Desnz.Chmm.Common.Providers;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.Common.Mediator.Behaviours;

public class DateTimeOverrideBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public DateTimeOverrideBehavior(IDateTimeProvider dateTime,
        IHttpContextAccessor httpContextAccessor)
    {
        _dateTimeProvider = dateTime;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var dateTime = _httpContextAccessor.HttpContext.Request.Headers["date-time-override"];

        if (dateTime.Any()) {
            var dateParts = dateTime.First().Split("-").Select(i => int.Parse(i)).ToArray();
            var dateOnly = new DateOnly(dateParts[0], dateParts[1], dateParts[2]);

            _dateTimeProvider.OverrideDate(dateOnly);
        }

        return await next();
    }
}