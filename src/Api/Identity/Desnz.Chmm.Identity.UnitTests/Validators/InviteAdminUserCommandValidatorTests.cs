using FluentValidation.TestHelper;
using Desnz.Chmm.Identity.Api.Validators;
using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using System.Xml.Linq;

namespace Desnz.Chmm.Identity.UnitTests.Validators
{
    public class InviteAdminUserCommandValidatorTests
    {
        private readonly InviteAdminUserCommandValidator _validator;

        public InviteAdminUserCommandValidatorTests()
        {
            _validator = new InviteAdminUserCommandValidator();
        }

        #region Name
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_IsEmpty(string name)
        {
            var command = GeInviteAdminUserCommand();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var command = GeInviteAdminUserCommand();
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
            var command = GeInviteAdminUserCommand();
            command.Name = name;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }
        #endregion

        #region Email
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Email_IsEmpty(string email)
        {
            var command = GeInviteAdminUserCommand();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void Should_Have_Error_When_EmailLength_Exceeds_Maximum()
        {
            var command = GeInviteAdminUserCommand();
            command.Email = new string('*', 101);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("Joe.Bloggs@example.com")]
        [InlineData("Jean-Francois.Champollion@example.com")]
        [InlineData("AlexO'Neil@example.com")]
        public void Should_Not_Have_Error_When_Email(string email)
        {
            var command = GeInviteAdminUserCommand();
            command.Email = email;
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }
        #endregion

        #region RoleIds
        [Fact]
        public void Should_Have_Error_When_RoleIds_IsEmpty()
        {
            var command = GeInviteAdminUserCommand();
            command.RoleIds = new List<Guid>();
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.RoleIds);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RoleIds_IsNotEmpty()
        {
            var command = GeInviteAdminUserCommand();
            command.RoleIds = new List<Guid>() { Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.RoleIds);
        }

        #endregion

        private InviteAdminUserCommand GeInviteAdminUserCommand()
        {
            return new InviteAdminUserCommand();
        }
    }
    public class InviteManufacturerUserCommandValidatorTests
    {
        private readonly InviteManufacturerUserCommandValidator _validator;

        public InviteManufacturerUserCommandValidatorTests()
        {
            _validator = new InviteManufacturerUserCommandValidator();
        }

        #region Name
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_IsEmpty(string name)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), name, "email@example.com", "Job Title", "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_NameLength_Exceeds_Maximum()
        {
            var name = new string('*', 101);
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), name, "email@example.com", "Job Title", "09847587326");
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
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), name, "email@example.com", "Job Title", "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Name);
        }
        #endregion

        #region Email
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Email_IsEmpty(string email)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", email, "Job Title", "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void Should_Have_Error_When_EmailLength_Exceeds_Maximum()
        {
            var email = new string('*', 101);
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", email, "Job Title", "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("Joe.Bloggs@example.com")]
        [InlineData("Jean-Francois.Champollion@example.com")]
        [InlineData("AlexO'Neil@example.com")]
        public void Should_Not_Have_Error_When_Email(string email)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", email, "Job Title", "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.Email);
        }
        #endregion

        #region JobTitle
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_JobTitle_IsEmpty(string jobTitle)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", jobTitle, "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.JobTitle);
        }

        [Fact]
        public void Should_Have_Error_When_JobtitleLength_Exceeds_Maximum()
        {
            var jobTitle = new string('*', 101);
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", jobTitle, "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.JobTitle);
        }

        [Theory]
        [InlineData("Sales Clerk")]
        [InlineData("Production Manager")]
        [InlineData("Manager")]
        public void Should_Not_Have_Error_When_JobTitle(string jobTitle)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", jobTitle, "09847587326");
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.JobTitle);
        }
        #endregion

        #region PhoneNumber
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_PhoneNumber_IsEmpty(string phoneNumber)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", "Job title", phoneNumber);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Fact]
        public void Should_Have_Error_When_PhoneNumber_Exceeds_Maximum()
        {
            var phoneNumber = new string('1', 101);
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", "Job title", phoneNumber);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Fact]
        public void Should_Have_Error_When_PhoneNumber_LessThan_Minimum()
        {
            var phoneNumber = new string('1', 4);
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", "Job title", phoneNumber);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Theory]
        [InlineData("Aes234324")]
        public void Should_Have_Error_When_PhoneNumber_AlphaCharacters(string phoneNumber)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", "Job title", phoneNumber);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(c => c.TelephoneNumber);
        }

        [Theory]
        [InlineData("09847587326")]
        [InlineData("82940383209480")]
        public void Should_Not_Have_Error_When_PhoneNumber(string phoneNumber)
        {
            var command = new InviteManufacturerUserCommand(Guid.NewGuid(), "name", "email@example.com", "Job title", phoneNumber);
            var result = _validator.TestValidate(command);
            result
                .ShouldNotHaveValidationErrorFor(c => c.TelephoneNumber);
        }
        #endregion
    }
}