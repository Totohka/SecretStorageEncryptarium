using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Entities;

namespace DAL
{
    public class SecretContext : DbContext
    {
        public SecretContext(DbContextOptions<SecretContext> dbContextOptions) : base(dbContextOptions) { }
        public DbSet<WhiteListIp> WhiteListIps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StoragePolicy> StoragePolicies { get; set; }
        public DbSet<StorageLinkPolicy> StorageLinkPolicies { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<SecretPolicy> SecretPolicies { get; set; }
        public DbSet<SecretLinkPolicy> SecretLinkPolicies { get; set; }
        public DbSet<Secret> Secrets { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Audit> Audit { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WhiteListIp>(WhiteListIpConfigure);
            modelBuilder.Entity<User>(UserConfigure);
            modelBuilder.Entity<StoragePolicy>(StoragePolicyConfigure);
            modelBuilder.Entity<StorageLinkPolicy>(StorageLinkPolicyConfigure);
            modelBuilder.Entity<Storage>(StorageConfigure);
            modelBuilder.Entity<SecretPolicy>(SecretPolicyConfigure);
            modelBuilder.Entity<SecretLinkPolicy>(SecretLinkPolicyConfigure);
            modelBuilder.Entity<Secret>(SecretConfigure);
            modelBuilder.Entity<RefreshToken>(RefreshTokenConfigure);
            modelBuilder.Entity<Audit>(AuditConfigure);
            modelBuilder.Entity<ApiKey>(ApiKeyConfigure);
            modelBuilder.Entity<Role>(RoleConfigure);
            modelBuilder.Entity<UserRole>(UserRoleConfigure);
            modelBuilder.Entity<RoleType>(RoleTypeConfigure);
        }
        public void WhiteListIpConfigure(EntityTypeBuilder<WhiteListIp> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Ip).HasMaxLength(15).IsRequired();
            builder.Property(p => p.ApiKeyUid).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }
        public void RoleTypeConfigure(EntityTypeBuilder<RoleType> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Code).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }
        public void UserRoleConfigure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(p => new { p.UserUid, p.RoleUid });
            builder.Property(p => p.UserUid).IsRequired();
            builder.Property(p => p.RoleUid).IsRequired();
            builder.Property(p => p.IsMain).IsRequired();
        }
        public void RoleConfigure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(600).IsRequired();
            builder.Property(p => p.RoleTypeUid).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void UserConfigure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Email).HasMaxLength(183).IsRequired();
            builder.Property(p => p.Login).HasMaxLength(100).IsRequired();
            builder.Property(p => p.PasswordHash).HasMaxLength(100).IsRequired();
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.RefreshTokenUid);
            builder.Property(p => p.IsCreateStorage).IsRequired();
            builder.Property(p => p.IsAdmin).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void StoragePolicyConfigure(EntityTypeBuilder<StoragePolicy> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(600).IsRequired();
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.IsRead).IsRequired();
            builder.Property(p => p.IsCreate).IsRequired();
            builder.Property(p => p.IsUpdate).IsRequired();
            builder.Property(p => p.IsDelete).IsRequired();
            builder.Property(p => p.IsCommon).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void StorageLinkPolicyConfigure(EntityTypeBuilder<StorageLinkPolicy> builder)
        {
            builder.HasKey(p => new { p.RoleUid, p.StorageUid });
            builder.Property(p => p.StoragePolicyUid).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void StorageConfigure(EntityTypeBuilder<Storage> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Title).HasMaxLength(200).IsRequired(); 
            builder.Property(p => p.Description).HasMaxLength(600).IsRequired();
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.IsCommon).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void SecretPolicyConfigure(EntityTypeBuilder<SecretPolicy> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(600).IsRequired();
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.IsRead).IsRequired();
            builder.Property(p => p.IsUpdate).IsRequired();
            builder.Property(p => p.IsDelete).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void SecretLinkPolicyConfigure(EntityTypeBuilder<SecretLinkPolicy> builder)
        {
            builder.HasKey(p => new { p.RoleUid, p.SecretUid });
            builder.Property(p => p.SecretPolicyUid).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }

        public void SecretConfigure(EntityTypeBuilder<Secret> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Value);
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.StorageUid).IsRequired();
        }

        public void RefreshTokenConfigure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.DateCreate).IsRequired();
            builder.Property(p => p.DateExpireToken).IsRequired();
            builder.Property(p => p.DateDeadToken);
            builder.Property(p => p.IsActive).IsRequired();
        }
        public void AuditConfigure(EntityTypeBuilder<Audit> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.UserUid);
            builder.Property(p => p.StatusCode).IsRequired();
            builder.Property(p => p.SourceMicroservice).IsRequired();
            builder.Property(p => p.SourceController).IsRequired();
            builder.Property(p => p.SourceMethod).HasMaxLength(100).IsRequired();
            builder.Property(p => p.IsSourceRightAdmin).IsRequired();
            builder.Property(p => p.AuthorizePolice).IsRequired();
            builder.Property(p => p.Action).HasMaxLength(1000).IsRequired();
            builder.Property(p => p.DateAct).IsRequired();
            builder.Property(p => p.Entity);
            builder.Property(p => p.EntityUid);
        }

        public void ApiKeyConfigure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.HasKey(p => p.Uid);
            builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
            builder.Property(p => p.KeyHash).HasMaxLength(200).IsRequired();
            builder.Property(p => p.UserUid).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
        }
    }
}
