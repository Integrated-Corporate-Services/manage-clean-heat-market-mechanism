using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Annual.VerificationStatement;

public class DownloadAnnualVerificationStatementQueryHandler : FileDownloadBase, IRequestHandler<DownloadAnnualVerificationStatementQuery, ActionResult<Stream>>
{
    public DownloadAnnualVerificationStatementQueryHandler(
        IFileService fileService,
        IRequestValidator requestValidator) : base(fileService, requestValidator)
    {
    }

    public async Task<ActionResult<Stream>> Handle(DownloadAnnualVerificationStatementQuery query, CancellationToken cancellationToken)
    {
        var fileKey = $"{query.OrganisationId}/{query.SchemeYearId}/";
        if (query.IsEditing) fileKey += "edit/";
        fileKey += query.FileName;

        return await FileDownload(query.OrganisationId, query.FileName, Buckets.AnnualVerificationStatement, fileKey);
    }
}
