using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Infrastructure.Postgres.Services
{
    public class OldDepsDeletionService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OldDepsDeletionService> _logger;

        public OldDepsDeletionService(IServiceScopeFactory serviceScopeFactory, ILogger<OldDepsDeletionService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Сервис по очистке устаревших депов был запущен");

                using var scope = _serviceScopeFactory.CreateScope();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogCritical(ex, "Сервис по очистке устаревших депов был отменён");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при запуску сервиса по очистке устаревших депов");
                throw;
            }

            return Task.CompletedTask;
        }
    }
}