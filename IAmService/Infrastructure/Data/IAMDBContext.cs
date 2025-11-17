using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.Entities;
using Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class IAMDBContext : DbContext
    {
        public IAMDBContext(DbContextOptions<IAMDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<UserPrivilege> UserPrivileges { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Address và Password là Value Object(đối tượng sở hữu - Owned entity type).
            //không phải một bảng riêng trong database, mà sẽ được lưu kèm
            //trong bảng của entity chứa nó.
            modelBuilder.Owned<Password>(); //Nếu sử dụng cách này hoặc modelBuilder.Entity<User>().OwnsOne(u => u.Address);
            modelBuilder.Owned<Address>();  //EF hiểu rằng Address là một owned type, tức là nó không có bảng riêng trong database.
                                            //Các property của Address sẽ được lưu kèm trong bảng Users(với tên cột kiểu Address_Street, Address_City, ...).
                                            //Tên coloumn = Tên Value Object + "_" + Tên property.
                                            //User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Dob).IsRequired();
                entity.Property(u => u.Gender).IsRequired();
                entity.Property(u => u.PhoneNumber).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.IsActive).HasDefaultValue(true);

                entity.HasMany(u => u.UserRoles)
                        .WithOne(ur => ur.User)
                        .HasForeignKey(ur => ur.UserId);

                var userRolesNav = entity.Metadata.FindNavigation(nameof(User.UserRoles))!;
                userRolesNav.SetField("userRoles");
                userRolesNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(u => u.UserPrivileges)
                        .WithOne(up => up.User)
                        .HasForeignKey(up => up.UserId);

                var userPrivilegesNav = entity.Metadata.FindNavigation(nameof(User.UserPrivileges))!;
                userPrivilegesNav.SetField("userPrivileges");
                userPrivilegesNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                entity.HasOne(u => u.RefreshToken)
                        .WithOne(rt => rt.User)
                        .HasForeignKey<RefreshToken>(rt => rt.UserId);

                //// Config Value Object Address
                //entity.OwnsOne(u => u.Address, address =>
                //{
                //    address.Property(a => a.Street).HasColumnName("Street");
                //    address.Property(a => a.City).HasColumnName("City");
                //    address.Property(a => a.State).HasColumnName("State");
                //    address.Property(a => a.ZipCode).HasColumnName("ZipCode");
                //    address.Property(a => a.Country).HasColumnName("Country");
                //});

                //// Config Value Object Password
                //entity.OwnsOne(u => u.Password, password =>
                //{
                //    password.Property(p => p.Hashed).HasColumnName("PasswordHash").IsRequired();
                //    password.Property(p => p.Salt).HasColumnName("PasswordSalt");
                //});

                //Sử dụng cách này nếu muốn tùy chỉnh tên coloumn của Value Object theo ý muốn.
            });

            //Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);

                entity.Property(r => r.Code).IsRequired();
                entity.Property(r => r.Name).IsRequired();
                entity.Property(r => r.Description).IsRequired();
                entity.HasMany(r => r.RolePrivileges)
                        .WithOne(rp => rp.Role)
                        .HasForeignKey(rp => rp.RoleId);

                entity.HasMany(r => r.UserRoles)
                      .WithOne(ur => ur.Role)
                      .HasForeignKey(ur => ur.RoleId);
            });

            

            //RefreshToken configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.RefreshTokenID);
                entity.Property(rt => rt.UserId).IsRequired();
                entity.Property(rt => rt.Token).IsRequired();
                entity.Property(rt => rt.ExpiresAt).IsRequired();
                entity.Property(rt => rt.CreateAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(rt => rt.IsRevoked).HasDefaultValue(false);

                //One-to-one relation: User <-> RefreshToken
                entity.HasOne(rt => rt.User)
                      .WithOne(u => u.RefreshToken)
                      .HasForeignKey<RefreshToken>(rt => rt.UserId);
            });

            //AuditLog configuration
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

            //UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => ur.UserRoleId);
                entity.Property(ur => ur.UserId).IsRequired();
                entity.Property(ur => ur.RoleId).IsRequired();
                entity.Property(ur => ur.IsActive).HasDefaultValue(true);
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId);
            });

            //UserPrivilege configuration
            modelBuilder.Entity<UserPrivilege>(entity =>
            {
                entity.HasKey(up => up.UserPrivilegeId);
                entity.Property(up => up.UserId).IsRequired();
                entity.Property(up => up.PrivilegeId).IsRequired();
                entity.Property(up => up.IsGranted).HasDefaultValue(true);
                entity.HasOne(up => up.User)
                      .WithMany(u => u.UserPrivileges)
                      .HasForeignKey(up => up.UserId);

                entity.HasOne(up => up.Privilege)
                    .WithMany(p => p.UserPrivileges)
                    .HasForeignKey(up => up.PrivilegeId);
            });

            //Privilege configuration
            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.HasKey(p => p.PrivilegeId);
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Description).IsRequired();

                entity.HasMany(p => p.UserPrivileges)
                    .WithOne(up => up.Privilege)
                    .HasForeignKey(up => up.PrivilegeId);

                // UserPrivileges
                var userPrivNav = entity.Metadata
                    .FindNavigation(nameof(Privilege.UserPrivileges))!;
                userPrivNav.SetField("_userPrivileges");
                userPrivNav.SetPropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(p => p.RolePrivileges)
                    .WithOne(rp => rp.Privilege)
                    .HasForeignKey(rp => rp.PrivilegeId);

                // RolePrivileges
                var rolePrivNav = entity.Metadata
                    .FindNavigation(nameof(Privilege.RolePrivileges))!;
                rolePrivNav.SetField("_rolePrivileges");
                rolePrivNav.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            //RolePrivilege configuration
            modelBuilder.Entity<RolePrivilege>(entity =>
            {
                entity.HasKey(rp => rp.RolePrivilegeId);
                entity.Property(rp => rp.RoleId).IsRequired();
                entity.Property(rp => rp.PrivilegeId).IsRequired();
                entity.Property(rp => rp.IsActive).HasDefaultValue(true);

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePrivileges)
                      .HasForeignKey(rp => rp.RoleId);

                entity.HasOne(rp => rp.Privilege)
                        .WithMany(p => p.RolePrivileges)
                        .HasForeignKey(rp => rp.PrivilegeId);
            });

        }
    }
}
