//using Xunit;
//using Desnz.Chmm.Identity.Api;
//using Desnz.Chmm.Testing.Common;
//using static Desnz.Chmm.Identity.ApiTests.Endpoints;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Hosting;
//using Desnz.Chmm.Identity.Api.Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using Desnz.Chmm.Identity.Common.Commands;
//using Desnz.Chmm.Common.Constants;
//using Desnz.Chmm.Identity.Api.Entities;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using Desnz.Chmm.Common.Mediator;
//using Desnz.Chmm.Identity.Common.Dtos;
//using Desnz.Chmm.Identity.Api.Constants;
//using Desnz.Chmm.Identity.Common.Dtos.Admin;

//namespace Desnz.Chmm.Identity.ApiTests.Controllers;

//public class UserControllerIntegrationTests : IntegrationTestsBase<LocalEntryPoint>
//{
//    private static IdentityContext _context;
//    private ChmmRole _adminRole;
//    private EditAdminUserCommand _editUserCommand;

//    private static void Configure(IWebHostBuilder builder)
//    {
//        builder.ConfigureServices(async services =>
//        {
//            var dataContextOptions = new DbContextOptionsBuilder<IdentityContext>()
//                .UseInMemoryDatabase(databaseName: "UserControllerTestsDatabase_" + Guid.NewGuid()).Options;
//            _context = new IdentityContext(dataContextOptions);
//            await new IdentityContextSeed().SeedAsync(_context);
//            services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(IdentityContext), _context));
//        });
//    }

//    public UserControllerIntegrationTests() : base(Configure)
//    {
//        _adminRole = _context.ChmmRoles.First(x => x.Name == IdentityConstants.Roles.RegulatoryOfficer);
//        _editUserCommand = new EditAdminUserCommand { Id = Guid.NewGuid(), Name = "fyfyfhf", RoleIds = new List<Guid> { _adminRole.Id } };
//    }

//    #region Expected responses

//    public class ExpectedGetUsersResponse
//    {
//        public List<ExpectedUser> Users { get; set; }
//    }

//    public class ExpectedGetUserResponse
//    {
//        public ExpectedUser User { get; set; }
//    }

//    public class ExpectedUser
//    {
//        public Guid Id { get; set; }
//        public string Name { get; set; }
//        public string Email { get; set; }
//        public IList<ExpectedRole> ChmmRoles { get; set; }
//    }

//    public class ExpectedGetRolesResponse
//    {
//        public List<ExpectedRole> Roles { get; set; }
//    }

//    public class ExpectedRole
//    {
//        public string Name { get; set; }
//    }

//    #endregion

//    #region Get Roles tests

//    [Fact]
//    public async Task Get_admin_roles_returns_multiple_200()
//    {
//        var expected = new ExpectedGetRolesResponse()
//        {
//            Roles = new List<ExpectedRole>
//            {
//                new ExpectedRole { Name = IdentityConstants.Roles.RegulatoryOfficer  },
//                new ExpectedRole { Name = IdentityConstants.Roles.SeniorTechnicalOfficer  },
//                new ExpectedRole { Name = IdentityConstants.Roles.PrincipalTechnicalOfficer  }
//            }
//        };
//        await CheckGet<IEnumerable<ExpectedRole>>(Get.AdminRoles, StatusCodes.Status200OK, expected.Roles);
//    }

//    #endregion

//    #region Get users tests
//    [Fact]
//    public async Task Get_users_returns_empty_200()
//    {
//        var expected = new ExpectedGetUsersResponse() { Users = new List<ExpectedUser>() };
//        await CheckGet<IEnumerable<ExpectedUser>>(Get.AllUsers, StatusCodes.Status200OK, expected.Users);
//    }

//    [Fact]
//    public async Task Get_unknown_user_returns_404()
//    {
//        var userId = Guid.NewGuid();
//        await CheckGet<ApiClients.Http.ProblemDetails>(Get.AdminById(userId), StatusCodes.Status404NotFound, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status404NotFound);
//            actual.Detail.Should().Be($"Could not find user with id {userId}");
//        });
//    }

//    [Fact]
//    public async Task Get_admin_users_returns_multiple_200()
//    {
//        var admin1 = new ChmmUser("Test", "abc@example.com", new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.PrincipalTechnicalOfficer) });
//        var admin2 = new ChmmUser("Test", "def@example.com", new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.RegulatoryOfficer) });
//        var admin3 = new ChmmUser("Test", "ijk@example.com", new List<ChmmRole> { new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer) });

