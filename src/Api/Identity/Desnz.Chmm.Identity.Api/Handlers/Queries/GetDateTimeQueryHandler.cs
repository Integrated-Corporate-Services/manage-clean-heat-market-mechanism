using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Identity.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries
{
    public class GetDateTimeQueryHandler : IRequestHandler<GetDateTimeQuery, ActionResult<string>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetDateTimeQueryHandler(
            IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ActionResult<string>> Handle(GetDateTimeQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_dateTimeProvider.UtcNow.ToString("G"));
        }
    }
}
