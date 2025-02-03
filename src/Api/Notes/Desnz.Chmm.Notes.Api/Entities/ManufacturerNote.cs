using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Notes.Api.Entities;

/// <summary>
/// Notes for an organisation
/// </summary>
public class ManufacturerNote : Entity
{
    #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    protected ManufacturerNote() : base() { }

    /// <summary>
    /// Constructor taking all fields
    /// </summary>
    /// <param name="organisationId">The ID of the organisation the note corresponds to</param>
    /// <param name="schemeYearId">The ID of the scheme year the note corresponds to</param>
    /// <param name="details">The note details</param>
    /// <param name="createdBy">The creator of the record</param>
    public ManufacturerNote(Guid organisationId, Guid schemeYearId, string details, string? createdBy = null) : base(createdBy)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        Details = details;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Organisation notes are for
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// Scheme Year notes are for
    /// </summary>
    public Guid SchemeYearId { get; private set; }

    /// <summary>
    /// The detail for the note
    /// </summary>
    public string Details { get; private set; }

    #endregion
}
