using Microsoft.EntityFrameworkCore;
using Repositories.Models;

namespace Repositories
{
    public class FacilityBookingDBContext : DbContext
    {

        public FacilityBookingDBContext(DbContextOptions<FacilityBookingDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomSlot> RoomSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
