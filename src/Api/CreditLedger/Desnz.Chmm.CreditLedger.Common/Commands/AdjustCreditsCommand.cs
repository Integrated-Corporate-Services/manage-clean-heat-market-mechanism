using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

public class AdjustCreditsCommand : IRequest<ActionResult>
{
    public AdjustCreditsCommand(Guid organisationId, Guid schemeYearId, decimal value)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        Value = value;
    }

    public Guid OrganisationId { get; }
    public Guid SchemeYearId { get; }
    public decimal Value { get; }
}