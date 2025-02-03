using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    /// <summary>
    /// Get all credit weightings
    /// </summary>
    public class GetAllCreditWeightingsQueryHandler : BaseRequestHandler<GetAllCreditWeightingsQuery, ActionResult<List<CreditWeightingsDto>>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="schemeYearRepository">Scheme year repository</param>
        /// <param name="mapper"></param>
        public GetAllCreditWeightingsQueryHandler(
            ILogger<BaseRequestHandler<GetAllCreditWeightingsQuery, ActionResult<List<CreditWeightingsDto>>>> logger,
            ISchemeYearRepository schemeYearRepository, IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<List<CreditWeightingsDto>>> Handle(GetAllCreditWeightingsQuery request, CancellationToken cancellationToken)
        {
            var schemeYearData = await _schemeYearRepository.GetAllCreditWeightings();

            var mappedData = _mapper.Map<List<CreditWeightingsDto>>(schemeYearData);

            return mappedData;
        }
    }
}
