using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateExpireToken = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateDeadToken = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "RoleTypes",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTypes", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "SecretPolicies",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsUpdate = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretPolicies", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "StoragePolicies",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsCreate = table.Column<bool>(type: "boolean", nullable: false),
                    IsUpdate = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    IsCommon = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoragePolicies", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCommon = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(183)", maxLength: 183, nullable: false),
                    Login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshTokenUid = table.Column<Guid>(type: "uuid", nullable: true),
                    IsUserPass = table.Column<bool>(type: "boolean", nullable: false),
                    IsApiKey = table.Column<bool>(type: "boolean", nullable: false),
                    IsOAuth = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsCreateStorage = table.Column<bool>(type: "boolean", nullable: false),
                    Code2FA = table.Column<string>(type: "text", nullable: false),
                    IsActiveCode = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_Users_RefreshTokens_RefreshTokenUid",
                        column: x => x.RefreshTokenUid,
                        principalTable: "RefreshTokens",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    RoleTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_Roles_RoleTypes_RoleTypeUid",
                        column: x => x.RoleTypeUid,
                        principalTable: "RoleTypes",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secrets",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StorageUid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secrets", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_Secrets_Storages_StorageUid",
                        column: x => x.StorageUid,
                        principalTable: "Storages",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    KeyHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Users_UserUid",
                        column: x => x.UserUid,
                        principalTable: "Users",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    UserUid = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DateAct = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_Audit_Users_UserUid",
                        column: x => x.UserUid,
                        principalTable: "Users",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorageLinkPolicies",
                columns: table => new
                {
                    StorageUid = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleUid = table.Column<Guid>(type: "uuid", nullable: false),
                    StoragePolicyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLinkPolicies", x => new { x.RoleUid, x.StorageUid });
                    table.ForeignKey(
                        name: "FK_StorageLinkPolicies_Roles_RoleUid",
                        column: x => x.RoleUid,
                        principalTable: "Roles",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorageLinkPolicies_StoragePolicies_StoragePolicyUid",
                        column: x => x.StoragePolicyUid,
                        principalTable: "StoragePolicies",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorageLinkPolicies_Storages_StorageUid",
                        column: x => x.StorageUid,
                        principalTable: "Storages",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserUid = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleUid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserUid, x.RoleUid });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleUid",
                        column: x => x.RoleUid,
                        principalTable: "Roles",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserUid",
                        column: x => x.UserUid,
                        principalTable: "Users",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecretLinkPolicies",
                columns: table => new
                {
                    SecretUid = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleUid = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretPolicyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretLinkPolicies", x => new { x.RoleUid, x.SecretUid });
                    table.ForeignKey(
                        name: "FK_SecretLinkPolicies_Roles_RoleUid",
                        column: x => x.RoleUid,
                        principalTable: "Roles",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecretLinkPolicies_SecretPolicies_SecretPolicyUid",
                        column: x => x.SecretPolicyUid,
                        principalTable: "SecretPolicies",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecretLinkPolicies_Secrets_SecretUid",
                        column: x => x.SecretUid,
                        principalTable: "Secrets",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WhiteListIps",
                columns: table => new
                {
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Ip = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ApiKeyUid = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiteListIps", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_WhiteListIps_ApiKeys_ApiKeyUid",
                        column: x => x.ApiKeyUid,
                        principalTable: "ApiKeys",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UserUid",
                table: "ApiKeys",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_UserUid",
                table: "Audit",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleTypeUid",
                table: "Roles",
                column: "RoleTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_SecretLinkPolicies_SecretPolicyUid",
                table: "SecretLinkPolicies",
                column: "SecretPolicyUid");

            migrationBuilder.CreateIndex(
                name: "IX_SecretLinkPolicies_SecretUid",
                table: "SecretLinkPolicies",
                column: "SecretUid");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_StorageUid",
                table: "Secrets",
                column: "StorageUid");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLinkPolicies_StoragePolicyUid",
                table: "StorageLinkPolicies",
                column: "StoragePolicyUid");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLinkPolicies_StorageUid",
                table: "StorageLinkPolicies",
                column: "StorageUid");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleUid",
                table: "UserRoles",
                column: "RoleUid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RefreshTokenUid",
                table: "Users",
                column: "RefreshTokenUid");

            migrationBuilder.CreateIndex(
                name: "IX_WhiteListIps_ApiKeyUid",
                table: "WhiteListIps",
                column: "ApiKeyUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "SecretLinkPolicies");

            migrationBuilder.DropTable(
                name: "StorageLinkPolicies");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "WhiteListIps");

            migrationBuilder.DropTable(
                name: "SecretPolicies");

            migrationBuilder.DropTable(
                name: "Secrets");

            migrationBuilder.DropTable(
                name: "StoragePolicies");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Storages");

            migrationBuilder.DropTable(
                name: "RoleTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
