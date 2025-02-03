using Desnz.Chmm.BoilerSales.Api.Extensions;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Quarterly.SupportingEvidence;

public class DownloadQuarterlySupportingEvidenceQueryHandler : FileDownloadBase, IRequestHandler<DownloadQuarterlySupportingEvidenceQuery, ActionResult<Stream>>
{
    public DownloadQuarterlySupportingEvidenceQueryHandler(
        IFileService fileService,
        IRequestValidator requestValidator) : base(fileService, requestValidator)
    { }

    public async Task<ActionResult<Stream>> Handle(DownloadQuarterlySupportingEvidenceQuery query, CancellationToken cancellationToken)
    {
        return await FileDownload(query.OrganisationId, query.FileName, Buckets.QuarterlySupportingEvidence, query.FileName.BuildObjectKeyForQuarterlyBoilerSales(query.OrganisationId, query.SchemeYearId, query.SchemeYearQuarterId));
    }
}
