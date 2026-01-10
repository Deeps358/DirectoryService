using CSharpFunctionalExtensions;
using DirectoryServices.Application.Departaments.Services.OldDepsDeletionService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Infrastructure.Postgres.Services
{
    public class OldDepsDeletionInitiolizerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OldDepsDeletionInitiolizerService> _logger;

        public OldDepsDeletionInitiolizerService(
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            ILogger<OldDepsDeletionInitiolizerService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var startTime = _configuration.GetValue<TimeSpan>("ClearOldDepsService:WorkTime");

            while(!cancellationToken.IsCancellationRequested)
            {
                var nextRun = DateTime.Now.Date.Add(startTime);
                if (DateTime.Now > nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - DateTime.Now;
                _logger.LogInformation($"Следующий запуск сервиса удаления старых депов через {delay.TotalHours} часов");

                await Task.Delay(delay, cancellationToken);

                await StartWorkAsync(cancellationToken);
            }
        }

        private async Task StartWorkAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Фоновый сервис по очистке устаревших депов был запущен");

                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                IDeletionService getService = scope.ServiceProvider.GetRequiredService<IDeletionService>();

                UnitResult<Error> serviceResult = await getService.Start(cancellationToken);

                _logger.LogInformation("Фоновый сервис по очистке устаревших депов отработал!");

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogCritical(ex, "Фоновый сервис по очистке устаревших депов был отменён");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при запуске фонового сервиса по очистке устаревших депов");
                throw;
            }
        }
    }
}