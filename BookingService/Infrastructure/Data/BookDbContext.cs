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
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestPurpose> TestPurposes { get; set; }
        public DbSet<ServiceTestPurpose> ServiceTestPurposes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //================
            // Appointment
            //================
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");
                entity.HasKey(e => e.AppointmentId);

                entity.Property(e => e.Province).IsRequired();
                entity.Property(e => e.District).IsRequired();
                entity.Property(e => e.CollectionLocation).IsRequired();
                entity.Property(e => e.Note).IsRequired(false);
                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .IsRequired();
                entity.Property(e => e.FingerprintFile).IsRequired(false);

                // FK relationships
                entity.HasOne(a => a.Service)
                      .WithMany(s => s.Appointments)
                      .HasForeignKey(a => a.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict); // important, avoid cascade path

                entity.HasOne(a => a.TestCategory)  
                      .WithMany(tc => tc.Appointments)
                      .HasForeignKey(a => a.TestCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.TestPurpose)
                      .WithMany(tp => tp.Appointments)
                      .HasForeignKey(a => a.TestPurposeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            //================
            // Audit Logs
            //================
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(al => al.AuditLogId);
                entity.Property(al => al.EntityName).IsRequired();
                entity.Property(al => al.Action).IsRequired();
                entity.Property(al => al.PerformedBy).HasMaxLength(100);
                entity.Property(al => al.OldValue);
                entity.Property(al => al.NewValue);
                entity.Property(al => al.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            });

            //================
            // Service
            //================
            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");
                entity.HasKey(e => e.ServiceId);
                entity.Property(e => e.ServiceName).IsRequired();
                entity.Property(e => e.ServiceType).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();

                // Navigation
                entity.HasMany(s => s.TestCategories)
                      .WithOne(tc => tc.Service)
                      .HasForeignKey(tc => tc.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict); // important

                entity.HasMany(s => s.ServiceTestPurposes)
                      .WithOne(stp => stp.Service)
                      .HasForeignKey(stp => stp.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(s => s.Appointments)
                      .WithOne(a => a.Service)
                      .HasForeignKey(a => a.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Backing fields
                var categoriesNav = entity.Metadata.FindNavigation(nameof(Service.TestCategories));
                categoriesNav.SetField("categories");
                categoriesNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                var serviceTestPurposesNav = entity.Metadata.FindNavigation(nameof(Service.ServiceTestPurposes));
                serviceTestPurposesNav.SetField("serviceTestPurposes");
                serviceTestPurposesNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                var appointmentsNav = entity.Metadata.FindNavigation(nameof(Service.Appointments));
                appointmentsNav.SetField("appointments");
                appointmentsNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                // Unique index
                entity.HasIndex(s => s.ServiceName).IsUnique();
            });

            //================
            // TestCategory
            //================
            modelBuilder.Entity<TestCategory>(entity =>
            {
                entity.ToTable("TestCategory");
                entity.HasKey(e => e.TestCategoryId);
                entity.Property(e => e.CategoryName).IsRequired();
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired();

                entity.HasOne(tc => tc.Service)
                      .WithMany(s => s.TestCategories)
                      .HasForeignKey(tc => tc.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(tc => tc.Appointments)
                      .WithOne(a => a.TestCategory)
                      .HasForeignKey(a => a.TestCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Backing field
                var appointmentsNav = entity.Metadata.FindNavigation(nameof(TestCategory.Appointments));
                appointmentsNav.SetField("appointments");
                appointmentsNav.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            //================
            // TestPurpose
            //================
            modelBuilder.Entity<TestPurpose>(entity =>
            {
                entity.ToTable("TestPurpose");
                entity.HasKey(e => e.TestPurposeId);
                entity.Property(e => e.TestPurposeName).IsRequired();
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired();

                entity.HasMany(tp => tp.Appointments)
                      .WithOne(a => a.TestPurpose)
                      .HasForeignKey(a => a.TestPurposeId)
                      .OnDelete(DeleteBehavior.Restrict);

                var appointmentsNav = entity.Metadata.FindNavigation(nameof(TestPurpose.Appointments));
                appointmentsNav.SetField("_appointments");
                appointmentsNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(tp => tp.ServiceTestPurposes)
                      .WithOne(stp => stp.TestPurpose)
                      .HasForeignKey(stp => stp.TestPurposeId)
                      .OnDelete(DeleteBehavior.Restrict);

                var serviceTestPurposesNav = entity.Metadata.FindNavigation(nameof(TestPurpose.ServiceTestPurposes));
                serviceTestPurposesNav.SetField("_serviceTestPurposes");
                serviceTestPurposesNav.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            //================
            // ServiceTestPurpose
            //================
            modelBuilder.Entity<ServiceTestPurpose>(entity =>
            {
                entity.ToTable("ServiceTestPurpose");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsActive).IsRequired();

                entity.HasOne(stp => stp.Service)
                      .WithMany(s => s.ServiceTestPurposes)
                      .HasForeignKey(stp => stp.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(stp => stp.TestPurpose)
                      .WithMany(tp => tp.ServiceTestPurposes)
                      .HasForeignKey(stp => stp.TestPurposeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}
