
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using IdentityServer4.Models;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    public class MongoDbConfigurationContext : MongoDbContextBase
    {
        private readonly IMongoCollection<Client> _collectionClients;
        private readonly IMongoCollection<Entities.ClientCorsOrigin> _collectionClientCorsOrigins;
        private readonly IMongoCollection<IdentityResource> _collectionIdentityResources;
        private readonly IMongoCollection<ApiResource> _collectionApiResources;
        private readonly IMongoCollection<ApiScope> _collectionApiScopes;

        public MongoDbConfigurationContext(IOptions<MongoDbContextOptions> options)
            : base(options)
        {
            _collectionClients
                        = _database.GetCollection<Client>("clients");
            _collectionClientCorsOrigins
         = _database.GetCollection<Entities.ClientCorsOrigin>("clientCorsOrigins");
            _collectionIdentityResources
            = _database.GetCollection<IdentityResource>("identityResources");
            _collectionApiResources
           = _database.GetCollection<ApiResource>("apiResources");
            _collectionApiScopes
           = _database.GetCollection<ApiScope>("apiScopes");

        }

        public IMongoCollection<Client> Clients => _collectionClients;
        public IMongoCollection<ApiScope> ApiScopes => _collectionApiScopes;
        public IMongoCollection<ApiResource> ApiResources => _collectionApiResources;
        public IMongoCollection<IdentityResource> IdentityResources => _collectionIdentityResources;
        public IMongoCollection<Entities.ClientCorsOrigin> ClientCorsOrigins => _collectionClientCorsOrigins;
    }
}
