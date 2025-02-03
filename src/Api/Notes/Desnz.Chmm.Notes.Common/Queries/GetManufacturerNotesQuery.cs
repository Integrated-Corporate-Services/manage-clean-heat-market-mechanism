using Desnz.Chmm.Notes.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Queries;

/// <summary>
/// Defines a query to return all notes for a manufacturer
/// </summary>
public class GetManufacturerNotesQuery : IRequest<ActionResult<List<ManufacturerNoteDto>>>
{
    /// <summary>
    /// Init query
    /// claims on any specific manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to retrieve data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve data for</param>
    public GetManufacturerNotesQuery(Guid? organisationId, Guid? schemeYearId)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
    }

    /// <summary>
    /// The organisation to get the notes for
    /// </summary>
    public Guid? OrganisationId { get; private set; }

    /// <summary>
    /// The scheme year to get the notes for
    /// </summary>
    public Guid? SchemeYearId { get; private set; }
}
