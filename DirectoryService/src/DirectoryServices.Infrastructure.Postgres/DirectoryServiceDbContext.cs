using DirectoryServices.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryServices.Infrastructure.Postgres
{
    public class DirectoryServiceDbContext : DbContext
    {
        private readonly string _connectionString;

        public DirectoryServiceDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
        }

        public DbSet<Departament> Departaments => Set<Departament>();

        public DbSet<Location> Locations => Set<Location>();

        public DbSet<Position> Positions => Set<Position>();

        public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

        public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();
    }
}
