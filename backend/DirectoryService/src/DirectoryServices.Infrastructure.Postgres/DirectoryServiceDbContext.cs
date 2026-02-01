using DirectoryServices.Application.Database;
using DirectoryServices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Infrastructure.Postgres
{
    public class DirectoryServiceDbContext : DbContext, IReadDbContext
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
            modelBuilder.HasPostgresExtension("ltree");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
        }

        private ILoggerFactory CreateLoggerFactory() =>
            LoggerFactory.Create(builder => { builder.AddConsole(); });

        public DbSet<Departament> Departaments => Set<Departament>();

        public DbSet<Location> Locations => Set<Location>();

        public DbSet<Position> Positions => Set<Position>();

        public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

        public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();

        public IQueryable<Departament> DepartamentsRead => Set<Departament>().AsNoTracking();

        public IQueryable<Location> LocationsRead => Set<Location>().AsNoTracking();

        public IQueryable<Position> PositionsRead => Set<Position>().AsNoTracking();

        public IQueryable<DepartmentLocation> DepartmentLocationsRead => Set<DepartmentLocation>().AsNoTracking();

        public IQueryable<DepartmentPosition> DepartmentPositionsRead => Set<DepartmentPosition>().AsNoTracking();
    }
}
