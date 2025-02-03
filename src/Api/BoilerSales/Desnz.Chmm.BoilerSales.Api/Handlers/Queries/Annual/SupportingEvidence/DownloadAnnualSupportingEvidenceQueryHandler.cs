using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Annual.SupportingEvidence;

public class DownloadAnnualSupportingEvidenceQueryHandler : FileDownloadBase, IRequestHandler<DownloadAnnualSupportingEvidenceQuery, ActionResult<Stream>>
{
    public DownloadAnnualSupportingEvidenceQueryHandler(
        IFileService fileService,
        IRequestValidator requestValidator) : base(fileService, requestValidator)
    {
    }

    public async Task<ActionResult<Stream>> Handle(DownloadAnnualSupportingEvidenceQuery query, CancellationToken cancellationToken)
    {
        var fileKey = $"{query.OrganisationId}/{query.SchemeYearId}/";
        if (query.IsEditing) fileKey += "edit/";
        fileKey += query.FileName;

        return await FileDownload(query.OrganisationId, query.FileName, Buckets.AnnualSupportingEvidence, fileKey);
    }
}
