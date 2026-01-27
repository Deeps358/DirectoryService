using Dapper;
using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Application.Departaments.Queries.GetChildrensById;
using DirectoryServices.Contracts.Departaments;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Departaments.Queries.GetRoots
{
    public class GetRootsWithChildrensHandler : IQueryHandler<GetRootsWithChildrensDto, GetRootsWithChildrensQuery>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly HybridCache _hybridCache;
        private readonly ILogger<GetRootsWithChildrensHandler> _logger;

        public GetRootsWithChildrensHandler(
            IDbConnectionFactory connectionFactory,
            HybridCache hybridCache,
            ILogger<GetRootsWithChildrensHandler> logger)
        {
            _connectionFactory = connectionFactory;
            _hybridCache = hybridCache;
            _logger = logger;
        }

        public async Task<GetRootsWithChildrensDto?> Handle(GetRootsWithChildrensQuery query, CancellationToken cancellationToken)
        {
            GetRootsWithChildrensDto roots = await GetDepWithChildrensFromCache(query, cancellationToken);
            return roots;
        }

        private async Task<GetRootsWithChildrensDto> GetDepWithChildrensFromCache(
            GetRootsWithChildrensQuery query,
            CancellationToken cancellationToken)
        {
            string rootsWithChildsFilter = $"{CacheConstants.DEPARTMENT_ROOTS_KEY}{query.Request.Size}_page_{query.Request.Page}_prefetch_{query.Request.Prefetch}";

            List<DepartamentDto> rootsWithChildsCache = await _hybridCache.GetOrCreateAsync<List<DepartamentDto>>(
                key: rootsWithChildsFilter,
                factory: async _ =>
                {
                    var sql = """
                    WITH roots AS (SELECT d.id, d."name", d.identifier, d.parent_id, d."path" , d."depth", d.is_active, d.created_at, d.updated_at  
                        FROM departaments d 
                        WHERE d.depth = 0 AND d.is_active = true
                        OFFSET @rootsOffset
                        LIMIT @size)
                    
                    SELECT *,
                        (EXISTS(SELECT 1 FROM departaments d WHERE d.parent_id = roots.id OFFSET @prefetch LIMIT 1)) AS has_more_childrens
                    FROM roots
                    
                    UNION ALL
                    
                    SELECT children.*, (EXISTS(SELECT 1 FROM departaments d WHERE d.parent_id = children.id )) AS has_more_childrens
                    FROM roots r
                    CROSS JOIN LATERAL (
                        SELECT d.id, d."name", d.identifier, d.parent_id, d."path" , d."depth", d.is_active, d.created_at, d.updated_at  
                        FROM departaments d
                        WHERE d.parent_id = r.id AND d.is_active  = true
                        ORDER BY d."name"
                        LIMIT @prefetch) children;
                    """;

                    var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

                    int rootsSkip = (query.Request.Page - 1 ?? 0) * (query.Request.Size ?? 20);

                    var depsWithChildrens = (await connection.QueryAsync<DepartamentDto>(
                        sql,
                        param: new
                        {
                            size = query.Request.Size ?? 20,
                            rootsOffset = rootsSkip,
                            prefetch = query.Request.Prefetch ?? 3,
                        })).ToList();

                    return depsWithChildrens;
                },
                tags: [ CacheConstants.DEPARTAMENTS_COMMON_KEY, CacheConstants.DEPARTMENT_ROOTS_KEY ],
                cancellationToken: cancellationToken);

            string roots_count = CacheConstants.DEPARTMENT_ROOTS_COUNT_KEY;

            int rootsCountCache = await _hybridCache.GetOrCreateAsync<int>(
                key: roots_count,
                factory: async _ =>
                {
                    var sql = """
                    SELECT COUNT(*) FROM departaments WHERE "depth" = 0;
                    """;

                    var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

                    int rootsCountResponce = await connection.ExecuteScalarAsync<int>(sql);

                    return rootsCountResponce;
                },
                tags: new List<string> { CacheConstants.DEPARTAMENTS_COMMON_KEY, CacheConstants.DEPARTMENT_ROOTS_COUNT_KEY },
                cancellationToken: cancellationToken);

            return new GetRootsWithChildrensDto(rootsWithChildsCache, rootsCountCache);
        }
    }
}