using DAL;
using DAL.Repositories.StorageLinkPolicies.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForStorageLinkPolicyRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task StorageLinkPolicyRepository_GetAllAsync_ReturnTwoStorageLinkPolicies()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncStorageLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncStorageLinkPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StorageLinkPolicies.Add(new StorageLinkPolicy
                {
                    StoragePolicyUid = Guid.NewGuid(),
                    StorageUid = Guid.NewGuid(),
                    RoleUid = Guid.NewGuid(),
                    IsActive = true
                });

                context.StorageLinkPolicies.Add(new StorageLinkPolicy
                {
                    StoragePolicyUid = Guid.NewGuid(),
                    StorageUid = Guid.NewGuid(),
                    RoleUid = Guid.NewGuid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var storageLinkPolicyRepository = new StorageLinkPolicyRepository(dbContextFactory.Object);
            var storageLinkPolicys = await storageLinkPolicyRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, storageLinkPolicys.Count);
        }

        [Fact]
        public async Task StorageLinkPolicyRepository_GetAsyncUid_ReturnStorageLinkPolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidStorageLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidStorageLinkPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var storageUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StorageLinkPolicies.Add(new StorageLinkPolicy
                {
                    StoragePolicyUid = new Guid(),
                    StorageUid = storageUid,
                    RoleUid = userUid,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var storageLinkPolicyRepository = new StorageLinkPolicyRepository(dbContextFactory.Object);
            var storageLinkPolicy = await storageLinkPolicyRepository.GetAsync(storageUid, userUid);

            //Assert
            Assert.NotNull(storageLinkPolicy);
        }

        [Fact]
        public async Task StorageLinkPolicyRepository_CreateAsync_ReturnCreatedStorageLinkPolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncStorageLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncStorageLinkPolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var storageUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            //Act
            var storageLinkPolicyRepository = new StorageLinkPolicyRepository(dbContextFactory.Object);
            await storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy
            {
                StoragePolicyUid = new Guid(),
                StorageUid = storageUid,
                RoleUid = userUid,
                IsActive = true
            });

            //Assert
            StorageLinkPolicy? storageLinkPolicy = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                storageLinkPolicy = await context.StorageLinkPolicies.FindAsync(userUid, storageUid);
            }
            Assert.NotNull(storageLinkPolicy);
        }

        [Fact]
        public async Task StorageLinkPolicyRepository_UpdateAsync_ReturnStorageLinkPolicyWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorageLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorageLinkPolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorageLinkPolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorageLinkPolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            StorageLinkPolicy? storageLinkPolicy = null;

            var storageUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StorageLinkPolicies.Add(new StorageLinkPolicy
                {
                    StoragePolicyUid = new Guid(),
                    StorageUid = storageUid,
                    RoleUid = userUid,
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var storageLinkPolicyRepository = new StorageLinkPolicyRepository(dbContextFactory.Object);
            await storageLinkPolicyRepository.UpdateAsync(new StorageLinkPolicy
            {
                StoragePolicyUid = new Guid(),
                StorageUid = storageUid,
                RoleUid = userUid,
                IsActive = false
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                storageLinkPolicy = await context.StorageLinkPolicies.FindAsync(userUid, storageUid);
            }
            //Assert
            Assert.NotNull(storageLinkPolicy);
            Assert.False(storageLinkPolicy.IsActive);
        }

        [Fact]
        public async Task StorageLinkPolicyRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorageLinkPolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorageLinkPolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorageLinkPolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorageLinkPolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var storageUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            var userUid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e60");

            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StorageLinkPolicies.Add(new StorageLinkPolicy
                {
                    StoragePolicyUid = new Guid(),
                    StorageUid = storageUid,
                    RoleUid = userUid,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var storageLinkPolicyRepository = new StorageLinkPolicyRepository(dbContextFactory.Object);
            await storageLinkPolicyRepository.DeleteAsync(storageUid, userUid);
            StorageLinkPolicy? storageLinkPolicy = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                storageLinkPolicy = await context.StorageLinkPolicies.FindAsync(userUid, storageUid);
            }
            //Assert
            Assert.Null(storageLinkPolicy);
        }
    }
}