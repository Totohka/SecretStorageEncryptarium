using DAL;
using DAL.Repositories.SecretPolicies.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForSecretPolicyRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task SecretPolicyRepository_GetAllAsync_ReturnTwoSecretPolicies()
        {
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncSecretPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncSecretPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretPolicies.Add(new SecretPolicy
                {
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SecretPolicies.Add(new SecretPolicy
                {
                    Title = "Title2",
                    Description = "Description2",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var secretPolicyRepository = new SecretPolicyRepository(dbContextFactory.Object);
            var secretPolicies = await secretPolicyRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, secretPolicies.Count);
        }

        [Fact]
        public async Task SecretPolicyRepository_GetAsyncUid_ReturnSecretPolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidSecretPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidSecretPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretPolicies.Add(new SecretPolicy
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var secretPolicyRepository = new SecretPolicyRepository(dbContextFactory.Object);
            var secretPolicy = await secretPolicyRepository.GetAsync(uid);

            //Assert
            Assert.Equal(secretPolicy.Uid, uid);
        }

        [Fact]
        public async Task SecretPolicyRepository_CreateAsync_ReturnCreatedSecretPolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncSecretPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncSecretPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var secretPolicyRepository = new SecretPolicyRepository(dbContextFactory.Object);
            await secretPolicyRepository.CreateAsync(new SecretPolicy
            {
                Uid = uid,
                Title = "Title1",
                Description = "Description1",
                DateCreate = DateTime.UtcNow,
                IsRead = false,
                IsUpdate = false,
                IsDelete = false,
                IsActive = true
            });

            //Assert
            SecretPolicy? secretPolicy = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                secretPolicy = await context.SecretPolicies.FindAsync(uid);
            }
            Assert.NotNull(secretPolicy);
        }

        [Fact]
        public async Task SecretPolicyRepository_UpdateAsync_ReturnSecretPolicyWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretPolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretPolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretPolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            SecretPolicy? secretPolicy = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretPolicies.Add(new SecretPolicy
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var secretPolicyRepository = new SecretPolicyRepository(dbContextFactory.Object);
            await secretPolicyRepository.UpdateAsync(new SecretPolicy
            {
                Uid = uid,
                Title = "Title1",
                Description = "Description1",
                DateCreate = DateTime.UtcNow,
                IsRead = true,
                IsUpdate = false,
                IsDelete = false,
                IsActive = true
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                secretPolicy = await context.SecretPolicies.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(secretPolicy);
            Assert.True(secretPolicy.IsRead);
        }

        [Fact]
        public async Task SecretPolicyRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretPolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretPolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretPolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretPolicies.Add(new SecretPolicy
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = true,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var secretPolicyRepository = new SecretPolicyRepository(dbContextFactory.Object);
            await secretPolicyRepository.DeleteAsync(uid);
            SecretPolicy? secretPolicy = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                secretPolicy = await context.SecretPolicies.FindAsync(uid);
            }
            //Assert
            Assert.Null(secretPolicy);
        }
    }
}