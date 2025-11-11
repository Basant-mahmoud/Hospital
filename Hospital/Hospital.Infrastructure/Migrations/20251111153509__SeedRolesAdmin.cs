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
            var adminUser = new User
            {
                Id = adminId,
                FullName = "Admin",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@Hospital.com",
                NormalizedEmail = "ADMIN@HOSPITAL.COM",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true,
                AccessFailedCount = 0,         
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false
            };

            var passwordHash = hasher.HashPassword(adminUser, "Admin@123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "Id", "FullName", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
                    "PasswordHash", "Role", "CreatedAt", "UpdatedAt", "EmailConfirmed",
                    "AccessFailedCount", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled"
                },
                values: new object[]
                {
                    adminUser.Id,
                    adminUser.FullName,
                    adminUser.UserName,
                    adminUser.NormalizedUserName,
                    adminUser.Email,
                    adminUser.NormalizedEmail,
                    passwordHash,
                    adminUser.Role,
                    adminUser.CreatedAt,
                    adminUser.UpdatedAt,
                    adminUser.EmailConfirmed,
                    adminUser.AccessFailedCount,
                    adminUser.PhoneNumberConfirmed,
                    adminUser.TwoFactorEnabled,
                    adminUser.LockoutEnabled
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Users] WHERE Role = 'Admin'");
        }
    }
}
