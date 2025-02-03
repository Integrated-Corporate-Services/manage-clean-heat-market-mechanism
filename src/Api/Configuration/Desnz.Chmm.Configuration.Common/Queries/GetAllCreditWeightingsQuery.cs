using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetAllCreditWeightingsQuery : IRequest<ActionResult<List<CreditWeightingsDto>>>
    {
        public GetAllCreditWeightingsQuery()
        { }
    }
}
