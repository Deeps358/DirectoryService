using Dapper;
using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts;
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
            return await _readDbContext.LocationsRead
                .Include(l => l.DepartmentLocations)
                .Where(l => l.Id == LocId.GetCurrent(query.LocationId))
                .Select(l => new GetLocationDto()
                {
                    Id = l.Id.Value,
                    Name = l.Name.Value,
                    Adress = new AdressDto(l.Adress.City, l.Adress.Street, l.Adress.Building, l.Adress.Room),
                    Timezone = l.Timezone.Value,
                    IsActive = l.IsActive,
                    DepartmentLocations = _readDbContext.DepartmentLocationsRead
                        .Where(dl => dl.LocationId == l.Id)
                        .Select(dl => new DepartamentLocationsDto
                        {
                            DepartamentId = dl.DepartamentId.Value,
                            LocationId = dl.LocationId.Value,
                            CreatedAt = dl.CreatedAt,
                            UpdatedAt = dl.UpdatedAt,
                        }).ToList(),
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}