using Hospital.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Hospital.Infrastructure.Migrations
{
    public partial class _SeedRolesAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var adminId = Guid.NewGuid().ToString();
            var hasher = new PasswordHasher<User>();
            var passwordHash = hasher.HashPassword(null!, "Admin@123");

            // Insert admin user
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
            "Id", "FullName", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
            "PasswordHash", "Role", "CreatedAt", "UpdatedAt", "EmailConfirmed",
            "AccessFailedCount", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "SecurityStamp"
                },
                values: new object[]
                {
            adminId,
            "Admin",
            "admin",
            "ADMIN",
            "admin@Hospital.com",
            "ADMIN@HOSPITAL.COM",
            passwordHash,
            "Admin",
            DateTime.UtcNow,
            DateTime.UtcNow,
            true,
            0,
            false,
            false,
            false,
            Guid.NewGuid().ToString()
                }
            );

            // Assign role using string interpolation
            migrationBuilder.Sql($@"
        INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
        SELECT '{adminId}', [Id]
        FROM [AspNetRoles]
        WHERE [Name] = 'Admin';
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First, delete from AspNetUserRoles
            migrationBuilder.Sql(@"
        DELETE FROM [AspNetUserRoles]
        WHERE [UserId] IN (SELECT [Id] FROM [Users] WHERE [UserName] = 'admin');
    ");

            // Then, delete from Users table
            migrationBuilder.Sql("DELETE FROM [Users] WHERE [UserName] = 'admin';");
        }
    }
}
