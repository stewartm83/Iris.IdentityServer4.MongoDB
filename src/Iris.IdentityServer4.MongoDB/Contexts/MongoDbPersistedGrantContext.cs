using IdentityServer4.Models;
using Iris.IdentityServer4.MongoDB.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Iris.IdentityServer4.MongoDB.Contexts
{
    public class MongoDbPersistedGrantContext : MongoDbContextBase
    {
        private readonly IMongoCollection<PersistedGrant> _collectionPersistedGrants;
        private readonly IMongoCollection<Entities.DeviceFlowCodes> _collectionDeviceFlowCodes;

        public MongoDbPersistedGrantContext(IOptions<MongoDbStoreOptions> options)
            : base(options)
        {
            _collectionPersistedGrants = _database.GetCollection<PersistedGrant>("persistedGrants");
            _collectionPersistedGrants.Indexes.CreateOneAsync(new CreateIndexModel<PersistedGrant>(Builders<PersistedGrant>.IndexKeys.Ascending(p => p.Key), new CreateIndexOptions() { Background = true }));
            _collectionPersistedGrants.Indexes.CreateOneAsync(new CreateIndexModel<PersistedGrant>(Builders<PersistedGrant>.IndexKeys.Ascending(p => p.SubjectId), new CreateIndexOptions() { Background = true }));
            _collectionPersistedGrants.Indexes.CreateOneAsync(new CreateIndexModel<PersistedGrant>(
              Builders<PersistedGrant>.IndexKeys.Combine(
                  Builders<PersistedGrant>.IndexKeys.Ascending(p => p.ClientId),
                  Builders<PersistedGrant>.IndexKeys.Ascending(p => p.SubjectId)),
              new CreateIndexOptions() { Background = true }));
            _collectionPersistedGrants.Indexes.CreateOneAsync(new CreateIndexModel<PersistedGrant>(
              Builders<PersistedGrant>.IndexKeys.Combine(
                  Builders<PersistedGrant>.IndexKeys.Ascending(p => p.ClientId),
                  Builders<PersistedGrant>.IndexKeys.Ascending(p => p.SubjectId),
                  Builders<PersistedGrant>.IndexKeys.Ascending(p => p.Type)),
              new CreateIndexOptions() { Background = true }));

            _collectionDeviceFlowCodes = _database.GetCollection<Entities.DeviceFlowCodes>("deviceFlowCodes");
            _collectionDeviceFlowCodes.Indexes.CreateOneAsync(new CreateIndexModel<Entities.DeviceFlowCodes>(Builders<Entities.DeviceFlowCodes>.IndexKeys.Ascending(p => p.DeviceCode), new CreateIndexOptions() { Background = true }));
            _collectionDeviceFlowCodes.Indexes.CreateOneAsync(new CreateIndexModel<Entities.DeviceFlowCodes>(Builders<Entities.DeviceFlowCodes>.IndexKeys.Ascending(p => p.Expiration), new CreateIndexOptions() { Background = true }));
        }

        public IMongoCollection<PersistedGrant> PersistedGrants => _collectionPersistedGrants;
        public IMongoCollection<Entities.DeviceFlowCodes> DeviceFlowCodes => _collectionDeviceFlowCodes;
    }
}
