using IdentityServer4.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    public class MongoDbPersistedGrantContext : MongoDbContextBase
    {
        private readonly IMongoCollection<PersistedGrant> _collectionPersistedGrants;
        private readonly IMongoCollection<Entities.DeviceFlowCodes> _collectionDeviceFlowCodes;

        public MongoDbPersistedGrantContext(IOptions<MongoDbContextOptions> options)
            : base(options)
        {
            _collectionPersistedGrants = _database.GetCollection<PersistedGrant>("persistedGrants");
            _collectionDeviceFlowCodes = _database.GetCollection<Entities.DeviceFlowCodes>("deviceFlowCodes");

        }

        public IMongoCollection<PersistedGrant> PersistedGrants => _collectionPersistedGrants;
        public IMongoCollection<Entities.DeviceFlowCodes> DeviceFlowCodes => _collectionDeviceFlowCodes;
    }
}
