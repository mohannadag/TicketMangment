using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketMangment.ViewModel;

namespace TicketMangment.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<TicketLogs> TicketLogs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            ////Seeding a  'Administrator' role to AspNetRoles table
            //modelBuilder.Entity<IdentityRole>().HasData(
            //    new IdentityRole 
            //    {
            //        Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        Name = "Administrator",
            //        NormalizedName = "ADMINISTRATOR".ToUpper() 
            //    }
            // );


            ////a hasher to hash the password before seeding the user to the db
            //var hasher = new PasswordHasher<IdentityUser>();


            ////Seeding the User to AspNetUsers table
            //modelBuilder.Entity<IdentityUser>().HasData(
            //    new IdentityUser
            //    {
            //        Id = "8e445865-a24d-4543-a6c6-9443d048cdb9", // primary key
            //    UserName = "myuser",
            //        NormalizedUserName = "MYUSER",
            //        PasswordHash = hasher.HashPassword(null, "Pa$$w0rd")
            //    }
            //);


            ////Seeding the relation between our user and role to AspNetUserRoles table
            //modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            //    new IdentityUserRole<string>
            //    {
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
            //    }
            //);


            //modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 1,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "List Roles"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 2,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "Create Role"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 3,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "Edit Role"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 4,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "Delete Role"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 5,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "Manage Claims Role"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 6,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "List Roles"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 7,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Roles Mangment",
            //        ClaimType = "Edit Users In Role"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 8,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "Access Control Panel",
            //        ClaimType = "Access Control Panel"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 9,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "User Mangment",
            //        ClaimType = "Manage Claims User"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 10,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "User Mangment",
            //        ClaimType = "Users List"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 11,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "User Mangment",
            //        ClaimType = "Mange User Roles"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        Id = 12,
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        ClaimValue = "User Mangment",
            //        ClaimType = "Edit User"
            //    }
            //    );




            //IdentityRole role = new IdentityRole
            //{
            //    Name = "Developer",
            //    NormalizedName = "DEVELOPER"
            //};

            //IdentityUser user = new IdentityUser
            //{
            //    UserName = "system",
            //    NormalizedUserName = "SYSTEM",
            //    Email = "ag.mohannad@gmail.com",
            //    NormalizedEmail = "AG.MOHANNAD@GMAIL.COM",
            //    PasswordHash = "123456789"
            //};

            //foreach (var claim in ClaimsStore.AllClaims)
            //{
            //    await roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
            //}

            //builder.Entity<IdentityRole>().HasData(
            //        new IdentityRole
            //        {
            //            Name = role.Name,
            //            NormalizedName = role.NormalizedName
            //        }
            //    );

            //builder.Entity<IdentityUser>().HasData(
            //        new IdentityUser
            //        {
            //            UserName = user.UserName,
            //            NormalizedUserName = user.NormalizedUserName,
            //            Email = user.Email,
            //            NormalizedEmail = user.NormalizedEmail,
            //            PasswordHash = 
            //        }
            //    );





            //modelBuilder.Seed();
            
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
