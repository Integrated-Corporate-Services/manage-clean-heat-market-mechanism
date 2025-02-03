using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Commands;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class EditAdminUserCommandValidatorTests
    {
        private readonly EditAdminUserCommandValidator _validator;

        public EditAdminUserCommandValidatorTests()
        {
            _validator = new EditAdminUserCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Id_IsEmpty(string inString)
        {
            var command = GeEditAdminUserCommand();
            command.Name = "A name";

            Guid guidFromString;
            var guid = Guid.TryParse(inString, out guidFromString);

            command.Id = guidFromString;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Id);
        }

        #region Name
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_IsEmpty(string name)
        {
            var command = GeEditAdminUserCommand();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var command = GeEditAdminUserCommand();
            command.Name = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData("Joe Bloggs")]
        [InlineData("Jean-Francois Champollion")]
        [InlineData("Alex O'Neil")]
        public void Should_Not_Have_Error_When_Name(string name)
        {
            var command = GeEditAdminUserCommand();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }
        #endregion

        #region RoleIds
        [Fact]
        public void Should_Have_Error_When_RoleIds_IsEmpty()
        {
            var command = GeEditAdminUserCommand();
            command.RoleIds = new List<Guid>();
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.RoleIds);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RoleIds_IsNotEmpty()
        {
            var command = GeEditAdminUserCommand();
            command.RoleIds = new List<Guid>() { Guid.NewGuid()};
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.RoleIds);
        }

        #endregion

        private EditAdminUserCommand GeEditAdminUserCommand()
        {
            return new EditAdminUserCommand();
        }
    }
}