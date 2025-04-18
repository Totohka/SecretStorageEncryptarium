using DAL;
using DAL.Repositories.Secrets.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForSecretRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task SecretRepository_GetAllAsync_ReturnTwoSecrets()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncSecret").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncSecret").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Secrets.Add(new Secret
                {
                    Name = "qwerty",
                    Value = "qwerty",
                    DateCreate = DateTime.UtcNow,
                    StorageUid = new Guid()
                });

                context.Secrets.Add(new Secret
                {
                    Name = "qwerty",
                    Value = "qwerty",
                    DateCreate = DateTime.UtcNow,
                    StorageUid = new Guid()
                });

                context.SaveChanges();
            }

            //Act
            var secretRepository = new SecretRepository(dbContextFactory.Object);
            var secrets = await secretRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, secrets.Count);
        }

        [Fact]
        public async Task SecretRepository_GetAsyncUid_ReturnSecret()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidSecret").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidSecret").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Secrets.Add(new Secret
                {
                    Uid = uid,
                    Name = "qwerty",
                    Value = "qwerty",
                    DateCreate = DateTime.UtcNow,
                    StorageUid = new Guid()
                });

                context.SaveChanges();
            }

            //Act
            var secretRepository = new SecretRepository(dbContextFactory.Object);
            var secret = await secretRepository.GetAsync(uid);

            //Assert
            Assert.Equal(secret.Uid, uid);
        }

        [Fact]
        public async Task SecretRepository_CreateAsync_ReturnCreatedSecret()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncSecret").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncSecret").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var secretRepository = new SecretRepository(dbContextFactory.Object);
            await secretRepository.CreateAsync(new Secret
            {
                Uid = uid,
                Name = "qwerty",
                Value = "qwerty",
                DateCreate = DateTime.UtcNow,
                StorageUid = new Guid()
            });

            //Assert
            Secret? secret = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                secret = await context.Secrets.FindAsync(uid);
            }
            Assert.NotNull(secret);
        }

        [Fact]
        public async Task SecretRepository_UpdateAsync_ReturnSecretWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecret").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecret").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecret").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncSecret").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            Secret? secret = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Secrets.Add(new Secret
                {
                    Uid = uid,
                    Name = "qwerty",
                    Value = "qwerty",
                    DateCreate = DateTime.UtcNow,
                    StorageUid = new Guid()
                });

                context.SaveChanges();
            }
            //Act
            var secretRepository = new SecretRepository(dbContextFactory.Object);
            await secretRepository.UpdateAsync(new Secret
            {
                Uid = uid,
                Name = "Name",
                Value = "qwerty",
                DateCreate = DateTime.UtcNow,
                StorageUid = new Guid()
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                secret = await context.Secrets.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(secret);
            Assert.Equal("Name", secret.Name);
        }

        [Fact]
        public async Task SecretRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecret").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecret").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecret").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncSecret").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Secrets.Add(new Secret
                {
                    Uid = uid,
                    Name = "qwerty",
                    Value = "qwerty",
                    DateCreate = DateTime.UtcNow,
                    StorageUid = new Guid()
                });

                context.SaveChanges();
            }

            //Act
            var secretRepository = new SecretRepository(dbContextFactory.Object);
            await secretRepository.DeleteAsync(uid);
            Secret? secret = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                secret = await context.Secrets.FindAsync(uid);
            }
            //Assert
            Assert.Null(secret);
        }
    }
}