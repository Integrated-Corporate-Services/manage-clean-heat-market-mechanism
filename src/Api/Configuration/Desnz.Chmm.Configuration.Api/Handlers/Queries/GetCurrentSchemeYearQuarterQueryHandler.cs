using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetCurrentSchemeYearQuarterQueryHandler : BaseRequestHandler<GetCurrentSchemeYearQuarterQuery, ActionResult<SchemeYearQuarterDto>>
    {
        private readonly ISchemeYearRepository _schemeYearRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMapper _mapper;

        public GetCurrentSchemeYearQuarterQueryHandler(
            ILogger<BaseRequestHandler<GetCurrentSchemeYearQuarterQuery, ActionResult<SchemeYearQuarterDto>>> logger,
            ISchemeYearRepository schemeYearRepository, 
            IDateTimeProvider dateTimeProvider, 
            IMapper mapper) : base(logger)
        {
            _schemeYearRepository = schemeYearRepository;
            _dateTimeProvider = dateTimeProvider;
            _mapper = mapper;
        }

        public override async Task<ActionResult<SchemeYearQuarterDto>> Handle(GetCurrentSchemeYearQuarterQuery request, CancellationToken cancellationToken)
        {
            var date = _dateTimeProvider.UtcDateNow;
            var schemeYearData = _schemeYearRepository.GetSchemeYearQuarter(i => date >= i.StartDate && date <= i.EndDate);

            var mappedData = _mapper.Map<SchemeYearQuarterDto>(schemeYearData);

            return mappedData;
        }
    }
}

