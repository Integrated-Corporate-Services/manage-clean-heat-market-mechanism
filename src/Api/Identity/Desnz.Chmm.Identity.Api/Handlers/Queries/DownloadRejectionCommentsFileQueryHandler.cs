using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationFileConstants;

namespace Desnz.Chmm.Identity.Api.Handlers.Queries
{
    public class DownloadRejectionCommentsFileQueryHandler : BaseRequestHandler<DownloadRejectionCommentsFileQuery, ActionResult<Stream>>
    {
        private readonly IOrganisationsRepository _organisationsRepository;
        private readonly ICurrentUserService _userService;
        private readonly IFileService _fileService;
        public DownloadRejectionCommentsFileQueryHandler(
            ILogger<BaseRequestHandler<DownloadRejectionCommentsFileQuery, ActionResult<Stream>>> logger,
            IOrganisationsRepository organisationsRepository,
            ICurrentUserService userService,
            IFileService fileService) : base(logger)
        {
            _organisationsRepository = organisationsRepository;
            _userService = userService;
            _fileService = fileService;
        }

        public override async Task<ActionResult<Stream>> Handle(DownloadRejectionCommentsFileQuery query, CancellationToken cancellationToken)
        {
            var organisationId = query.OrganisationId;

            var user = _userService.CurrentUser;

            if (user == null || user.GetUserId() == null)
                return UserNotAuthenticated();

            var oganisation = await _organisationsRepository.GetById(organisationId);
            if (oganisation == null)
                return CannotFindOrganisation(organisationId);

            var fileKey = $"{organisationId}/{query.FileName}";

            var downloadResponse = await _fileService.DownloadFileAsync(Buckets.IdentityOrganisationRejections, fileKey);

            switch (downloadResponse.ValidationError)
            {
                case null:
                    return Responses.File(downloadResponse.FileContent, downloadResponse.ContentType, query.FileName);
                case "NotFound":
                    return (ActionResult<Stream>)ErrorDownloadingFile(string.Format("Could not download an Rejection Comments File with name: {0} for organisation with Id {1}", query.FileName, query.OrganisationId));
                default:
                    return (ActionResult<Stream>)Responses.BadRequest(downloadResponse.ValidationError);
            }
        }
    }
}
