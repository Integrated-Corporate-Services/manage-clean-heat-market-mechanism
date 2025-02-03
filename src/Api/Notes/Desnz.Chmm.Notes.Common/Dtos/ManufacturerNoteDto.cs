using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Notes.Common.Dtos;

/// <summary>
/// Manufacturer Notes
/// </summary>
public class ManufacturerNoteDto : Entity
{
    #region Properties

    /// <summary>
    /// Organisation the note pertains to
    /// </summary>
    public Guid OrganisationId { get; set; }

    /// <summary>
    /// Scheme Year notes are for
    /// </summary>
    public Guid SchemeYearId { get; set; }

    /// <summary>
    /// The detail for the note
    /// </summary>
    public string Details { get; set; }

    #endregion Properties
}
