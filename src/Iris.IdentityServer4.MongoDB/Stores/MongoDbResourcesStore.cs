using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    /// <summary>
    /// MongoDb Resource Store
    /// </summary>
    public class MongoDbResourceStore : IResourceStore
    {
        private readonly MongoDbConfigurationContext _configurationContext;
        private readonly ILogger<MongoDbResourceStore> _logger;

        public MongoDbResourceStore(
            MongoDbConfigurationContext configurationContext,
            ILogger<MongoDbResourceStore> logger)
        {
            _configurationContext = configurationContext
                ?? throw new ArgumentNullException(nameof(configurationContext));
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Resources> GetAllResourcesAsync()
        {
            var identity = await _configurationContext.IdentityResources.AsQueryable().ToListAsync();
            var apis = await _configurationContext.ApiResources.AsQueryable().ToListAsync();
            var scopes = await _configurationContext.ApiScopes.AsQueryable().ToListAsync();

            var result = new Resources(identity, apis, scopes);

            _logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources",
                result.IdentityResources.Select(x => x.Name)
                .Union(result.ApiScopes.Select(x => x.Name)),
                result.ApiResources.Select(x => x.Name));

            return result;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null)
            {
                throw new ArgumentNullException(nameof(apiResourceNames));
            }

            return await _configurationContext.ApiResources.AsQueryable()
                .Where(a => apiResourceNames.Contains(a.Name))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            return await _configurationContext.IdentityResources.AsQueryable()
                .Where(a => scopeNames.Contains(a.Name))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            return await _configurationContext.ApiResources.AsQueryable()
                .Where(a => a.Scopes.Any(x => scopeNames.Contains(x)))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            return await _configurationContext.ApiScopes.AsQueryable()
                .Where(a => scopeNames.Contains(a.Name))
                .ToListAsync();
        }
    }
}
