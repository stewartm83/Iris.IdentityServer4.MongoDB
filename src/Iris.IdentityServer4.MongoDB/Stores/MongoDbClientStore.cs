
using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IdentityServer4.Stores;
using IdentityServer4.Models;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    /// <summary>
    /// MongoDb client store
    /// </summary>
    public class MongoDbClientStore : IClientStore
    {
        private readonly MongoDbConfigurationContext _configurationContext;

        /// <summary>
        /// <see cref="MongoDbClientStore"/>
        /// </summary>
        /// <param name="configurationContext"></param>
        public MongoDbClientStore(MongoDbConfigurationContext configurationContext)
        {
            _configurationContext = configurationContext
                ?? throw new ArgumentNullException(nameof(configurationContext));
        }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var query = _configurationContext.Clients
                .AsQueryable()
                .Where(c => c.ClientId == clientId);

            return await query.FirstOrDefaultAsync();
        }
    }
}
