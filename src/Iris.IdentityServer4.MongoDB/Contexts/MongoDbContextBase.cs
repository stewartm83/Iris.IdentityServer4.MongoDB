
using System;
using Iris.IdentityServer4.MongoDB.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Iris.IdentityServer4.MongoDB.Contexts
{
    public abstract class MongoDbContextBase
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        private readonly IOptions<MongoDbStoreOptions> _options;

        public MongoDbContextBase(IOptions<MongoDbStoreOptions> options)
        {
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
            _client = GetMongoClient(_options.Value.ConnectionString);
            _database = _client.GetDatabase(options.Value.Database);
        }

        private MongoClient GetMongoClient(string connectionString)
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.MaxConnectionPoolSize = 1000;
            settings.MinConnectionPoolSize = 10;

            return new MongoClient(settings);
        }
    }
}
