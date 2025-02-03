using AutoMapper;
using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands
{
    public class CreateLicenceHoldersCommandHandlerTests
    {
        private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;
        private readonly Mock<ILogger<CreateLicenceHoldersCommandHandler>> _mockLogger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CreateLicenceHoldersCommandHandler _handler;
        private readonly Mock<IMapper> _mockMapper;
        private List<CreateLicenceHolderCommand> _licenceHolders;

        public CreateLicenceHoldersCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<CreateLicenceHoldersCommandHandler>>();
            _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>();
            _mockMapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _mockLicenceHolderRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _handler = new CreateLicenceHoldersCommandHandler(_mockLogger.Object, _mockLicenceHolderRepository.Object, _mockMapper.Object);

            _licenceHolders = new List<CreateLicenceHolderCommand> { new CreateLicenceHolderCommand { McsManufacturerId = 1, McsManufacturerName = "Test" } };

            _mockMapper.Setup(x => x.Map<List<LicenceHolder>>(_licenceHolders)).Returns(new List<LicenceHolder> { new LicenceHolder(1, "name") });
        }

        [Fact]
        internal async Task CreateItem_Calls_Repository_Create()
        {
            //Arrange
            var guid = Guid.NewGuid();

            _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult<LicenceHolder?>(null));
            _mockLicenceHolderRepository.Setup(x => x.Create(It.IsAny<LicenceHolder>(), It.IsAny<bool>())).Returns(Task.FromResult(guid));

            var command = new CreateLicenceHoldersCommand() { LicenceHolders = _licenceHolders} ;

            //Act
            var actionResult = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<List<Guid>>>(actionResult);
            _mockLicenceHolderRepository.Verify(x => x.Append(It.IsAny<IQueryable<LicenceHolder>>()), Times.Once);
        }

        [Fact]
        internal async Task CreateItem_With_Existing_That_Is_Different_Returns_Bad_Request()
        {
            //Arrange
            var guid = Guid.NewGuid();

            var licenceHolder = new LicenceHolder(1, "Something Else");

            _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(licenceHolder));
            _mockLicenceHolderRepository.Setup(x => x.Create(It.IsAny<LicenceHolder>(), It.IsAny<bool>())).Returns(Task.FromResult(guid));

            var command = new CreateLicenceHoldersCommand() { LicenceHolders = _licenceHolders };

            //Act
            var actionResult = await _handler.Handle(command, CancellationToken.None);

            //Assert
            _mockLicenceHolderRepository.Verify(x => x.Create(It.IsAny<LicenceHolder>(), true), Times.Never);
        }
    }
}
