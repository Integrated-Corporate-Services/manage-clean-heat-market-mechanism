using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    /// <summary>
    /// Handler for the Create Licence Holders command
    /// </summary>
    public class CreateLicenceHoldersCommandHandler : BaseRequestHandler<CreateLicenceHoldersCommand, ActionResult<List<Guid>>>
    {
        private readonly ILicenceHolderRepository _licenceHoldersRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CreateLicenceHoldersCommandHandler(
        ILogger<BaseRequestHandler<CreateLicenceHoldersCommand, ActionResult<List<Guid>>>> logger,
        ILicenceHolderRepository licenceHoldersRepository,
        IMapper mapper) : base(logger)
        {
            _licenceHoldersRepository = licenceHoldersRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<ActionResult<List<Guid>>> Handle(CreateLicenceHoldersCommand request, CancellationToken cancellationToken)
        {
            var licenceHolders = _mapper.Map<List<Entities.LicenceHolder>>(request.LicenceHolders);
            await _licenceHoldersRepository.Append(licenceHolders.AsQueryable());

            return Responses.Created(licenceHolders.Select(x => x.Id));
        }
    }
}
