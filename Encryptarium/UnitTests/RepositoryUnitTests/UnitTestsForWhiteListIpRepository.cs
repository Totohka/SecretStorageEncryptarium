using DAL;
using DAL.Repositories.WhiteListIps.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForWhiteListIpRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task WhiteListIpRepository_GetAllAsync_ReturnTwoWhiteListIps()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncWhiteListIp").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncWhiteListIp").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.WhiteListIps.Add(new WhiteListIp
                {
                    Ip = "192.168.1.1",
                    ApiKeyUid = new Guid(),
                    IsActive = true
                });

                context.WhiteListIps.Add(new WhiteListIp
                {
                    Ip = "192.168.1.2",
                    ApiKeyUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var whiteListIpRepository = new WhiteListIpRepository(dbContextFactory.Object);
            var whiteListIps = await whiteListIpRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, whiteListIps.Count);
        }

        [Fact]
        public async Task WhiteListIpRepository_GetAsyncUid_ReturnWhiteListIp()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidWhiteListIp").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidWhiteListIp").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.WhiteListIps.Add(new WhiteListIp
                {
                    Uid = uid,
                    Ip = "192.168.1.1",
                    ApiKeyUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var whiteListIpRepository = new WhiteListIpRepository(dbContextFactory.Object);
            var whiteListIp = await whiteListIpRepository.GetAsync(uid);

            //Assert
            Assert.Equal(whiteListIp.Uid, uid);
        }

        [Fact]
        public async Task WhiteListIpRepository_CreateAsync_ReturnCreatedWhiteListIp()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncWhiteListIp").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncWhiteListIp").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var whiteListIpRepository = new WhiteListIpRepository(dbContextFactory.Object);
            await whiteListIpRepository.CreateAsync(new WhiteListIp
            {
                Uid = uid,
                Ip = "192.168.1.1",
                ApiKeyUid = new Guid(),
                IsActive = true
            });

            //Assert
            WhiteListIp? whiteListIp = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                whiteListIp = await context.WhiteListIps.FindAsync(uid);
            }
            Assert.NotNull(whiteListIp);
        }

        [Fact]
        public async Task WhiteListIpRepository_UpdateAsync_ReturnWhiteListIpWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncWhiteListIp").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncWhiteListIp").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncWhiteListIp").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncWhiteListIp").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            WhiteListIp? whiteListIp = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.WhiteListIps.Add(new WhiteListIp
                {
                    Uid = uid,
                    Ip = "192.168.1.1",
                    ApiKeyUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var whiteListIpRepository = new WhiteListIpRepository(dbContextFactory.Object);
            await whiteListIpRepository.UpdateAsync(new WhiteListIp
            {
                Uid = uid,
                Ip = "192.168.1.2",
                ApiKeyUid = new Guid(),
                IsActive = true
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                whiteListIp = await context.WhiteListIps.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(whiteListIp);
            Assert.Equal("192.168.1.2", whiteListIp.Ip);
        }

        [Fact]
        public async Task WhiteListIpRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncWhiteListIp").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncWhiteListIp").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncWhiteListIp").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncWhiteListIp").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.WhiteListIps.Add(new WhiteListIp
                {
                    Uid = uid,
                    Ip = "192.168.1.2",
                    ApiKeyUid = new Guid(),
                    IsActive = true
                });

                context.SaveChanges();
            }

            //Act
            var whiteListIpRepository = new WhiteListIpRepository(dbContextFactory.Object);
            await whiteListIpRepository.DeleteAsync(uid);
            WhiteListIp? whiteListIp = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                whiteListIp = await context.WhiteListIps.FindAsync(uid);
            }
            //Assert
            Assert.Null(whiteListIp);
        }
    }
}