using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Queries
{
    public class GetDateTimeQuery : IRequest<ActionResult<string>>
    {
    }
}
