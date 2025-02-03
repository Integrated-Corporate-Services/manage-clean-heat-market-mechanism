using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetObligationCalculationsQuery : IRequest<ActionResult<ObligationCalculationsDto>>
    {
        public GetObligationCalculationsQuery(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; }
    }
}

