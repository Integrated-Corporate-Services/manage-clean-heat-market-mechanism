using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Identity.Api.Entities;

/// <summary>
/// Organisation approval file
/// </summary>
public class OrganisationDecisionFile : Entity
{
    #region Properties

    /// <summary>
    /// Organisation file is for
    /// </summary>
    public Organisation Organisation { get; private set; }

    /// <summary>
    /// Id of organisation file belongs to
    /// </summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>
    /// File key (AWS reference)
    /// </summary>
    public string FileKey { get; private set; }

    /// <summary>
    /// Name of file
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Decision: Approve, Reject
    /// </summary>
    public string Decision { get; private set; }

    #endregion

    #region Constructors

    public OrganisationDecisionFile() : base() { }

    public OrganisationDecisionFile(AwsFileDto file, string? createdBy = null) : base(createdBy)
    {
        FileKey = file.FileKey;
        FileName = file.FileName;
    }

    public OrganisationDecisionFile(string fileName, string fileKey, Guid organisationId, string decision, string? createdBy = null) : base(createdBy)
    {
        FileName = fileName;
        FileKey = fileKey;
        OrganisationId = organisationId;
        Decision = decision;
    }

    #endregion
}