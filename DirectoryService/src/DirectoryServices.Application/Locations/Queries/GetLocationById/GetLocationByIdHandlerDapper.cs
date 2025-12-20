using System.Text.Json;
using Dapper;
using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts;
using DirectoryServices.Contracts.Locations;

namespace DirectoryServices.Application.Locations.Queries.GetLocationById
{

    public class GetLocationByIdHandlerDapper : IQueryHandler<GetLocationDto, GetLocationByIdQuery>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetLocationByIdHandlerDapper(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<GetLocationDto?> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken)
        {
            GetLocationDto? locationDto = null;

            var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            await connection.QueryAsync<GetLocationDto, string, DepartamentLocationsDto, GetLocationDto>(
                """
                SELECT  l.id,
                        l.name,
                        l.timezone,
                        l.is_active,
                        l.created_at,
                        l.updated_at,
                        l.adress::text,
                        dl.departament_id,
                        dl.location_id,
                        dl."CreatedAt",
                        dl."UpdatedAt"
                FROM locations l
                JOIN departament_locations dl ON dl.location_id = l.id
                WHERE l.id = @locId
                """,
                param: new
                {
                    locId = query.LocationId
                },
                splitOn: "adress, departament_id",
                map: (l, adressJson, dl) =>
                {
                    locationDto ??= l with
                    {
                        Adress = JsonSerializer.Deserialize<AdressDto>(adressJson)
                    };

                    locationDto.DepartmentLocations.Add(dl);

                    return locationDto;
                });

            return locationDto;
        }
    }
}