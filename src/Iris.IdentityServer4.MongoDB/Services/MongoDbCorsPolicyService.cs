using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IdentityServer4.Services;
using Iris.IdentityServer4.MongoDB.Stores;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB.Services
{
    /// <summary>
    /// CORS policy service that configures the allowed origins from a list of clients' redirect URLs.
    /// </summary>
    public class MongoDbCorsPolicyService : ICorsPolicyService
    {
        private readonly MongoDbConfigurationContext _configurationContext;
        private readonly ILogger<MongoDbCorsPolicyService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbCorsPolicyService"/> class.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="clients">The clients.</param>
        public MongoDbCorsPolicyService(
            MongoDbConfigurationContext configurationContext,
            ILogger<MongoDbCorsPolicyService> logger)
        {
            _configurationContext = configurationContext
                ?? throw new ArgumentNullException(nameof(configurationContext));
            _logger  = logger;
       
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public virtual async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var query = _configurationContext.ClientCorsOrigins.AsQueryable().Where(
               c=>c.Origin == origin);

            var isAllowed = await query.AnyAsync();

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }
}
