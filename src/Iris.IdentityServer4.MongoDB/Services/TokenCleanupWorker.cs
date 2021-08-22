using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Iris.IdentityServer4.MongoDB.Stores;
using Iris.IdentityServer4.MongoDB.Options;

namespace Iris.IdentityServer4.MongoDB.Services
{
    /// <summary>
    /// TokenCleanupWorker
    /// </summary>
    public class TokenCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<MongoDbStoreOptions> _options;
        private readonly ILogger<TokenCleanupWorker> _logger;

        /// <summary>
        /// TokenCleanupWorker
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public TokenCleanupWorker(
            IServiceProvider serviceProvider,
            IOptions<MongoDbStoreOptions> options,
            ILogger<TokenCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_options.Value.TokenCleanupInterval, stoppingToken);
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
