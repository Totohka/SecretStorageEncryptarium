using DAL;
using DAL.Repositories.StoragePolicies.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForStoragePolicyRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task StoragePolicyRepository_GetAllAsync_ReturnTwoStoragePolicies()
        {
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncStoragePolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncStoragePolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StoragePolicies.Add(new StoragePolicy
                {
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsCreate = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.StoragePolicies.Add(new StoragePolicy
                {
                    Title = "Title2",
                    Description = "Description2",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsCreate = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var storagePolicyRepository = new StoragePolicyRepository(dbContextFactory.Object);
            var storagePolicies = await storagePolicyRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, storagePolicies.Count);
        }

        [Fact]
        public async Task StoragePolicyRepository_GetAsyncUid_ReturnStoragePolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidStoragePolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidStoragePolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StoragePolicies.Add(new StoragePolicy
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsCreate = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var storagePolicyRepository = new StoragePolicyRepository(dbContextFactory.Object);
            var storagePolicy = await storagePolicyRepository.GetAsync(uid);

            //Assert
            Assert.Equal(storagePolicy.Uid, uid);
        }

        [Fact]
        public async Task StoragePolicyRepository_CreateAsync_ReturnCreatedStoragePolicy()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncStoragePolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncStoragePolicy").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var storagePolicyRepository = new StoragePolicyRepository(dbContextFactory.Object);
            await storagePolicyRepository.CreateAsync(new StoragePolicy
            {
                Uid = uid,
                Title = "Title1",
                Description = "Description1",
                DateCreate = DateTime.UtcNow,
                IsRead = false,
                IsCreate = false,
                IsUpdate = false,
                IsDelete = false,
                IsActive = true
            });

            //Assert
            StoragePolicy? storagePolicy = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                storagePolicy = await context.StoragePolicies.FindAsync(uid);
            }
            Assert.NotNull(storagePolicy);
        }

        [Fact]
        public async Task StoragePolicyRepository_UpdateAsync_ReturnStoragePolicyWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStoragePolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStoragePolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStoragePolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStoragePolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            StoragePolicy? storagePolicy = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StoragePolicies.Add(new StoragePolicy
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = false,
                    IsCreate = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var storagePolicyRepository = new StoragePolicyRepository(dbContextFactory.Object);
            await storagePolicyRepository.UpdateAsync(new StoragePolicy
            {
                Uid = uid,
                Title = "Title1",
                Description = "Description1",
                DateCreate = DateTime.UtcNow,
                IsRead = true,
                IsCreate = false,
                IsUpdate = false,
                IsDelete = false,
                IsActive = true
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                storagePolicy = await context.StoragePolicies.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(storagePolicy);
            Assert.True(storagePolicy.IsRead);
        }

        [Fact]
        public async Task StoragePolicyRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStoragePolicy").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStoragePolicy").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStoragePolicy").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStoragePolicy").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.StoragePolicies.Add(new StoragePolicy
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow,
                    IsRead = true,
                    IsCreate = false,
                    IsUpdate = false,
                    IsDelete = false,
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var storagePolicyRepository = new StoragePolicyRepository(dbContextFactory.Object);
            await storagePolicyRepository.DeleteAsync(uid);
            StoragePolicy? storagePolicy = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                storagePolicy = await context.StoragePolicies.FindAsync(uid);
            }
            //Assert
            Assert.Null(storagePolicy);
        }
    }
}