using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chronicle.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a71bdca4-500b-4bd4-9a40-4ce084883d6a", "1f8c2d90-0001-4a1a-9c3e-aaaa00000001", "Admin", "ADMIN" },
                    { "b845423f-422d-4235-9f6b-76f2d22d2f2d", "1f8c2d90-0003-4a1a-9c3e-aaaa00000003", "Writer", "WRITER" },
                    { "c577002b-a010-449e-990c-99c0d10c1d1a", "1f8c2d90-0002-4a1a-9c3e-aaaa00000002", "Revisor", "REVISOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e3b0c442-98fc-4c14-9afb-f4c8996fb924", 0, "1f8c2d90-0005-4a1a-9c3e-aaaa00000005", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAIAAYagAAAAEDp/kcQLbkjSpcGPMCe53mSXVhjieDabahZbX0fvZYwxdV7HUUmQVQ8hjedPpGvaJw==", null, false, "1f8c2d90-0004-4a1a-9c3e-aaaa00000004", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a71bdca4-500b-4bd4-9a40-4ce084883d6a", "e3b0c442-98fc-4c14-9afb-f4c8996fb924" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b845423f-422d-4235-9f6b-76f2d22d2f2d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c577002b-a010-449e-990c-99c0d10c1d1a");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a71bdca4-500b-4bd4-9a40-4ce084883d6a", "e3b0c442-98fc-4c14-9afb-f4c8996fb924" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a71bdca4-500b-4bd4-9a40-4ce084883d6a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e3b0c442-98fc-4c14-9afb-f4c8996fb924");
        }
    }
}
