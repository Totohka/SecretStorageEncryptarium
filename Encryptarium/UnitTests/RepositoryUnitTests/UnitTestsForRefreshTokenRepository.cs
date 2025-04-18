using DAL;
using DAL.Repositories.RefreshTokens.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForRefreshTokenRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task RefreshTokenRepository_GetAllAsync_ReturnTwoRefreshTokens()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncRefreshToken").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncRefreshToken").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.RefreshTokens.Add(new RefreshToken
                {
                    DateExpireToken = DateTime.UtcNow,
                    DateCreate = DateTime.UtcNow
                });

                context.RefreshTokens.Add(new RefreshToken
                {
                    DateExpireToken = DateTime.UtcNow,
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var refreshTokenRepository = new RefreshTokenRepository(dbContextFactory.Object);
            var refreshTokens = await refreshTokenRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, refreshTokens.Count);
        }

        [Fact]
        public async Task RefreshTokenRepository_GetAsyncUid_ReturnRefreshToken()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidRefreshToken").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidRefreshToken").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.RefreshTokens.Add(new RefreshToken
                {
                    Uid = uid,
                    DateExpireToken = DateTime.UtcNow,
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var refreshTokenRepository = new RefreshTokenRepository(dbContextFactory.Object);
            var refreshToken = await refreshTokenRepository.GetAsync(uid);

            //Assert
            Assert.Equal(refreshToken.Uid, uid);
        }

        [Fact]
        public async Task RefreshTokenRepository_CreateAsync_ReturnCreatedRefreshToken()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncRefreshToken").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncRefreshToken").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var refreshTokenRepository = new RefreshTokenRepository(dbContextFactory.Object);
            await refreshTokenRepository.CreateAsync(new RefreshToken
            {
                Uid = uid,
                DateExpireToken = DateTime.UtcNow,
                DateCreate = DateTime.UtcNow
            });

            //Assert
            RefreshToken? refreshToken = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                refreshToken = await context.RefreshTokens.FindAsync(uid);
            }
            Assert.NotNull(refreshToken);
        }

        [Fact]
        public async Task RefreshTokenRepository_UpdateAsync_ReturnRefreshTokenWithUpdateTitle()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncRefreshToken").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncRefreshToken").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncRefreshToken").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncRefreshToken").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            RefreshToken? refreshToken = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.RefreshTokens.Add(new RefreshToken
                {
                    Uid = uid,
                    DateExpireToken = DateTime.UtcNow,
                    DateCreate = DateTime.UtcNow,
                    IsActive = true
                });

                context.SaveChanges();
            }
            //Act
            var refreshTokenRepository = new RefreshTokenRepository(dbContextFactory.Object);
            await refreshTokenRepository.UpdateAsync(new RefreshToken
            {
                Uid = uid,
                DateExpireToken = DateTime.UtcNow,
                DateCreate = DateTime.UtcNow,
                IsActive = false
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                refreshToken = await context.RefreshTokens.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(refreshToken);
            Assert.False(refreshToken.IsActive);
        }

        [Fact]
        public async Task RefreshTokenRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncRefreshToken").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncRefreshToken").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncRefreshToken").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncRefreshToken").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.RefreshTokens.Add(new RefreshToken
                {
                    Uid = uid,
                    DateExpireToken = DateTime.UtcNow,
                    DateCreate = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var refreshTokenRepository = new RefreshTokenRepository(dbContextFactory.Object);
            await refreshTokenRepository.DeleteAsync(uid);
            RefreshToken? refreshToken = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                refreshToken = await context.RefreshTokens.FindAsync(uid);
            }
            //Assert
            Assert.Null(refreshToken);
        }
    }
}