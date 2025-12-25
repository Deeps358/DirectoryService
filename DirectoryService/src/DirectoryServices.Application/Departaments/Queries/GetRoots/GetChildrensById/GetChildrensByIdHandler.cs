using Dapper;
using DirectoryServices.Application.Abstractions;
using DirectoryServices.Application.Database;
using DirectoryServices.Contracts.Departaments;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Application.Departaments.Queries.GetRoots.GetChildrensById
{
    public class GetChildrensByIdHandler : IQueryHandler<DepartamentDto[], GetChildrensByIdQuery>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<GetChildrensByIdHandler> _logger;

        public GetChildrensByIdHandler(IDbConnectionFactory connectionFactory, ILogger<GetChildrensByIdHandler> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<DepartamentDto[]?> Handle(GetChildrensByIdQuery query, CancellationToken cancellationToken)
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

            var depsChildrensById = (await connection.QueryAsync<DepartamentDto>(
                sql,
                param: new
                {
                    parentId = query.ParentId,
                    size = query.Request.Size ?? 20,
                    childrensOffset = (query.Request.Page - 1 ?? 0) * (query.Request.Size ?? 20),
                })).ToArray();

            return depsChildrensById;
        }
    }
}