//        await _context.ChmmUsers.AddRangeAsync(new[] { admin1, admin2, admin3 });
//        var count = await _context.SaveChangesAsync();

//        var expected = new ExpectedGetUsersResponse()
//        {
//            Users = new List<ExpectedUser>
//            {
//                new ExpectedUser { Id = admin1.Id, Name = "Test",Email = "abc@example.com", ChmmRoles = new List<ExpectedRole> { new ExpectedRole{ Name = IdentityConstants.Roles.PrincipalTechnicalOfficer } } },
//                new ExpectedUser { Id = admin2.Id, Name = "Test",Email = "def@example.com", ChmmRoles = new List<ExpectedRole> { new ExpectedRole{ Name = IdentityConstants.Roles.RegulatoryOfficer } } },
//                new ExpectedUser { Id = admin3.Id, Name = "Test",Email = "ijk@example.com", ChmmRoles = new List<ExpectedRole> { new ExpectedRole{ Name = IdentityConstants.Roles.SeniorTechnicalOfficer } } }
//            }
//        };
//        await CheckGet<IEnumerable<ExpectedUser>>(Get.AllUsers, StatusCodes.Status200OK, expected.Users);
//    }

//    [Fact]
//    public async Task Get_valid_user_returns_200()
//    {
//        var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        var result = await _context.ChmmUsers.AddAsync(storedUser);
//        var count = await _context.SaveChangesAsync();

//        await CheckGet<ExpectedUser>(Get.AdminById(storedUser.Id), StatusCodes.Status200OK, actual =>
//        {
//            actual.Should().NotBeNull();
//            actual.Id.Should().Be(storedUser.Id);
//            actual.Email.Should().Be("test@example.com");
//        });
//    }
//    #endregion

//    #region InviteUser tests
//    [Fact]
//    public async Task Invite_user_without_name_returns_400()
//    {
//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.InviteAdminUser, new InviteAdminUserCommand { Name = "", Email = "test@example.com", RoleIds = new List<Guid> { _adminRole.Id } }, StatusCodes.Status400BadRequest, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//            actual.Title.Should().Be("'Name' must not be empty.");
//        });
//    }

//    [Fact]
//    public async Task Invite_user_without_email_returns_400()
//    {
//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.InviteAdminUser, new InviteAdminUserCommand { Name = "Test", Email = "", RoleIds = new List<Guid> { _adminRole.Id } }, StatusCodes.Status400BadRequest, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//            actual.Title.Should().Be("'Email' must not be empty.");
//        });
//    }

//    [Fact]
//    public async Task Invite_existing_user_returns_400()
//    {
//        var user = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        var result = await _context.ChmmUsers.AddAsync(user);
//        var count = await _context.SaveChangesAsync();

//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.InviteAdminUser, new InviteAdminUserCommand { Name = "Test", Email = "test@example.com", RoleIds = new List<Guid> { _adminRole.Id } }, StatusCodes.Status400BadRequest, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//            actual.Detail.Should().Be("User already exists for email test@example.com");
//        });
//    }

//    [Fact]
//    public async Task Invite_user_with_no_matching_roles_returns_404()
//    {
//        var newRoleId = Guid.NewGuid();

//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.InviteAdminUser, new InviteAdminUserCommand { Name = "Test", Email = "test@example.com", RoleIds = new List<Guid> { newRoleId } }, StatusCodes.Status404NotFound, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status404NotFound);
//            actual.Detail.Should().Be($"Role(s) {newRoleId} not found");
//        });
//    }

//    [Fact]
//    public async Task Invite_user_with_some_no_matching_roles_returns_404()
//    {
//        var newRoleId = Guid.NewGuid();

//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.InviteAdminUser, new InviteAdminUserCommand { Name = "Test", Email = "test@example.com", RoleIds = new List<Guid> { _adminRole.Id, newRoleId } }, StatusCodes.Status404NotFound, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status404NotFound);
//            actual.Detail.Should().Be($"Role(s) {newRoleId} not found");
//        });
//    }

//    [Fact]
//    public async Task Invite_user_returns_200()
//    {
//        await CheckPost<CreatedResponse>(Post.InviteAdminUser, new InviteAdminUserCommand { Name = "Test", Email = "test@example.com", RoleIds = new List<Guid> { _adminRole.Id } }, StatusCodes.Status201Created, actual =>
//        {
//            actual.Location.Should().NotBeNullOrWhiteSpace();
//            bool parsed = Guid.TryParse(actual.Location, out Guid userId);
//            parsed.Should().BeTrue();
//        });
//    }

