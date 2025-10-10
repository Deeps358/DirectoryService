using DirectoryServices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            optionsBuilder.UseLoggerFactory(CreateLoggerFactory()); // логирование в консольку
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
        }

        private ILoggerFactory CreateLoggerFactory() =>
            LoggerFactory.Create(builder => { builder.AddConsole(); });

        public DbSet<Departament> Departaments => Set<Departament>();

        public DbSet<Location> Locations => Set<Location>();

        public DbSet<Position> Positions => Set<Position>();

        public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

        public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();
    }
}
