using kartverket2025.Models.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace kartverket2025.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<MapReportModel> MapReport { get; set; }
        public DbSet<MapReportStatus> MapReportStatus { get; set; }
        public DbSet<MapPriorityStatus> MapPriorityStatus { get; set; }
        public DbSet<TileLayerModel> TileLayer { get; set; }
        

        //seed roles

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var sysAdminRoleId = "1";
            var caseHandlerRoleId = "2";
            var mapUserRoleId = "3";

            var roles = new List<IdentityRole>
    {
        new IdentityRole
        {
            Name = "System Admin",
            NormalizedName = "SYSTEM ADMIN",
            Id = sysAdminRoleId,
            ConcurrencyStamp = sysAdminRoleId
        },
        new IdentityRole
        {
            Name = "Case Handler",
            NormalizedName = "CASE HANDLER",
            Id = caseHandlerRoleId,
            ConcurrencyStamp = caseHandlerRoleId
        },
        new IdentityRole
        {
            Name = "Map User",
            NormalizedName = "MAP USER",
            Id = mapUserRoleId,
            ConcurrencyStamp = mapUserRoleId
        }
    };

            modelBuilder.Entity<IdentityRole>().HasData(roles); // 🔥 THIS LINE MAKES THE SEEDING WORK

            //    //seed MapUser
            // Seed users
            var sysAdminId = "1";
            var caseHandlerId = "2";
            var mapUserId = "3";

            var hasher = new PasswordHasher<ApplicationUser>();

            var sysAdminUser = new ApplicationUser
            {
                Id = sysAdminId,
                UserName = "systemadmin@test.com",
                NormalizedUserName = "SYSTEMADMIN@TEST.COM",
                Email = "systemadmin@test.com",
                NormalizedEmail = "SYSTEMADMIN@TEST.COM",
                FirstName = "System",
                LastName = "Admin",
            };
            sysAdminUser.PasswordHash = hasher.HashPassword(sysAdminUser, "systemadmin123@");

            var caseHandlerUser = new ApplicationUser
            {
                Id = caseHandlerId,
                UserName = "casehandler@test.com",
                NormalizedUserName = "CASEHANDLER@TEST.COM",
                Email = "casehandler@test.com",
                NormalizedEmail = "CASEHANDLER@TEST.COM",
                FirstName = "Case",
                LastName = "Handler",
            };
            caseHandlerUser.PasswordHash = hasher.HashPassword(caseHandlerUser, "casehandler123@");

            var mapUser = new ApplicationUser
            {
                Id = mapUserId,
                UserName = "mapuser@test.com",
                NormalizedUserName = "MAPUSER@TEST.COM",
                Email = "mapuser@test.com",
                NormalizedEmail = "MAPUSER@TEST.COM",
                FirstName = "Map",
                LastName = "User",
            };
            mapUser.PasswordHash = hasher.HashPassword(mapUser, "mapuser123@");

            // Seed the users
            modelBuilder.Entity<ApplicationUser>().HasData(sysAdminUser, caseHandlerUser, mapUser);

            // Assign roles to users
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = sysAdminId,
                    RoleId = sysAdminRoleId
                },
                new IdentityUserRole<string>
                {
                    UserId = caseHandlerId,
                    RoleId = caseHandlerRoleId
                },
                new IdentityUserRole<string>
                {
                    UserId = mapUserId,
                    RoleId = mapUserRoleId
                }
            );


            modelBuilder.Entity<MapReportModel>()
                .HasKey(m => m.Id);
        modelBuilder.Entity<MapReportStatus>()
                .HasKey(m => m.Id);
        modelBuilder.Entity<MapPriorityStatus>()
                .HasKey(m => m.Id);

          modelBuilder.Entity<MapReportModel>()
                    .HasOne(x => x.MapReportStatusModel)
                    .WithMany()
                    .HasForeignKey(i => i.MapReportStatusId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MapReportStatus>().HasData(
                new MapReportStatus { Id= 1, Status = "Under way..."},
                new MapReportStatus { Id = 2, Status = "On the way" },
                new MapReportStatus { Id = 3, Status = "Completed" },
                new MapReportStatus { Id = 4, Status = "Denied" }
                );
        modelBuilder.Entity<MapPriorityStatus>().HasData(
                new MapPriorityStatus { Id = 1, PriorityStatus = "Low" },
                new MapPriorityStatus { Id = 2, PriorityStatus = "Medium" },
                new MapPriorityStatus { Id = 3, PriorityStatus = "High" }
            );
            modelBuilder.Entity<TileLayerModel>().HasData(
                new TileLayerModel { Id = 1, KartType = "Topofarge"},
                new TileLayerModel { Id = 2, KartType = "Topogråtone" },
                new TileLayerModel { Id = 3, KartType = "Turkart" },
                new TileLayerModel { Id = 4, KartType = "Sjøkart" },
                new TileLayerModel { Id = 5, KartType = "Carto Light" },
                new TileLayerModel { Id = 6, KartType = "Carto Dark" }
            );

        }

    }
}

