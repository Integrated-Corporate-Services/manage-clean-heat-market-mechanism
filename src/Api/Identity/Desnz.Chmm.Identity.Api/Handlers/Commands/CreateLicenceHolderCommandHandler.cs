using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Api.Handlers.Commands
{
    /// <summary>
    /// Handler for the Create Licence Holder command
    /// </summary>
    public class CreateLicenceHolderCommandHandler : BaseRequestHandler<CreateLicenceHolderCommand, ActionResult<Guid>>
    {
        private readonly ILicenceHolderRepository _licenceHoldersRepository;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CreateLicenceHolderCommandHandler(
        ILogger<BaseRequestHandler<CreateLicenceHolderCommand, ActionResult<Guid>>> logger,
        ILicenceHolderRepository licenceHoldersRepository) : base(logger)
        {
            _licenceHoldersRepository = licenceHoldersRepository;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<ActionResult<Guid>> Handle(CreateLicenceHolderCommand request, CancellationToken cancellationToken)
        {
            var existing = await _licenceHoldersRepository.Get(h => h.McsManufacturerId == request.McsManufacturerId);

            // If this item exists and the name is the same, just return the Id
            if (existing != null && existing.Name == request.McsManufacturerName)
                return existing.Id;

            if (existing != null && existing.Name != request.McsManufacturerName)
                return CannotChangeMcsManufacturerName(existing.Id, existing.Name, request.McsManufacturerName);

            var licenceHolder = new LicenceHolder(request.McsManufacturerId, request.McsManufacturerName);
            var licenceHolderId = await _licenceHoldersRepository.Create(licenceHolder);

            return Responses.Created(licenceHolderId);
        }
    }
}