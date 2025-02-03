using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Identity.Api.Entities;

/// <summary>
/// Organisation structure file
/// </summary>
public class OrganisationStructureFile : Entity
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

    #endregion

    #region Constructors

    public OrganisationStructureFile() : base() { }

    public OrganisationStructureFile(AwsFileDto file, string? createdBy = null) : base(createdBy)
    {
        FileKey = file.FileKey;
        FileName = file.FileName;
    }

    public OrganisationStructureFile(string fileName, string fileKey, Guid organisationId, string? createdBy = null): base(createdBy)
    {
        FileKey = fileKey;
        FileName = fileName;
        OrganisationId = organisationId;
    }

    #endregion
}
