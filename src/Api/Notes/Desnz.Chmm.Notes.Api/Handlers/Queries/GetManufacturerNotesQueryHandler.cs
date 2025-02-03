using AutoMapper;
using Desnz.Chmm.Notes.Common.Dtos;
using Desnz.Chmm.Notes.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Notes.Api.Handlers.Queries;

/// <summary>
/// Handles a GetManufacturerNotesQuery
/// </summary>
public class GetManufacturerNotesQueryHandler : BaseRequestHandler<GetManufacturerNotesQuery, ActionResult<List<ManufacturerNoteDto>>>
{
    private readonly IMapper _mapper;
    private readonly IManufacturerNotesRepository _manufacturerNotesRepository;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor taking all arguments
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="mapper">The object mapper</param>
    /// <param name="manufacturerNotesRepository">The repository for accessing manufacturer notes</param>
    public GetManufacturerNotesQueryHandler(
        ILogger<BaseRequestHandler<GetManufacturerNotesQuery, ActionResult<List<ManufacturerNoteDto>>>> logger,
        IMapper mapper,
        IManufacturerNotesRepository manufacturerNotesRepository,
        IRequestValidator requestValidator) : base(logger)
    {
        _mapper = mapper;
        _manufacturerNotesRepository = manufacturerNotesRepository;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles the query
    /// </summary>
    /// <param name="request">The get notes request to handle</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    public override async Task<ActionResult<List<ManufacturerNoteDto>>> Handle(GetManufacturerNotesQuery request, CancellationToken cancellationToken)
    {
        var organisationId = request.OrganisationId;
        var schemeYearId = request.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        var manufacturerNotes = (await _manufacturerNotesRepository.GetAll(o =>
            o.OrganisationId == request.OrganisationId && o.SchemeYearId == request.SchemeYearId)).OrderByDescending(x => x.CreationDate);

        return _mapper.Map<List<ManufacturerNoteDto>>(manufacturerNotes);
    }
}
