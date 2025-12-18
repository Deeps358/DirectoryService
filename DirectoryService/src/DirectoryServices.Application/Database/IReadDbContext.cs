using DirectoryServices.Entities;

namespace DirectoryServices.Application.Database
{
    public interface IReadDbContext
    {
        IQueryable<Location> LocationsRead { get; }
    }
}
