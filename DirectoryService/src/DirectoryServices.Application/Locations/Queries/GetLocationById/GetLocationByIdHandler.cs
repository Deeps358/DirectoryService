using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities.ValueObjects.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Locations.Queries.GetLocationById
{
    public class GetLocationByIdHandler : IQueryHandler<GetLocationDto, GetLocationByIdQuery>
    {
        private readonly IReadDbContext _readDbContext;
        private readonly ILogger<GetLocationByIdHandler> _logger;

        public GetLocationByIdHandler(IReadDbContext readDbContext, ILogger<GetLocationByIdHandler> logger)
        {
            _readDbContext = readDbContext;
            _logger = logger;
        }

        public async Task<GetLocationDto?> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken)
        {
            var location = await _readDbContext.LocationsRead
                .Include(l => l.DepartmentLocations)
                .FirstOrDefaultAsync(l => l.Id == LocId.GetCurrent(query.LocationId), cancellationToken);

            if(location is null)
            {
                return null;
            }

            return new GetLocationDto()
            {
                Id = location.Id.Value,
                Name = location.Name.Value,
                City = location.Adress.City,
                Street = location.Adress.Street,
                Building = location.Adress.Building,
                Room = location.Adress.Room,
                Timezone = location.Timezone.Value,
                IsActive = location.IsActive,
                DepartmentLocations = location.DepartmentLocations.Select(dl => dl.DepartamentId.Value).ToArray(),
                CreatedAt = location.CreatedAt,
                UpdatedAt = location.UpdatedAt,
            };
        }
    }
}