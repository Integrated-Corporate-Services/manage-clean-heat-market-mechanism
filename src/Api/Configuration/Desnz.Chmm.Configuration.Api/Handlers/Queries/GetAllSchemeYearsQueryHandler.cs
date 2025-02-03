using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetAllSchemeYearsQueryHandler : BaseRequestHandler<GetAllSchemeYearsQuery, ActionResult<List<SchemeYearDto>>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="schemeYearRepository">Scheme year repository</param>
        /// <param name="mapper"></param>
        public GetAllSchemeYearsQueryHandler(
            ILogger<BaseRequestHandler<GetAllSchemeYearsQuery, ActionResult<List<SchemeYearDto>>>> logger,
            ISchemeYearRepository schemeYearRepository,
            IMapper mapper) : base(logger) 
        {
            _schemeYearRepository = schemeYearRepository;
            _mapper = mapper;
        }

        public override async Task<ActionResult<List<SchemeYearDto>>> Handle(GetAllSchemeYearsQuery request, CancellationToken cancellationToken)
        {
            var schemeYearData = await _schemeYearRepository.GetAllSchemeYears();

            var mappedData = _mapper.Map<List<SchemeYearDto>>(schemeYearData);

            return mappedData;
        }
    }
}

