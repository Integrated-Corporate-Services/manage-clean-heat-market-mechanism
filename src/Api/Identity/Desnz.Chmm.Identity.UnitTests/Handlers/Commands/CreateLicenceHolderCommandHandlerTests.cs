using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands
{
    public class CreateLicenceHolderCommandHandlerTests
    {
        private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;
        private readonly Mock<ILogger<CreateLicenceHolderCommandHandler>> _mockLogger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CreateLicenceHolderCommandHandler _handler;

        public CreateLicenceHolderCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<CreateLicenceHolderCommandHandler>>();
            _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _mockLicenceHolderRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            _handler = new CreateLicenceHolderCommandHandler(_mockLogger.Object, _mockLicenceHolderRepository.Object);
        }

        [Fact]
        internal async Task CreateItem_Calls_Repository_Create()
        {
            //Arrange
            var guid = Guid.NewGuid();

            _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult<LicenceHolder?>(null));
            _mockLicenceHolderRepository.Setup(x => x.Create(It.IsAny<LicenceHolder>(), It.IsAny<bool>())).Returns(Task.FromResult(guid));

            var command = new CreateLicenceHolderCommand() { McsManufacturerId = 1, McsManufacturerName = "Test" };

            //Act
            var actionResult = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<Guid>>(actionResult);
            _mockLicenceHolderRepository.Verify(x => x.Create(It.IsAny<LicenceHolder>(), true), Times.Once);
        }

        [Fact]
        internal async Task CreateItem_With_Existing_Does_Not_Call_Repository_Create()
        {
            //Arrange
            var guid = Guid.NewGuid();

            var licenceHolder = new LicenceHolder(1, "Test");

            _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(licenceHolder));
            _mockLicenceHolderRepository.Setup(x => x.Create(It.IsAny<LicenceHolder>(), It.IsAny<bool>())).Returns(Task.FromResult(guid));

            var command = new CreateLicenceHolderCommand() { McsManufacturerId = 1, McsManufacturerName = "Test" };

            //Act
            var actionResult = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<Guid>>(actionResult);
            _mockLicenceHolderRepository.Verify(x => x.Create(It.IsAny<LicenceHolder>(), true), Times.Never);
        }

        [Fact]
        internal async Task CreateItem_With_Existing_That_Is_Different_Returns_Bad_Request()
        {
            //Arrange
            var guid = Guid.NewGuid();

            var licenceHolder = new LicenceHolder(1, "Something Else");

            _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(licenceHolder));
            _mockLicenceHolderRepository.Setup(x => x.Create(It.IsAny<LicenceHolder>(), It.IsAny<bool>())).Returns(Task.FromResult(guid));

            var expectedResult = Responses.BadRequest($"Cannot change Licence Holder with Id: {licenceHolder.Id} from: {licenceHolder.Name} to: Test");

            var command = new CreateLicenceHolderCommand() { McsManufacturerId = 1, McsManufacturerName = "Test" };

            //Act
            var actionResult = await _handler.Handle(command, CancellationToken.None);

            //Assert
            actionResult.Result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
