using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.YearEnd.Api.Controllers;

public class ProcessEndOfYearCommand : IRequest<ActionResult>
{
    public ProcessEndOfYearCommand()
    { }
}