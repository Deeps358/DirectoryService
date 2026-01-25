using Dapper;
using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts.Departaments;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Departaments.Queries.GetChildrensById
{
    public class GetChildrensByIdHandler : IQueryHandler<DepartamentDto[], GetChildrensByIdQuery>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly HybridCache _hybridCache;
        private readonly ILogger<GetChildrensByIdHandler> _logger;

        public GetChildrensByIdHandler(
            IDbConnectionFactory connectionFactory,
            HybridCache hybridCache,
            ILogger<GetChildrensByIdHandler> logger)
        {
            _connectionFactory = connectionFactory;
            _hybridCache = hybridCache;
            _logger = logger;
        }

        public async Task<DepartamentDto[]?> Handle(GetChildrensByIdQuery query, CancellationToken cancellationToken)
        {
            var childrens = GetDepWithChildrensFromCache(query, cancellationToken);
            return await childrens;
        }

        private async Task<DepartamentDto[]> GetDepWithChildrensFromCache(
            GetChildrensByIdQuery query,
            CancellationToken cancellationToken)
        {
            string requestFilter = $"depid_{query.ParentId}_size_{query.Request.Size}_page_{query.Request.Page}";

            var depsCache = await _hybridCache.GetOrCreateAsync<DepartamentDto[]>(
                key: requestFilter,
                factory: async _ =>
                {
                    var sql = """
                    with childrens as (SELECT d.id, d."name", d.identifier, d.parent_id, d."path" , d."depth", d.is_active, d.created_at, d.updated_at  
                        FROM departaments d 
                        WHERE d.parent_id = @parentId
                        offset @childrensOffset
                        limit @size)
                    select *,
                        (exists(select 1 from departaments d where d.parent_id = childrens.id limit 1)) as has_more_childrens
                    from childrens
                    """;

                    var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

                    ValueTask.FromResult<DepartamentDto[]>(null);

                    DepartamentDto[] depsChildrensById = (await connection.QueryAsync<DepartamentDto>(
                    sql,
                    param: new
                    {
                        parentId = query.ParentId,
                        size = query.Request.Size ?? 20,
                        childrensOffset = (query.Request.Page - 1 ?? 0) * (query.Request.Size ?? 20),
                    })).ToArray();

                    return depsChildrensById;
                },
                cancellationToken: cancellationToken);

            return depsCache;
        }
    }
}