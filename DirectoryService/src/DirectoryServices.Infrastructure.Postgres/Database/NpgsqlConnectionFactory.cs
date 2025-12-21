using System.Data;
using DirectoryServices.Application.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryServices.Infrastructure.Postgres.Database
{
    public class NpgsqlConnectionFactory : IDisposable, IAsyncDisposable, IDbConnectionFactory
    {
        private readonly NpgsqlDataSource _dataSource;

        public NpgsqlConnectionFactory(IConfiguration configuration)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DirectoryServiceDevDb"));
            dataSourceBuilder.UseLoggerFactory(CreateLoggerFactory());

            _dataSource = dataSourceBuilder.Build();
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            return await _dataSource.OpenConnectionAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dataSource.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _dataSource.DisposeAsync();
        }

        private ILoggerFactory CreateLoggerFactory() =>
            LoggerFactory.Create(builder => { builder.AddConsole(); });
    }
}