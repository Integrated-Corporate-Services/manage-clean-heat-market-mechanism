using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetFirstSchemeYearQuery : IRequest<ActionResult<SchemeYearDto>>
    {
    }
}
