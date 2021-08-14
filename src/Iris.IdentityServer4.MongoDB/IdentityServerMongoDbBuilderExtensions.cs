using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Iris.IdentityServer4.MongoDB.Stores;
using Iris.IdentityServer4.MongoDB.Services;

namespace Iris.IdentityServer4.MongoDB
{
    public static class IdentityServerMongoDbBuilderExtensions
    {
        private static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, MongoDbClientStore>();
            builder.Services.AddTransient<IResourceStore, MongoDbResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, MongoDbCorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            builder.Services.Configure<MongoDbContextOptions>(configuration.GetSection("MongoDbContextOptions"));
            builder.Services.TryAddSingleton<MongoDbConfigurationContext>();

            builder.AddConfigurationStore();

            return builder;
        }

        // Operational Store
        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            builder.Services.Configure<MongoDbContextOptions>(configuration.GetSection("MongoDbContextOptions"));
            builder.Services.TryAddSingleton<MongoDbPersistedGrantContext>();

            builder.AddOperationalStore();
            return builder;
        }

        private static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IPersistedGrantStore, MongoDbPersistedGrantStore>();
            builder.Services.AddSingleton<TokenCleanupService>();

            return builder;
        }
    }
}
