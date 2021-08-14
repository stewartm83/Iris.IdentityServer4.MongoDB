
using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    public abstract class MongoDbContextBase
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        private readonly IOptions<MongoDbContextOptions> _options;

        public MongoDbContextBase(IOptions<MongoDbContextOptions> options)
        {
            _options = options
                ?? throw new ArgumentNullException(nameof(options));
            
            _client = GetMongoClient(_options.Value.MongoConnectionString);
            _database = _client.GetDatabase(options.Value.MongoDatabase);
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
