
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.SensorsData;

namespace Project.Models
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Device> Devices {  get; set; }
        public DbSet<Room> Rooms {  get; set; }
        public DbSet<RoomDevice> RoomDevices { get; set; }
        public DbSet<SensorData_MotionDetection> MotionData {  get; set; }
        public DbSet<SensorData_WaterLevel> WaterLevelData {  get; set; }
        public DbSet<SensorData_EnergyUsage> EnergyUsageData {  get; set; }
        public DbSet<SensorData_LightDetection> LightData {  get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RoomDevice>()
            .HasKey(mg => new { mg.RoomId, mg.DeviceId });

            builder.Entity<RoomDevice>()
                .HasOne(mg => mg.Room)
                .WithMany(m => m.RoomDevices)
                .HasForeignKey(mg => mg.DeviceId);

            builder.Entity<RoomDevice>()
                 .HasOne(mg => mg.Device)
                 .WithMany(g => g.RoomDevices)
                 .HasForeignKey(mg => mg.RoomId);

            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users", "security");
            builder.Entity<IdentityRole>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "security");
        }
    }
}
