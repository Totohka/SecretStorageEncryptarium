using DAL;
using DAL.Repositories.Storages.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForStorageRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task StorageRepository_GetAllAsync_ReturnTwoStorages()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncStorage").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncStorage").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Storages.Add(new Storage
                {
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow
                });

                context.Storages.Add(new Storage
                {
                    Title = "Title2",
                    Description = "Description2",
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var storageRepository = new StorageRepository(dbContextFactory.Object);
            var storages = await storageRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, storages.Count);
        }

        [Fact]
        public async Task StorageRepository_GetAsyncUid_ReturnStorage()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidStorage").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidStorage").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Storages.Add(new Storage
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var storageRepository = new StorageRepository(dbContextFactory.Object);
            var storage = await storageRepository.GetAsync(uid);

            //Assert
            Assert.Equal(storage.Uid, uid);
        }

        [Fact]
        public async Task StorageRepository_CreateAsync_ReturnCreatedStorage()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncStorage").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncStorage").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var storageRepository = new StorageRepository(dbContextFactory.Object);
            await storageRepository.CreateAsync(new Storage
            {
                Uid = uid,
                Title = "Title1",
                Description = "Description1",
                DateCreate = DateTime.UtcNow
            });

            //Assert
            Storage? storage = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                storage = await context.Storages.FindAsync(uid);
            }
            Assert.NotNull(storage);
        }

        [Fact]
        public async Task StorageRepository_UpdateAsync_ReturnStorageWithUpdateTitle()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorage").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorage").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorage").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncStorage").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            Storage? storage = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Storages.Add(new Storage
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }
            //Act
            var storageRepository = new StorageRepository(dbContextFactory.Object);
            await storageRepository.UpdateAsync(new Storage
            {
                Uid = uid,
                Title = "TitleChange",
                Description = "Description1",
                DateCreate = DateTime.UtcNow
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                storage = await context.Storages.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(storage);
            Assert.Equal("TitleChange", storage.Title);
        }

        [Fact]
        public async Task StorageRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorage").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorage").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorage").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncStorage").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Storages.Add(new Storage
                {
                    Uid = uid,
                    Title = "Title1",
                    Description = "Description1",
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var storageRepository = new StorageRepository(dbContextFactory.Object);
            await storageRepository.DeleteAsync(uid);
            Storage? storage = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                storage = await context.Storages.FindAsync(uid);
            }
            //Assert
            Assert.Null(storage);
        }
    }
}