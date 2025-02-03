namespace Desnz.Chmm.Notes.Api.Extensions;

public static class NotesFileExtensions
{
    #region New note

    public static string BuildObjectKeyForFileForNewNote(this IFormFile file, Guid organisationId, Guid schemeYearId, Guid userId)
        => file.FileName.BuildObjectKeyForFileForNewNote(organisationId, schemeYearId, userId);

    public static string BuildObjectKeyForFileForNewNote(this string fileName, Guid organisationId, Guid schemeYearId, Guid userId)
        => $"temp/{organisationId}/{schemeYearId}/{userId}/{fileName}";

    public static string GetObjectKeyPrefixForNewNote(Guid organisationId, Guid schemeYearId, Guid userId)
        => $"temp/{organisationId}/{schemeYearId}/{userId}";

    #endregion

    #region Existing note

    public static string BuildObjectKeyForFileForExistingNote(this string fileName, Guid noteId)
        => $"{noteId}/{fileName}";

    public static string GetObjectKeyPrefixForExistingNote(Guid noteId)
        => $"{noteId}";

    #endregion
}