//    [Fact]
//    public async Task Get_admin_user_returns_200()
//    {

//        var user = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        var role = user.ChmmRoles.First();
//        var result = await _context.ChmmUsers.AddAsync(user);
//        var count = await _context.SaveChangesAsync();

//        var expectedResult = new AdminUserDto
//        {
//            Id = user.Id,
//            Name = user.Name,
//            Email = user.Email,
//            Status = user.Status,
//            ChmmRoles = new List<RoleDto> {
//                new RoleDto {
//                    Id = role.Id,
//                    Name = role.Name,
//                }
//            }
//        };

//        await CheckGet<AdminUserDto>(Get.AdminById(user.Id), StatusCodes.Status200OK, actual =>
//        {
//            actual.Should().BeEquivalentTo(expectedResult);
//        });
//    }

//    #endregion

//    #region EditUser
//    //[Fact]
//    //public async Task Edit_user_without_id_returns_400()
//    //{
//    //    _editUserCommand.Id = Guid.Empty;
//    //    await CheckPost<ApiClients.Http.ProblemDetails>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status400BadRequest, actual =>
//    //    {
//    //        actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//    //        actual.Title.Should().Be("'Id' must not be empty.");
//    //    });
//    //}

//    //[Fact]
//    //public async Task Edit_user_without_name_returns_400()
//    //{
//    //    _editUserCommand.Name = null;
//    //    await CheckPost<ApiClients.Http.ProblemDetails>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status400BadRequest, actual =>
//    //    {
//    //        actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//    //        actual.Title.Should().Be("One or more validation errors occurred.");
//    //        actual.Errors["Name"][0].Should().Be("The Name field is required.");
//    //    });
//    //}

//    //[Fact]
//    //public async Task Edit_user_without_roles_returns_400()
//    //{
//    //    _editUserCommand.RoleIds = null;
//    //    await CheckPost<ApiClients.Http.ProblemDetails>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status400BadRequest, actual =>
//    //    {
//    //        actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//    //        actual.Title.Should().Be("One or more validation errors occurred.");
//    //        actual.Errors["RoleIds"][0].Should().Be("The RoleIds field is required.");
//    //    });
//    //}

//    //[Fact]
//    //public async Task Edit_user_with_unknown_user_returns_404()
//    //{
//    //    _editUserCommand.Id = Guid.NewGuid();
//    //    await CheckPost<ApiClients.Http.ProblemDetails>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status404NotFound, actual =>
//    //    {
//    //        actual.Status.Should().Be(StatusCodes.Status404NotFound);
//    //        actual.Detail.Should().Be($"User not found for Id: {_editUserCommand.Id}");
//    //    });
//    //}

//    //[Fact]
//    //public async Task Edit_user_with_no_matching_roles_returns_404()
//    //{
//    //    var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//    //    var result = await _context.ChmmUsers.AddAsync(storedUser);
//    //    var count = await _context.SaveChangesAsync();

//    //    var newRoleId = Guid.NewGuid();

//    //    _editUserCommand = new EditAdminUserCommand { Id = storedUser.Id, Name = "fyfyfhf", RoleIds = new List<Guid> { newRoleId } };

//    //    await CheckPost<ApiClients.Http.ProblemDetails>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status404NotFound, actual =>
//    //    {
//    //        actual.Status.Should().Be(StatusCodes.Status404NotFound);
//    //        actual.Detail.Should().Be($"Role(s) {newRoleId} not found for Id: {_editUserCommand.Id}");
//    //    });
//    //}

//    //[Fact]
//    //public async Task Edit_user_with_some_no_matching_roles_returns_404()
//    //{
//    //    var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//    //    var result = await _context.ChmmUsers.AddAsync(storedUser);
//    //    var count = await _context.SaveChangesAsync();

//    //    var newRoleId = Guid.NewGuid();
//    //    _editUserCommand = new EditAdminUserCommand { Id = storedUser.Id, Name = "fyfyfhf", RoleIds = new List<Guid> { _adminRole.Id, newRoleId } };

