using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IdentityServer4.Stores;
using Iris.IdentityServer4.MongoDB.Stores;
using Iris.IdentityServer4.MongoDB.Services;
using System;
using Iris.IdentityServer4.MongoDB.Options;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB
{
    public static class IdentityServerMongoDbBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoDbStores(
            this IIdentityServerBuilder builder,
            Action<MongoDbStoreOptions> storeActionOptions)
        {
            builder.Services.Configure<MongoDbStoreOptions>(storeActionOptions);
            
            builder.Services.TryAddSingleton<MongoDbConfigurationContext>();
            builder.Services.TryAddSingleton<MongoDbPersistedGrantContext>();

            // Add configuration stores
            builder.AddClientStore<MongoDbClientStore>()
                .AddCorsPolicyService<MongoDbCorsPolicyService>()
                .AddResourceStore<MongoDbResourceStore>()
                .AddInMemoryCaching()
                .AddClientStoreCache<MongoDbClientStore>()
                .AddCorsPolicyCache<MongoDbCorsPolicyService>()
                .AddResourceStoreCache<MongoDbResourceStore>();

            // Add operational stores
            builder.Services.AddTransient<IPersistedGrantStore, MongoDbPersistedGrantStore>();
            builder.Services.AddTransient<IDeviceFlowStore, MongoDbDeviceFlowStore>();
            builder.Services.AddSingleton<TokenCleanupService>();
            builder.Services.AddHostedService<TokenCleanupWorker>();

            return builder;
        }
    }
}
