
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using IdentityServer4.Models;
using Iris.IdentityServer4.MongoDB.Options;

namespace Iris.IdentityServer4.MongoDB.Contexts
{
    public class MongoDbConfigurationContext : MongoDbContextBase
    {
        private readonly IMongoCollection<Client> _collectionClients;
        private readonly IMongoCollection<Entities.ClientCorsOrigin> _collectionClientCorsOrigins;
        private readonly IMongoCollection<IdentityResource> _collectionIdentityResources;
        private readonly IMongoCollection<ApiResource> _collectionApiResources;
        private readonly IMongoCollection<ApiScope> _collectionApiScopes;

        public MongoDbConfigurationContext(IOptions<MongoDbStoreOptions> options)
            : base(options)
        {
            _collectionClients
                        = _database.GetCollection<Client>("clients");

            _collectionClients.Indexes.CreateOneAsync(new CreateIndexModel<Client>(Builders<Client>.IndexKeys.Ascending(x => x.ClientId),
            new CreateIndexOptions() { Background = true }));

            _collectionClientCorsOrigins
         = _database.GetCollection<Entities.ClientCorsOrigin>("clientCorsOrigins");
            _collectionClientCorsOrigins.Indexes.CreateOneAsync(new CreateIndexModel<Entities.ClientCorsOrigin>(Builders<Entities.ClientCorsOrigin>.IndexKeys.Ascending(x => x.ClientId), new CreateIndexOptions() { Background = true }));

            _collectionIdentityResources
            = _database.GetCollection<IdentityResource>("identityResources");
            _collectionIdentityResources.Indexes.CreateOneAsync(new CreateIndexModel<IdentityResource>(Builders<IdentityResource>.IndexKeys.Ascending(x => x.Name), new CreateIndexOptions() { Background = true }));

            _collectionApiResources
           = _database.GetCollection<ApiResource>("apiResources");
            _collectionApiResources.Indexes.CreateOneAsync(new CreateIndexModel<ApiResource>(Builders<ApiResource>.IndexKeys.Ascending(x => x.Name), new CreateIndexOptions() { Background = true }));
            _collectionApiResources.Indexes.CreateOneAsync(new CreateIndexModel<ApiResource>(Builders<ApiResource>.IndexKeys.Ascending(x => x.Scopes), new CreateIndexOptions() { Background = true }));

            _collectionApiScopes
           = _database.GetCollection<ApiScope>("apiScopes");
            _collectionApiScopes.Indexes.CreateOneAsync(new CreateIndexModel<ApiScope>(Builders<ApiScope>.IndexKeys.Ascending(x => x.Name), new CreateIndexOptions { Background = true }));
        }

        public IMongoCollection<Client> Clients => _collectionClients;
        public IMongoCollection<ApiScope> ApiScopes => _collectionApiScopes;
        public IMongoCollection<ApiResource> ApiResources => _collectionApiResources;
        public IMongoCollection<IdentityResource> IdentityResources => _collectionIdentityResources;
        public IMongoCollection<Entities.ClientCorsOrigin> ClientCorsOrigins => _collectionClientCorsOrigins;
    }
}
