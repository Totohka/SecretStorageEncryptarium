using DAL;
using DAL.Repositories.SecretLinkPolicies.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForSecretLinkPolicyRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task SecretLinkPolicyRepository_GetAllAsync_ReturnTwoSecretLinkPolicies()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncSecretLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncSecretLinkPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretLinkPolicies.Add(new SecretLinkPolicy
                {
                    SecretPolicyUid = Guid.NewGuid(),
                    SecretUid = Guid.NewGuid(),
                    RoleUid = Guid.NewGuid(),
                    IsActive = true
                });

                context.SecretLinkPolicies.Add(new SecretLinkPolicy
                {
                    SecretPolicyUid = Guid.NewGuid(),
                    SecretUid = Guid.NewGuid(),
                    RoleUid = Guid.NewGuid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var secretLinkPolicyRepository = new SecretLinkPolicyRepository(dbContextFactory.Object);
            var secretLinkPolicys = await secretLinkPolicyRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, secretLinkPolicys.Count);
        }

        [Fact]
        public async Task SecretLinkPolicyRepository_GetAsyncUid_ReturnSecretLinkPolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidSecretLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidSecretLinkPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var secretUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretLinkPolicies.Add(new SecretLinkPolicy
                {
                    SecretPolicyUid = new Guid(),
                    SecretUid = secretUid,
                    RoleUid = userUid,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var secretLinkPolicyRepository = new SecretLinkPolicyRepository(dbContextFactory.Object);
            var secretLinkPolicy = await secretLinkPolicyRepository.GetAsync(secretUid, userUid);

            //Assert
            Assert.NotNull(secretLinkPolicy);
        }

        [Fact]
        public async Task SecretLinkPolicyRepository_CreateAsync_ReturnCreatedSecretLinkPolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncSecretLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncSecretLinkPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var secretUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            //Act
            var secretLinkPolicyRepository = new SecretLinkPolicyRepository(dbContextFactory.Object);
            await secretLinkPolicyRepository.CreateAsync(new SecretLinkPolicy
            {
                SecretPolicyUid = new Guid(),
                SecretUid = secretUid,
                RoleUid = userUid,
                IsActive = true
            });

            //Assert
            SecretLinkPolicy? secretLinkPolicy = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                secretLinkPolicy = await context.SecretLinkPolicies.FindAsync(userUid, secretUid);
            }
            Assert.NotNull(secretLinkPolicy);
        }

        [Fact]
        public async Task SecretLinkPolicyRepository_UpdateAsync_ReturnSecretLinkPolicyWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretLinkPolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretLinkPolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecretLinkPolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            SecretLinkPolicy? secretLinkPolicy = null;

            var secretUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretLinkPolicies.Add(new SecretLinkPolicy
                {
                    SecretPolicyUid = new Guid(),
                    SecretUid = secretUid,
                    RoleUid = userUid,
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var secretLinkPolicyRepository = new SecretLinkPolicyRepository(dbContextFactory.Object);
            await secretLinkPolicyRepository.UpdateAsync(new SecretLinkPolicy
            {
                SecretPolicyUid = new Guid(),
                SecretUid = secretUid,
                RoleUid = userUid,
                IsActive = false
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                secretLinkPolicy = await context.SecretLinkPolicies.FindAsync(userUid, secretUid);
            }
            //Assert
            Assert.NotNull(secretLinkPolicy);
            Assert.False(secretLinkPolicy.IsActive);
        }

        [Fact]
        public async Task SecretLinkPolicyRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretLinkPolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretLinkPolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecretLinkPolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var secretUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.SecretLinkPolicies.Add(new SecretLinkPolicy
                {
                    SecretPolicyUid = new Guid(),
                    SecretUid = secretUid,
                    RoleUid = userUid,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var secretLinkPolicyRepository = new SecretLinkPolicyRepository(dbContextFactory.Object);
            await secretLinkPolicyRepository.DeleteAsync(secretUid, userUid);
            SecretLinkPolicy? secretLinkPolicy = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                secretLinkPolicy = await context.SecretLinkPolicies.FindAsync(userUid, secretUid);
            }
            //Assert
            Assert.Null(secretLinkPolicy);
        }
    }
}