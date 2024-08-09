
using Microsoft.EntityFrameworkCore;

namespace Aeropuerto.Controler;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {

    }
    public DbSet<UserModel> User { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Seat> Seats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Reservation>().HasOne(f => f.userModel).WithMany(f => f.reservations).HasForeignKey(f => f.UserCI);
        modelBuilder.Entity<Reservation>().HasOne(f => f.flight).WithMany(f => f.Freservations).HasForeignKey(f => f.FlightID);
        modelBuilder.Entity<Role>().HasOne(f => f.userModel).WithOne(f => f.rol).HasForeignKey<Role>(f => f.UserCI);
        modelBuilder.Entity<Seat>().HasOne(f => f.flight).WithMany(f => f.Seats).HasForeignKey(f => f.IDFlight);

    }

}

