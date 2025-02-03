using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Commands;

/// <summary>
/// Command for creating a Manufacturer Note
/// </summary>
public class AddManufacturerNoteCommand : IRequest<ActionResult<Guid>>
{
    /// <summary>
    /// The ID of the Organisation to create the note for
    /// </summary>
    public Guid OrganisationId { get; set; }

    /// <summary>
    /// The ID of the Scheme Year to create the note for
    /// </summary>
    public Guid SchemeYearId { get; set; }

    /// <summary>
    /// The detail for the note
    /// </summary>
    public string Details { get; set; }
}
