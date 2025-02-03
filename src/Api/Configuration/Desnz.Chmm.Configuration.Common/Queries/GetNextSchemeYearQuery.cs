using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetNextSchemeYearQuery : IRequest<ActionResult<SchemeYearDto>>
    {
        public GetNextSchemeYearQuery(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; }
    }
}

