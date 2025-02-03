using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Common.Commands;

/// <summary>
/// Command to tranfer credits between organisations
/// </summary>
public class TransferCreditsCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="organisationId">The organisation initiating the transfer</param>
    /// <param name="destinationOrganisationId">The destination of the credit transfer</param>
    /// <param name="schemeYearId">The scheme year of the credit transfer</param>
    /// <param name="value">How many credits are being transferred</param>
    public TransferCreditsCommand(Guid organisationId, Guid destinationOrganisationId, Guid schemeYearId, decimal value)
    {
        OrganisationId = organisationId;
        DestinationOrganisationId = destinationOrganisationId;
        SchemeYearId = schemeYearId;
        Value = value;
    }

    /// <summary>
    /// The organisation initiating the transfer
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// The organisation receiving the credits
    /// </summary>
    public Guid DestinationOrganisationId { get; private set; }

    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// The value of the transfer
    /// </summary>
    public decimal Value { get; private set; }
}