//    //    await CheckPost<ApiClients.Http.ProblemDetails>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status404NotFound, actual =>
//    //    {
//    //        actual.Status.Should().Be(StatusCodes.Status404NotFound);
//    //        actual.Detail.Should().Be($"Role(s) {newRoleId} not found for Id: {_editUserCommand.Id}");
//    //    });
//    //}

//    //[Fact]
//    //public async Task Edit_user_returns_200()
//    //{
//    //    var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//    //    var result = await _context.ChmmUsers.AddAsync(storedUser);
//    //    var count = await _context.SaveChangesAsync();

//    //    _editUserCommand = new EditAdminUserCommand { Id = storedUser.Id, Name = "New Name", RoleIds = new List<Guid> { _adminRole.Id } };
//    //    await CheckPost<ActionResult>(Post.EditAdminUser, _editUserCommand, StatusCodes.Status200OK, actual =>
//    //    {
//    //    });
//    //}
//    #endregion

//    #region Activate Admin User
//    [Fact]
//    public async Task Activate_user_with_unknown_user_returns_404()
//    {
//        _editUserCommand.Id = Guid.NewGuid();
//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.ActivateAdminUser, _editUserCommand, StatusCodes.Status404NotFound, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status404NotFound);
//            actual.Detail.Should().Be($"Could not find user with id {_editUserCommand.Id}");
//        });
//    }

//    [Theory]
//    [InlineData(UsersConstants.Status.Active)]
//    [InlineData(UsersConstants.Status.Archived)]
//    [InlineData(UsersConstants.Status.Pending)]
//    public async Task Activate_user_with_incorrect_status_returns_400(string existingStatus)
//    {
//        var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        storedUser.Activate();
//        var result = await _context.ChmmUsers.AddAsync(storedUser);
//        var count = await _context.SaveChangesAsync();

//        var command = new ActivateAdminUserCommand { Id = storedUser.Id };

//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.ActivateAdminUser, command, StatusCodes.Status400BadRequest, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//            actual.Detail.Should().Be($"User with invalid status to activate: {storedUser}");
//        });
//    }

//    [Fact]
//    public async Task Activate_user_returns_204()
//    {
//        var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        storedUser.Deactivate();
//        var result = await _context.ChmmUsers.AddAsync(storedUser);
//        var count = await _context.SaveChangesAsync();

//        var command = new ActivateAdminUserCommand { Id = storedUser.Id };

//        await CheckPost<object>(Post.ActivateAdminUser, command, StatusCodes.Status204NoContent, actual =>
//        {
//        });
//    }
//    #endregion

//    #region Deactivate Admin User
//    [Fact]
//    public async Task Deactivate_user_with_unknown_user_returns_404()
//    {
//        _editUserCommand.Id = Guid.NewGuid();
//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.DeactivateAdminUser, _editUserCommand, StatusCodes.Status404NotFound, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status404NotFound);
//            actual.Detail.Should().Be($"Could not find user with id {_editUserCommand.Id}");
//        });
//    }

//    [Theory]
//    [InlineData(UsersConstants.Status.Inactive)]
//    [InlineData(UsersConstants.Status.Archived)]
//    [InlineData(UsersConstants.Status.Pending)]
//    public async Task Deactivate_user_with_incorrect_status_returns_400(string existingStatus)
//    {
//        var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        storedUser.SetStatus(existingStatus);
//        var result = await _context.ChmmUsers.AddAsync(storedUser);
//        var count = await _context.SaveChangesAsync();

//        var command = new ActivateAdminUserCommand { Id = storedUser.Id };

//        await CheckPost<ApiClients.Http.ProblemDetails>(Post.DeactivateAdminUser, command, StatusCodes.Status400BadRequest, actual =>
//        {
//            actual.Status.Should().Be(StatusCodes.Status400BadRequest);
//            actual.Detail.Should().Be($"User with invalid status to deactivate: {storedUser}");
//        });
//    }

//    [Fact]
//    public async Task Deactivate_user_returns_204()
//    {
//        var storedUser = new ChmmUser("Test", "test@example.com", new List<ChmmRole> { _adminRole });
//        storedUser.Activate();
//        var result = await _context.ChmmUsers.AddAsync(storedUser);
//        var count = await _context.SaveChangesAsync();

//        var command = new ActivateAdminUserCommand { Id = storedUser.Id };

//        await CheckPost<CreatedResponse>(Post.DeactivateAdminUser, command, StatusCodes.Status204NoContent, actual =>
//        {
//        });
//    }
//    #endregion
//}

