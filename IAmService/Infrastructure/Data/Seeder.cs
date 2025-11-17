using System;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Aggregate;

namespace Infrastructure.Data
{
    public static class Seeder
    {

        public static async Task SeedAdminRoleAsync(IAMDBContext context)
        {
            if (await context.Roles.AnyAsync(r => r.Code == "ADMIN")) return;

            var adminRole = new Role(
                "Administrator",
                "ADMIN",
                "System administrator role with full privileges"
            );

            context.Roles.Add(adminRole);
            await context.SaveChangesAsync();
            Console.WriteLine("Seeded ADMIN role successfully.");
        }
    }
}
