using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetSchemeYearSummaryConfigurationQuery : IRequest<ActionResult<SchemeYearSummaryConfigurationDto>>
    {
        public GetSchemeYearSummaryConfigurationQuery(Guid schemeYearId)
        {
            SchemeYearId = schemeYearId;
        }

        public Guid SchemeYearId { get; }
    }
}

