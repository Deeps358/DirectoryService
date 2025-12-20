using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Application.Locations.Queries.GetLocationById;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Locations.Queries.GetLocations
{
    public class GetLocationsHandler : IQueryHandler<GetLocationsDto, GetLocationQuery>
    {
        private readonly IReadDbContext _readDbContext;
        private readonly ILogger<GetLocationByIdHandler> _logger;

        public GetLocationsHandler(IReadDbContext readDbContext, ILogger<GetLocationByIdHandler> logger)
        {
            _readDbContext = readDbContext;
            _logger = logger;
        }

        public async Task<GetLocationsDto> Handle(GetLocationQuery query, CancellationToken cancellationToken)
        {
            var locationsQuery = _readDbContext.LocationsRead;

            if(!string.IsNullOrWhiteSpace(query.Request.Search))
                locationsQuery = locationsQuery.Where(l => EF.Functions.Like(l.Name.Value.ToLower(), $"%{query.Request.Search.ToLower()}%"));

            if(query.Request.IsActive.HasValue)
                locationsQuery = locationsQuery.Where(l => l.IsActive == query.Request.IsActive);

            if(query.Request.DepartamentIds != null && query.Request.DepartamentIds.Length > 0)
            {
                var depIds = query.Request.DepartamentIds.Select(DepId.GetCurrent).ToList();
                locationsQuery = locationsQuery
                    .Where(l => l.DepartmentLocations
                        .Any(dl => depIds
                            .Contains(dl.DepartamentId)));
            }

            long totalCount = await locationsQuery.LongCountAsync(cancellationToken);

            if(!string.IsNullOrWhiteSpace(query.Request.SortBy))
            {
                switch(query.Request.SortBy)
                {
                    case "Name":
                        locationsQuery = locationsQuery.OrderBy(l => l.Name);
                        break;
                    case "Creation":
                        locationsQuery = locationsQuery.OrderBy(l => l.CreatedAt);
                        break;
                }
            }

            int pagesToSkip = 0;

            if (query.Request.Page != null && query.Request.Page > 0)
                pagesToSkip = query.Request.Page.Value - 1;

            locationsQuery = locationsQuery
                .Skip(pagesToSkip * (query.Request.PageSize ?? 1))
                .Take(query.Request.PageSize ?? 20);

            var locations = await locationsQuery
                .Select(l => new LocationDto
                {
                    Id = l.Id.Value,
                    Name = l.Name.Value,
                    Adress = new AdressDto(l.Adress.City, l.Adress.Street, l.Adress.Building, l.Adress.Room),
                    Timezone = l.Timezone.Value,
                    IsActive = l.IsActive,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                })
                .ToListAsync(cancellationToken);

            return new GetLocationsDto(locations,  totalCount);
        }
    }
}