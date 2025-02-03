using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Desnz.Chmm.Common.Services.FileService;

namespace Desnz.Chmm.BoilerSales.Api.Services;

public class BoilerSalesFileCopyService : IBoilerSalesFileCopyService
{
    private readonly IFileService _fileService;

    public record ConcludedEditingSession(IReadOnlyList<string> VerificationStatements, IReadOnlyList<string> SupportingEvidences, IReadOnlyList<string> Errors);

    public record ConcludedEditingQuarterlySession(IReadOnlyList<string> SupportingEvidences, IReadOnlyList<string> Errors);


    public BoilerSalesFileCopyService(IFileService fileService)
    {
        _fileService = fileService;
    }

    private static string GetOriginalPrefix(Guid organisationId, Guid schemeYearId) => $"{organisationId}/{schemeYearId}";

    private static string GetOriginalPrefix(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId) => $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";

    private static string GetEditPrefix(Guid organisationId, Guid schemeYearId) => $"{organisationId}/{schemeYearId}/edit";

    private static string GetEditPrefix(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId) => $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}/edit";


    async Task<List<string?>> IBoilerSalesFileCopyService.PrepareForEditing(Guid organisationId, Guid schemeYearId)
    {
        var editPrefix = GetEditPrefix(organisationId, schemeYearId);
        var originalPrefix = GetOriginalPrefix(organisationId, schemeYearId);

        return await PrepareForEditingAnnual(editPrefix, originalPrefix);
    }

    async Task<List<string?>> IBoilerSalesFileCopyService.PrepareForEditing(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId)
    {
        var editPrefix = GetEditPrefix(organisationId, schemeYearId, schemeYearQuarterId);
        var originalPrefix = GetOriginalPrefix(organisationId, schemeYearId, schemeYearQuarterId);

        return await PrepareForEditingQuarterly(editPrefix, originalPrefix);
    }

    async Task<ConcludedEditingSession> IBoilerSalesFileCopyService.ConcludeEditing(Guid organisationId, Guid schemeYearId)
    {
        var editPrefix = GetEditPrefix(organisationId, schemeYearId);
        var originalPrefix = GetOriginalPrefix(organisationId, schemeYearId);

        return await ConcludeEditingAnnual(editPrefix, originalPrefix);
    }

    async Task<ConcludedEditingQuarterlySession> IBoilerSalesFileCopyService.ConcludeEditing(Guid organisationId, Guid schemeYearId, Guid schemeYearQuarterId)
    {
        var editPrefix = GetEditPrefix(organisationId, schemeYearId, schemeYearQuarterId);
        var originalPrefix = GetOriginalPrefix(organisationId, schemeYearId, schemeYearQuarterId);

        return await ConcludeEditingQuarterly(editPrefix, originalPrefix);
    }

    #region Private methods

    private async Task<IReadOnlyList<string>> GetFiles(string bucket, string prefix)
    {
        var files = await _fileService.GetFileNamesAsync(bucket, prefix);
        return files.Distinct().ToList();
    }

    private async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetAnnualFiles(string prefix)
    {
        var verificationStatementsTask = Task.Run(() => GetFiles(Buckets.AnnualVerificationStatement, prefix));
        var supportingEvidenceTask = Task.Run(() => GetFiles(Buckets.AnnualSupportingEvidence, prefix));
        await Task.WhenAll(verificationStatementsTask, supportingEvidenceTask);
        return new Dictionary<string, IReadOnlyList<string>>
        {
            { Buckets.AnnualVerificationStatement, verificationStatementsTask.Result },
            { Buckets.AnnualSupportingEvidence, supportingEvidenceTask.Result }
        };
    }

    private async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetQuarterlyFiles(string prefix)
    {
        var files = await GetFiles(Buckets.QuarterlySupportingEvidence, prefix);
        return new Dictionary<string, IReadOnlyList<string>>
        {
            { Buckets.QuarterlySupportingEvidence, files },
        };
    }

    private async Task<IEnumerable<FileUploadResponse>> DeleteFiles(string prefix, IReadOnlyDictionary<string, IReadOnlyList<string>> buckets)
    {
        var deleteFileTasks = buckets.SelectMany(bucket => bucket.Value.Select(
            fileToDelete => Task.Run(() => _fileService.DeleteObjectNonVersionedBucketAsync(
                bucket.Key, $"{prefix}/{fileToDelete}"))));
        var responses = await Task.WhenAll(deleteFileTasks);
        return responses;
    }

    private async Task<IEnumerable<FileCopyResponse>> CopyFiles(string sourcePrefix, string destinationPrefix, IReadOnlyDictionary<string, IReadOnlyList<string>> buckets)
    {
        var copyFileTasks = buckets.SelectMany(bucket => bucket.Value.Select(
            fileToCopy => Task.Run(() => _fileService.CopyFileAsync(
                bucket.Key, $"{sourcePrefix}/{fileToCopy}",
                bucket.Key, $"{destinationPrefix}/{fileToCopy}"))));
        var responses = await Task.WhenAll(copyFileTasks);
        return responses;
    }

    private async Task<List<string?>> PrepareForEditingAnnual(string editPrefix, string originalPrefix)
    {
        var originalFiles = await GetAnnualFiles(originalPrefix);
        var editFiles = await GetAnnualFiles(editPrefix);

        return await DeleteAndCopyFiles(editPrefix, originalPrefix, originalFiles, editFiles);
    }

    private async Task<List<string?>> PrepareForEditingQuarterly(string editPrefix, string originalPrefix)
    {
        var originalFiles = await GetQuarterlyFiles(originalPrefix);
        var editFiles = await GetQuarterlyFiles(editPrefix);

        return await DeleteAndCopyFiles(editPrefix, originalPrefix, originalFiles, editFiles);
    }

    private async Task<List<string?>> DeleteAndCopyFiles(string editPrefix, string originalPrefix, IReadOnlyDictionary<string, IReadOnlyList<string>> originalFiles, IReadOnlyDictionary<string, IReadOnlyList<string>> editFiles)
    {
        var deleteFileResults = await DeleteFiles(editPrefix, editFiles);
        if (deleteFileResults.Any(x => !string.IsNullOrWhiteSpace(x.ValidationError)))
        {
            return deleteFileResults.Select(x => x.ValidationError).ToList();
        }

        var copyFileResults = await CopyFiles(originalPrefix, editPrefix, originalFiles);
        if (copyFileResults.Any(x => !string.IsNullOrWhiteSpace(x.ValidationError)))
        {
            return copyFileResults.Select(x => x.ValidationError).ToList();
        }

        return new List<string?>();
    }

    private async Task<ConcludedEditingSession> ConcludeEditingAnnual(string editPrefix, string originalPrefix)
    {
        var originalFiles = await GetAnnualFiles(originalPrefix);
        var editFiles = await GetAnnualFiles(editPrefix);

        var errors = await ConcludeEditingDeleteAndCopyFiles(editPrefix, originalPrefix, originalFiles, editFiles);

        return errors.Any()
            ? new ConcludedEditingSession(
                null,
                null,
                errors
            )
            : new ConcludedEditingSession(
                editFiles.First(x => x.Key == Buckets.AnnualVerificationStatement).Value,
                editFiles.First(x => x.Key == Buckets.AnnualSupportingEvidence).Value,
                errors
        );
    }

    private async Task<ConcludedEditingQuarterlySession> ConcludeEditingQuarterly(string editPrefix, string originalPrefix)
    {
        var originalFiles = await GetQuarterlyFiles(originalPrefix);
        var editFiles = await GetQuarterlyFiles(editPrefix);

        var errors = await ConcludeEditingDeleteAndCopyFiles(editPrefix, originalPrefix, originalFiles, editFiles);

        return errors.Any()
            ? new ConcludedEditingQuarterlySession(
                null,
                errors
            )
            : new ConcludedEditingQuarterlySession(
                editFiles.First(x => x.Key == Buckets.QuarterlySupportingEvidence).Value,
                errors
        );
    }

    private async Task<List<string?>> ConcludeEditingDeleteAndCopyFiles(string editPrefix, string originalPrefix, IReadOnlyDictionary<string, IReadOnlyList<string>> originalFiles, IReadOnlyDictionary<string, IReadOnlyList<string>> editFiles)
    {
        var deleteFileResults = await DeleteFiles(originalPrefix, originalFiles);
        if (deleteFileResults.Any(x => !string.IsNullOrWhiteSpace(x.ValidationError) && x.ValidationError != "The selected file does not exists"))
        {
            return deleteFileResults.Select(x => x.ValidationError).ToList();
        }

        var copyFileResults = await CopyFiles(editPrefix, originalPrefix, editFiles);
        if (copyFileResults.Any(x => !string.IsNullOrWhiteSpace(x.ValidationError)))
        {
            return copyFileResults.Select(x => x.ValidationError).ToList();
        }

        return new List<string?>();
    }

    #endregion
}
