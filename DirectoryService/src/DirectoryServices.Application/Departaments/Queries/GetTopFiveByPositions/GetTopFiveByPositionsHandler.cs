using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts.Departaments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Departaments.Queries.GetTopFiveByPositions
{
    public class GetTopFiveByPositionsHandler : IQueryHandler<GetTopFiveByPositionsDto[], GetTopFiveByPositionsQuery>
    {
        private readonly IReadDbContext _readDbContext;
        private readonly ILogger<GetTopFiveByPositionsHandler> _logger;

        public GetTopFiveByPositionsHandler(IReadDbContext readDbContext, ILogger<GetTopFiveByPositionsHandler> logger)
        {
            _readDbContext = readDbContext;
            _logger = logger;
        }

        public async Task<GetTopFiveByPositionsDto[]?> Handle(GetTopFiveByPositionsQuery query, CancellationToken cancellationToken)
        {
            return await _readDbContext.DepartamentsRead
                .Select(d => new GetTopFiveByPositionsDto
                {
                    Id = d.Id.Value,
                    Name = d.Name.Value,
                    PositionsCount = d.Positions.Count,
                    IsActive = d.IsActive,
                })
                .OrderByDescending(d => d.PositionsCount)
                .Take(5)
                .ToArrayAsync();
        }
    }
}