
using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Iris.IdentityServer4.MongoDB.Entities;
using Microsoft.Extensions.Logging;
using Iris.IdentityServer4.MongoDB.Contexts;

namespace Iris.IdentityServer4.MongoDB.Stores
{
    /// <summary>
    /// MongoDb device flow store
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IDeviceFlowStore" />
    public partial class MongoDbDeviceFlowStore : IDeviceFlowStore
    {
        private readonly MongoDbPersistedGrantContext _persistedGrantContext;
        private readonly ILogger<MongoDbDeviceFlowStore> _logger;

        public MongoDbDeviceFlowStore(
            MongoDbPersistedGrantContext persistedGrantContext,
            ILogger<MongoDbDeviceFlowStore> logger)
        {
            _persistedGrantContext = persistedGrantContext
                ?? throw new ArgumentNullException(nameof(persistedGrantContext));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Stores the device authorization request.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            await _persistedGrantContext.DeviceFlowCodes.InsertOneAsync(new DeviceFlowCodes
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = data.ClientId,
                SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = data.CreationTime,
                Expiration = data.CreationTime.AddSeconds(data.Lifetime),
                Data = JsonConvert.SerializeObject(data)
            });

            return;
        }

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            var deviceFlowCode = await _persistedGrantContext.DeviceFlowCodes
                            .AsQueryable()
                            .Where(d => d.UserCode == userCode)
                            .FirstOrDefaultAsync();

            return JsonConvert.DeserializeObject<DeviceCode>(deviceFlowCode?.Data);
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCode = await _persistedGrantContext.DeviceFlowCodes
                .AsQueryable()
                .Where(d => d.DeviceCode == deviceCode)
                .FirstOrDefaultAsync();

            return JsonConvert.DeserializeObject<DeviceCode>(deviceFlowCode?.Data);
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var response = await _persistedGrantContext.DeviceFlowCodes
                .UpdateOneAsync(
                Builders<DeviceFlowCodes>.Filter.Eq(s => s.UserCode, userCode),
                Builders<DeviceFlowCodes>.Update.Set("Data", data));

            if (!response.IsAcknowledged || response.ModifiedCount <= 0)
            {
                _logger.LogError("Failed to update");

                throw new Exception("Failed to update");
            }

            return;
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var response = await _persistedGrantContext.DeviceFlowCodes
                .DeleteOneAsync(
                Builders<DeviceFlowCodes>.Filter.Eq(s => s.DeviceCode, deviceCode)
                );

            if (!response.IsAcknowledged || response.DeletedCount <= 0)
            {
                _logger.LogError("Failed to delete");

                throw new Exception("Failed to delete");
            }

            return;
        }
    }
}
