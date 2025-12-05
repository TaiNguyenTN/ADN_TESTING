using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.Entity;
using Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public static class Seeder
    {
        public static async Task SeedASync(BookDbContext context)
        {
            // ===== CHECK DATABASE READY =====
            if (!await context.Database.CanConnectAsync())
                throw new Exception("Database is not available.");

            // ============================
            // 🟦 CREATE SERVICE
            // ============================
            var serviceName = "DNA Test Service";
            var service = await context.Services.FirstOrDefaultAsync(s => s.ServiceName == serviceName);

            if (service == null)
            {
                service = new Service(
                    serviceName: serviceName,
                    serviceType: "DNA",
                    description: "DNA testing service",
                    price: 1500000
                );

                await context.Services.AddAsync(service);
                await context.SaveChangesAsync();
            }

            // ============================
            // 🟦 CREATE CATEGORY
            // ============================
            var categoryName = "Basic DNA Category";
            var category = await context.TestCategories
                .FirstOrDefaultAsync(c => c.CategoryName == categoryName && c.ServiceId == service.ServiceId);

            if (category == null)
            {
                category = new TestCategory(
                    categoryName: categoryName,
                    description: "Default test category",
                    serviceId: service.ServiceId
                );

                await context.TestCategories.AddAsync(category);
                await context.SaveChangesAsync();
            }

            // ============================
            // 🟦 CREATE PURPOSE
            // ============================
            var purposeName = "Paternity Test";
            var purpose = await context.TestPurposes
                .FirstOrDefaultAsync(p => p.TestPurposeName == purposeName);

            if (purpose == null)
            {
                purpose = new TestPurpose(
                    testPurposeName: purposeName,
                    description: "Father-child DNA test"
                );

                await context.TestPurposes.AddAsync(purpose);
                await context.SaveChangesAsync();
            }

            // ============================
            // 🟦 LINK SERVICE - PURPOSE
            // ============================
            var spLinkExists = await context.ServiceTestPurposes
                .AnyAsync(sp => sp.ServiceId == service.ServiceId && sp.TestPurposeId == purpose.TestPurposeId);

            if (!spLinkExists)
            {
                var spLink = new ServiceTestPurpose(
                    serviceId: service.ServiceId,
                    testPurposeId: purpose.TestPurposeId
                );
                await context.ServiceTestPurposes.AddAsync(spLink);
                await context.SaveChangesAsync();
            }

            // ============================
            // 🟦 CREATE USER CONTRACT
            // ============================
            var userContract = new UserContract(
                identityNumber: "058204001767",
                fullName: "Test Patient",
                dob: new DateTime(1990, 1, 1),
                gender: "Male",
                phoneNumber: "0900000000",
                email: "test@example.com"
            );

            // ============================
            // 🟦 CREATE 3 APPOINTMENTS
            // ============================
            var appointments = new List<Appointment>
            {
                new Appointment(
                    identityNumber: "058204001767",
                    DateTime.Now.AddDays(1),
                    service.ServiceId,
                    category.TestCategoryId,
                    purpose.TestPurposeId,
                    province: "HCM",
                    district: "District 1",
                    collectionLocation: "Location A",
                    note: "Note 1"
                ),
                new Appointment(
                    identityNumber: "058204001767",
                    DateTime.Now.AddDays(2),
                    service.ServiceId,
                    category.TestCategoryId,
                    purpose.TestPurposeId,
                    province: "HCM",
                    district: "District 2",
                    collectionLocation: "Location B",
                    note: "Note 2"
                ),
                new Appointment(
                    identityNumber: "058204001767",
                    DateTime.Now.AddDays(3),
                    service.ServiceId,
                    category.TestCategoryId,
                    purpose.TestPurposeId,
                    province: "HCM",
                    district: "District 3",
                    collectionLocation: "Location C",
                    note: "Note 3"
                )
            };

            // Chỉ thêm những appointment chưa tồn tại
            foreach (var appt in appointments)
            {
                var exists = await context.Appointments.AnyAsync(a =>
                    a.IdentityNumber == appt.IdentityNumber &&
                    a.AppointmentDate.Date == appt.AppointmentDate.Date
                );

                if (!exists)
                    await context.Appointments.AddAsync(appt);
            }

            await context.SaveChangesAsync();
        }
    }
}
