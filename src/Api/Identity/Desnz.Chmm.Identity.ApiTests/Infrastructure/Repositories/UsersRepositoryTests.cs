//using Desnz.Chmm.Common.Constants;
//using Desnz.Chmm.Identity.Api.Infrastructure;
//using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
//using Desnz.Chmm.Identity.Common.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;

//namespace Desnz.Chmm.Identity.ApiTests.Infrastructure.Repositories
//{

//    public class UsersRepositoryTests
//    {
//        private DbContextOptions<IdentityContext> _identityDataContextOptions = new DbContextOptionsBuilder<IdentityContext>()
//               .UseInMemoryDatabase(databaseName: "UsersRepositoryTestDatabase_" + Guid.NewGuid())
//               .Options;

//        private Mock<ILogger<UsersRepository>> _mockLogger;
//        private ChmmUser _user1;
//        private ChmmUser _user2;
//        private IdentityContext _identityDataContext;
//        private UsersRepository _repository;

//        public UsersRepositoryTests()
//        {
//            _mockLogger = new Mock<ILogger<UsersRepository>>();

//            _user1 = new ChmmUser("User One", "user1@example.com", null, new List<ChmmRole> { new ChmmRole("Role One") });
//            _user2 = new ChmmUser("User Two", "user2@example.com", null, new List<ChmmRole> { new ChmmRole("Role Two") });

//            _identityDataContext = GetNewInMemoryContext();
//            PopulateContext(_identityDataContext, new List<ChmmUser>() { _user1, _user2 });

//            _repository = new UsersRepository(_identityDataContext);
//        }

//        [Fact]
//        public async Task UsersRepository_CanGetAllUnfiltered()
//        {
//            //Arrange

//            //Act
//            var result = await _repository.Get();

//            //Assert
//            Assert.Equal(2, result.Count);
//            Assert.Equal("Role One", result.First(x => x.Email.Contains("user1")).ChmmRoles.First().Name);
//            Assert.Equal("Role Two", result.First(x => x.Email.Contains("user2")).ChmmRoles.First().Name);
//        }


//        [Fact]
//        public async Task UsersRepository_CanGetAllFiltered()
//        {
//            //Arrange

//            //Act
//            var result = await _repository.Get(x => x.Name.Contains("Two"));

//            //Assert
//            Assert.Single(result);
//            Assert.Equal("User Two", result[0].Name);
//        }



//        [Fact]
//        public async Task UsersRepository_GetUserById_IncludingRoles()
//        {
//            //Arrange

//            //Act
//            var result = await _repository.Get(_user1.Id);

//            //Assert
//            Assert.NotNull(result);
//            Assert.Single(result.ChmmRoles);
//        }


//        [Fact]
//        public async Task UsersRepository_GetUserById_NoRolesIncluded()
//        {
//            //Arrange

//            //Act
//            var result = await _repository.GetUserById(_user1.Id, includeRoles: false);

//            //Assert
//            Assert.NotNull(result);
//            Assert.Null(result.ChmmRoles);
//        }


//        [Fact]
//        public async Task UsersRepository_GetUserById_WithTracking_Off()
//        {
//            //Arrange
//            var result = await _repository.GetUserById(_user1.Id, includeRoles: true);
//            result.AddRole(AddNewRole());

//            //Act
//            var count = await _repository.UnitOfWork.SaveChangesAsync();

//            //Assert
//            Assert.Equal(0, count);

//            result = await _repository.GetUserById(_user1.Id, includeRoles: true);
//            Assert.Single(result.ChmmRoles);
//        }


//        [Fact]
//        public async Task UsersRepository_GetUserById_WithTracking_On()
//        {
//            //Arrange
//            var result = await _repository.GetUserById(_user1.Id, includeRoles: true, withTracking: true);
//            result.AddRole(AddNewRole());

//            //Act
//            var count = await _repository.UnitOfWork.SaveChangesAsync();

//            //Assert
//            Assert.Equal(1, count);

//            result = await _repository.GetUserById(_user1.Id, includeRoles: true);
//            Assert.Equal(2, result.ChmmRoles.Count);
//        }

//        private ChmmRole AddNewRole()
//        {
//            var role = new ChmmRole("New Role");
//            _identityDataContext.Append(new List<ChmmRole> { role });
//            _identityDataContext.SaveChanges();
//            return role;
//        }

//        [Fact]
//        public async Task UsersRepository_GetUserByEmailReadOnly_IncludingRoles()
//        {
//            //Arrange

//            //Act
//            var result = await _repository.GetUserByEmail(_user1.Email);

//            //Assert
//            Assert.NotNull(result);
//            Assert.Single(result.ChmmRoles);
//            Assert.Equal("user1@example.com", result.Email);
//        }


//        [Fact]
//        public async Task UsersRepository_GetUserByEmailReadOnly_NoRolesIncluded()
//        {
//            //Arrange

//            //Act
//            var result = await _repository.GetUserByEmail(_user1.Email, includeRoles: false);

//            //Assert
//            Assert.NotNull(result);
//            Assert.Null(result.ChmmRoles);
//            Assert.Equal("user1@example.com", result.Email);
//        }

//        [Fact]
//        public async Task UsersRepository_Can_CreateUser()
//        {
//            //Arrange
//            var newUser = new ChmmUser("New User", "new.user@example.com", null, new List<ChmmRole> { AddNewRole() });

//            //Act
//            var result = await _repository.Create(newUser);
            
//            //Assert
//            Assert.IsType<Guid>(result);
//        }

//        [Fact]
//        public async Task UsersRepository_CanUpdateUserStatus()
//        {
//            //Arrange
//            var user = new ChmmUser("Joe Bloggs", "user1@example.com", null, new List<ChmmRole>());

//            using var _identityDataContext = GetNewInMemoryContext();
//            PopulateContext(_identityDataContext, new List<ChmmUser>() { user });

//            Assert.Equal(UsersConstants.Status.Pending, _identityDataContext.ChmmUsers.First().Status);

//            user.Activate();

//            IUsersRepository repository = new UsersRepository(_identityDataContext);

//            //Act
//            var result = await repository.UnitOfWork.SaveChangesAsync();

//            //Assert
//            var resultUser = _identityDataContext.ChmmUsers.First();
//            Assert.Equal(UsersConstants.Status.Active, resultUser.Status);
//        }

//        #region Private Methods

//        /// <summary>
//        /// Gets a new instance of the an in-memory db context
//        /// </summary>
//        private IdentityContext GetNewInMemoryContext()
//        {
//            var context = new IdentityContext(_identityDataContextOptions);
//            context.Database.EnsureDeleted();
//            return context;
//        }

//        /// <summary>
//        /// Populated a db context with users
//        /// </summary>
//        /// <param name="context">The db context to populate</param>
//        /// <param name="users">The list of users to populate the context with</param>
//        private void PopulateContext(IdentityContext context, List<ChmmUser> users)
//        {
//            context.Append(users);
//            context.SaveChanges();
//        }

//        #endregion Private Methods
//    }
//}