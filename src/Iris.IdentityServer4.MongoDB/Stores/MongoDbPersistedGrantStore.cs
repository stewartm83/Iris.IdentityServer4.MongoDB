
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityServer4.Stores;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    /// <summary>
    /// MongoDb persisted grant store
    /// </summary>
    public class MongoDbPersistedGrantStore : IPersistedGrantStore
    {
        private readonly MongoDbPersistedGrantContext _persistedGrantContext;
        private readonly ILogger<MongoDbPersistedGrantStore> _logger;

        public MongoDbPersistedGrantStore(
            MongoDbPersistedGrantContext persistedGrantContext,
            ILogger<MongoDbPersistedGrantStore> logger)
        {
            _persistedGrantContext = persistedGrantContext
                ?? throw new ArgumentNullException(nameof(persistedGrantContext));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task StoreAsync(PersistedGrant grant)
        {
            await _persistedGrantContext.PersistedGrants.InsertOneAsync(grant);

            return;
        }

        /// <inheritdoc/>
        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await _persistedGrantContext.PersistedGrants.AsQueryable().Where(p => p.Key == key).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            FilterDefinition<PersistedGrant> query = Builders<PersistedGrant>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.ClientId, filter.ClientId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.SessionId, filter.SessionId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.SubjectId, filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.Type, filter.Type);
            }

            return await _persistedGrantContext.PersistedGrants.Find(query).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            var filter = Builders<PersistedGrant>.Filter.Eq(s => s.Key, key);
            var response = await _persistedGrantContext.PersistedGrants.DeleteOneAsync(filter);

            if (!response.IsAcknowledged || response.DeletedCount <= 0)
            {
                _logger.LogError("Failed to delete");

                throw new Exception("Failed to delete");
            }

            return;
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            FilterDefinition<PersistedGrant> query = Builders<PersistedGrant>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.ClientId, filter.ClientId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.SessionId, filter.SessionId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.SubjectId, filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query &= Builders<PersistedGrant>.Filter.Eq(x => x.Type, filter.Type);
            }

            await _persistedGrantContext.PersistedGrants.DeleteManyAsync(query);

            return;
        }
    }
}
