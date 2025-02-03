using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries
{
    public class FileDownloadBase
    {
        private readonly IFileService _fileService;
        private readonly IRequestValidator _requestValidator;

        public FileDownloadBase(
            IFileService fileService, 
            IRequestValidator requestValidator)
        {
            _fileService = fileService;
            _requestValidator = requestValidator;
        }

        public async Task<ActionResult<Stream>> FileDownload(Guid organisationId, string fileName, string bucketName, string key)
        {
            var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
                organisationId: organisationId);
            if (validationError != null)
                return validationError;

            var downloadResponse = await _fileService.DownloadFileAsync(bucketName, key);
            if (downloadResponse.ValidationError != null)
            {
                return Responses.BadRequest(downloadResponse.ValidationError);
            }

            return Responses.File(downloadResponse.FileContent, downloadResponse.ContentType, fileName);
        }
    }
}