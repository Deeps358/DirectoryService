using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Application.Departaments.Queries.GetRoots;
using DirectoryServices.Contracts.Departaments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Departaments.Queries.GetTopFiveByPositions
{
    public class GetTopFiveByPositionsHandler : IQueryHandler<GetTopFiveByPositionsDto[], GetTopFiveByPositionsQuery>
    {
        private readonly IReadDbContext _readDbContext;
        private readonly HybridCache _hybridCache;
        private readonly ILogger<GetTopFiveByPositionsHandler> _logger;

        public GetTopFiveByPositionsHandler(
            IReadDbContext readDbContext,
            HybridCache hybridCache,
            ILogger<GetTopFiveByPositionsHandler> logger)
        {
            _readDbContext = readDbContext;
            _hybridCache = hybridCache;
            _logger = logger;
        }

        public async Task<GetTopFiveByPositionsDto[]?> Handle(GetTopFiveByPositionsQuery query, CancellationToken cancellationToken)
        {
            return await GetTopFiveByPositions(query, cancellationToken);
        }

        private async Task<GetTopFiveByPositionsDto[]> GetTopFiveByPositions(
            GetTopFiveByPositionsQuery query,
            CancellationToken cancellationToken)
        {
            string topFiveDepsKey = "topfivedeps";

            GetTopFiveByPositionsDto[] topFiveDepsCache = await _hybridCache.GetOrCreateAsync<GetTopFiveByPositionsDto[]>(
                key: topFiveDepsKey,
                factory: async _ =>
                {
                    GetTopFiveByPositionsDto[] topFiveDepsResponce = await _readDbContext.DepartamentsRead
                        .Select(d => new GetTopFiveByPositionsDto
                        {
                            Id = d.Id.Value,
                            Name = d.Name.Value,
                            PositionsCount = d.Positions.Count,
                            IsActive = d.IsActive,
                        })
                        .OrderByDescending(d => d.PositionsCount)
                        .Take(5)
                        .ToArrayAsync(cancellationToken);

                    return topFiveDepsResponce;
                },
                cancellationToken: cancellationToken);

            return topFiveDepsCache;
        }
    }
}