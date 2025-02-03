using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetCreditWeightingsQuery : IRequest<ActionResult<CreditWeightingsDto>>
    {
        public GetCreditWeightingsQuery(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; }
    }
}

