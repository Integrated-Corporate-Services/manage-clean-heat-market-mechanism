using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetCurrentSchemeYearQueryHandler : BaseRequestHandler<GetCurrentSchemeYearQuery, ActionResult<SchemeYearDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMapper _mapper;

        public GetCurrentSchemeYearQueryHandler(
            ILogger<BaseRequestHandler<GetCurrentSchemeYearQuery, ActionResult<SchemeYearDto>>> logger,
            ISchemeYearRepository schemeYearRepository, 
            IDateTimeProvider dateTimeProvider, 
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _dateTimeProvider = dateTimeProvider;
            _mapper = mapper;
        }

        public override async Task<ActionResult<SchemeYearDto>> Handle(GetCurrentSchemeYearQuery request, CancellationToken cancellationToken)
        {
            var date = _dateTimeProvider.UtcDateNow;
            var schemeYearData = await _schemeYearRepository.GetSchemeYear(i => date >= i.StartDate && date <= i.EndDate);

            var mappedData = _mapper.Map<SchemeYearDto>(schemeYearData);

            return mappedData;
        }
    }
}

