using DAL;
using DAL.Repositories.Users.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForUserRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task UserRepository_GetAllAsync_ReturnTwoUsers()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncUser").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncUser").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Users.Add(new User
                {
                    Login = "qwerty",
                    PasswordHash = "qwerty",
                    Email = "qwerty"
                });

                context.Users.Add(new User
                {
                    Login = "qwerty1",
                    PasswordHash = "qwerty1",
                    Email = "qwerty1"
                });

                context.SaveChanges();
            }

            //Act
            var userRepository = new UserRepository(dbContextFactory.Object);
            var users = await userRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task UserRepository_GetAsyncUid_ReturnUser()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidUser").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidUser").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Users.Add(new User
                {
                    Uid = uid,
                    Login = "qwerty",
                    PasswordHash = "qwerty",
                    Email = "qwerty"
                });

                context.SaveChanges();
            }

            //Act
            var userRepository = new UserRepository(dbContextFactory.Object);
            var user = await userRepository.GetAsync(uid);

            //Assert
            Assert.Equal(user.Uid, uid);
        }

        [Fact]
        public async Task UserRepository_CreateAsync_ReturnCreatedUser()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncUser").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncUser").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var userRepository = new UserRepository(dbContextFactory.Object);
            await userRepository.CreateAsync(new User
            {
                Uid = uid,
                Login = "qwerty",
                PasswordHash = "qwerty",
                Email = "qwerty"
            });

            //Assert
            User? user = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                user = await context.Users.FindAsync(uid);
            }
            Assert.NotNull(user);
        }

        [Fact]
        public async Task UserRepository_UpdateAsync_ReturnUserWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncUser").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncUser").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncUser").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncUser").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            User? user = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Users.Add(new User
                {
                    Uid = uid,
                    Login = "qwerty",
                    PasswordHash = "qwerty",
                    Email = "qwerty"
                });

                context.SaveChanges();
            }
            //Act
            var userRepository = new UserRepository(dbContextFactory.Object);
            await userRepository.UpdateAsync(new User
            {
                Uid = uid,
                Login = "login",
                PasswordHash = "qwerty",
                Email = "qwerty"
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                user = await context.Users.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(user);
            Assert.Equal("login", user.Login);
        }

        [Fact]
        public async Task UserRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncUser").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncUser").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncUser").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncUser").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Users.Add(new User
                {
                    Uid = uid,
                    Login = "qwerty",
                    PasswordHash = "qwerty",
                    Email = "qwerty"
                });

                context.SaveChanges();
            }

            //Act
            var userRepository = new UserRepository(dbContextFactory.Object);
            await userRepository.DeleteAsync(uid);
            User? user = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                user = await context.Users.FindAsync(uid);
            }
            //Assert
            Assert.Null(user);
        }
    }
}