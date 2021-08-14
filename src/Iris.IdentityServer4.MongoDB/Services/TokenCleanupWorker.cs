using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Iris.IdentityServer4.MongoDB.Services
{
    /// <summary>
    /// TokenCleanupWorker
    /// </summary>
    public class TokenCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupWorker> _logger;
        public int CleanupInterval = 3600;

        /// <summary>
        /// TokenCleanupWorker
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public TokenCleanupWorker(
            IServiceProvider serviceProvider,
            ILogger<TokenCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(CleanupInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Task.Delay exception: {0}. Exiting.", ex.Message);
                    break;
                }

                await RemoveExpiredGrantsAsync();
            }
        }

        async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

                var tokenCleanupService = serviceScope.ServiceProvider.GetRequiredService<TokenCleanupService>();

                await tokenCleanupService.RemoveExpiredGrantsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }
    }
}
