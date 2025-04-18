using DAL;
using DAL.Repositories.ApiKeys.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForApiKeyRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task ApiKeyRepository_GetAllAsync_ReturnTwoApiKeys()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncApiKey").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncApiKey").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.ApiKeys.Add(new ApiKey
                {
                    Name = "qwerty",
                    KeyHash = "qwerty",
                    UserUid = new Guid(),
                    IsActive = true
                });

                context.ApiKeys.Add(new ApiKey
                {
                    Name = "qwerty",
                    KeyHash = "qwerty",
                    UserUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var apiKeyRepository = new ApiKeyRepository(dbContextFactory.Object);
            var apiKeys = await apiKeyRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, apiKeys.Count);
        }

        [Fact]
        public async Task ApiKeyRepository_GetAsyncUid_ReturnApiKey()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidApiKey").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidApiKey").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.ApiKeys.Add(new ApiKey
                {
                    Uid = uid,
                    Name = "qwerty",
                    KeyHash = "qwerty",
                    UserUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var apiKeyRepository = new ApiKeyRepository(dbContextFactory.Object);
            var apiKey = await apiKeyRepository.GetAsync(uid);

            //Assert
            Assert.Equal(apiKey.Uid, uid);
        }

        [Fact]
        public async Task ApiKeyRepository_CreateAsync_ReturnCreatedApiKey()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncApiKey").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncApiKey").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var apiKeyRepository = new ApiKeyRepository(dbContextFactory.Object);
            await apiKeyRepository.CreateAsync(new ApiKey
            {
                Uid = uid,
                Name = "qwerty",
                KeyHash = "qwerty",
                UserUid = new Guid(),
                IsActive = true
            });

            //Assert
            ApiKey? apiKey = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                apiKey = await context.ApiKeys.FindAsync(uid);
            }
            Assert.NotNull(apiKey);
        }

        [Fact]
        public async Task ApiKeyRepository_UpdateAsync_ReturnApiKeyWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncApiKey").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncApiKey").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncApiKey").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncApiKey").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            ApiKey? apiKey = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.ApiKeys.Add(new ApiKey
                {
                    Uid = uid,
                    Name = "qwerty",
                    KeyHash = "qwerty",
                    UserUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var apiKeyRepository = new ApiKeyRepository(dbContextFactory.Object);
            await apiKeyRepository.UpdateAsync(new ApiKey
            {
                Uid = uid,
                Name = "Name",
                KeyHash = "qwerty",
                UserUid = new Guid(),
                IsActive = true
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                apiKey = await context.ApiKeys.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(apiKey);
            Assert.Equal("Name", apiKey.Name);
        }

        [Fact]
        public async Task ApiKeyRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncApiKey").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncApiKey").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncApiKey").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncApiKey").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.ApiKeys.Add(new ApiKey
                {
                    Uid = uid,
                    Name = "qwerty",
                    KeyHash = "qwerty",
                    UserUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var apiKeyRepository = new ApiKeyRepository(dbContextFactory.Object);
            await apiKeyRepository.DeleteAsync(uid);
            ApiKey? apiKey = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                apiKey = await context.ApiKeys.FindAsync(uid);
            }
            //Assert
            Assert.Null(apiKey);
        }
    }
}