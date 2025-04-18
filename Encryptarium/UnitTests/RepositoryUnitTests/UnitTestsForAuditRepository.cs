using DAL;
using DAL.Repositories.Audits.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Moq;

namespace UnitTests.RepositoryUnitTests
{
    public class UnitTestsForAuditRepository
    {
        //{unit-of-work}_{scenario}_{expected-results-or-behaviour}

        [Fact]
        public async Task AuditRepository_GetAllAsync_ReturnTwoAudits()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncAudit").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAllAsyncAudit").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Audit.Add(new Audit
                {
                    UserUid = new Guid(),
                    Action = "action1",
                    DateAct = DateTime.UtcNow
                });

                context.Audit.Add(new Audit
                {
                    UserUid = new Guid(),
                    Action = "action2",
                    DateAct = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var auditRepository = new AuditRepository(dbContextFactory.Object);
            var audits = await auditRepository.GetAllAsync();

            //Assert
            Assert.Equal(2, audits.Count);
        }

        [Fact]
        public async Task AuditRepository_GetAsyncUid_ReturnAudit()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidAudit").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("GetAsyncUidAudit").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Audit.Add(new Audit
                {
                    Uid = uid,
                    UserUid = new Guid(),
                    Action = "action",
                    DateAct = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var auditRepository = new AuditRepository(dbContextFactory.Object);
            var audit = await auditRepository.GetAsync(uid);

            //Assert
            Assert.Equal(audit.Uid, uid);
        }

        [Fact]
        public async Task AuditRepository_CreateAsync_ReturnCreatedAudit()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncAudit").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("CreateAsyncAudit").Options));
            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");

            //Act
            var auditRepository = new AuditRepository(dbContextFactory.Object);
            await auditRepository.CreateAsync(new Audit
            {
                Uid = uid,
                UserUid = new Guid(),
                Action = "action",
                DateAct = DateTime.UtcNow
            });

            //Assert
            Audit? audit = null;
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                audit = await context.Audit.FindAsync(uid);
            }
            Assert.NotNull(audit);
        }

        [Fact]
        public async Task AuditRepository_UpdateAsync_ReturnAuditWithUpdateEmail()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncAudit").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncAudit").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncAudit").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("UpdateAsyncAudit").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            Audit? audit = null;
            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Audit.Add(new Audit
                {
                    Uid = uid,
                    UserUid = new Guid(),
                    Action = "action",
                    DateAct = DateTime.UtcNow
                });

                context.SaveChanges();
            }
            //Act
            var auditRepository = new AuditRepository(dbContextFactory.Object);
            await auditRepository.UpdateAsync(new Audit
            {
                Uid = uid,
                UserUid = new Guid(),
                Action = "action2",
                DateAct = DateTime.UtcNow
            });
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                audit = await context.Audit.FindAsync(uid);
            }
            //Assert
            Assert.NotNull(audit);
            Assert.Equal("action2", audit.Action);
        }

        [Fact]
        public async Task AuditRepository_DeleteAsync_ReturnNull()
        {
            //Arrange
            var dbContextFactory = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactory.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncAudit").Options));
            dbContextFactory.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncAudit").Options));

            var dbContextFactoryGet = new Mock<IDbContextFactory<SecretContext>>();
            dbContextFactoryGet.Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncAudit").Options));
            dbContextFactoryGet.Setup(f => f.CreateDbContext())
                .Returns(new SecretContext(new DbContextOptionsBuilder<SecretContext>()
                .UseInMemoryDatabase("DeleteAsyncAudit").Options));

            var options = new DbContextOptionsBuilder<SecretContext>().Options;

            var uid = new Guid("5498fbef-c159-48c2-898e-2f8ced113e69");
            using (var context = dbContextFactory.Object.CreateDbContext())
            {
                context.Audit.Add(new Audit
                {
                    Uid = uid,
                    UserUid = new Guid(),
                    Action = "action",
                    DateAct = DateTime.UtcNow
                });

                context.SaveChanges();
            }

            //Act
            var auditRepository = new AuditRepository(dbContextFactory.Object);
            await auditRepository.DeleteAsync(uid);
            Audit? audit = null;
            using (var context = dbContextFactoryGet.Object.CreateDbContext())
            {
                audit = await context.Audit.FindAsync(uid);
            }
            //Assert
            Assert.Null(audit);
        }
    }
}