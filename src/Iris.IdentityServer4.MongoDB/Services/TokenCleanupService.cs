using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using IdentityServer4.Models;
using Iris.IdentityServer4.MongoDB.Stores;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB.Services
{
    /// <summary>
    /// Helper to cleanup stale persisted grants and device codes.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly MongoDbPersistedGrantContext _persistedGrantDbContext;
        private readonly ILogger<TokenCleanupService> _logger;

        /// <summary>
        /// Constructor for TokenCleanupService.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="persistedGrantDbContext"></param>
        /// <param name="operationalStoreNotification"></param>
        /// <param name="logger"></param>
        public TokenCleanupService(
            MongoDbPersistedGrantContext persistedGrantDbContext,
            ILogger<TokenCleanupService> logger)
        {
            _persistedGrantDbContext = persistedGrantDbContext
                ?? throw new ArgumentNullException(nameof(persistedGrantDbContext));
            _logger = logger;
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            var result = await _persistedGrantDbContext.PersistedGrants
                .DeleteManyAsync(Builders<PersistedGrant>
                .Filter.Lt(x => x.Expiration, DateTime.UtcNow));

            _logger.LogInformation("Removing {grantCount} grants", result.DeletedCount);
        }


        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveDeviceCodesAsync()
        {
            var result = await _persistedGrantDbContext.DeviceFlowCodes
                .DeleteManyAsync(Builders<Entities.DeviceFlowCodes>
                .Filter.Lt(x => x.Expiration, DateTime.UtcNow));

            _logger.LogInformation("Removing {deviceCodeCount} device flow codes", result.DeletedCount);
        }
    }
}
