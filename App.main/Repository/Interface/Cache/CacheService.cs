using EasyCaching.Core;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.URI;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Repository.Interface.Cache
{
    public interface IResponseCacheService
    {
        Task CatcheResponseAsync(string cacheKey, object response);
        Task<string> GetCacheResponseAsync(string cacheKey);
        Task ResetCacheAsync(string cachekey);
        Task ResetCacheByPrefixAsync(string cachekey);
    }
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly IEasyCachingProviderFactory _providerFactory;
        private readonly ILoggerService _logger;
        private readonly IBaseURIs _uRIs;
        public ResponseCacheService(IEasyCachingProviderFactory providerFactory, ILoggerService logger, IBaseURIs uRIs)
        {
            _uRIs = uRIs;
            _logger = logger;
            _providerFactory = providerFactory;
            _cachingProvider = _providerFactory.GetCachingProvider("redis1");
        }
        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            try
            {
                var key = $"{_uRIs.LiveGateway}{cacheKey}";
                _logger.Information($"Cache");
                var cachedResponse = await _cachingProvider.GetAsync<string>(key);
                if (cachedResponse.HasValue) return cachedResponse.Value;

                if (cachedResponse.IsNull) return null;
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Cache not started:::: { ex.ToString()}");
                return null;
            }

        }
        public async Task ResetCacheByPrefixAsync(string cachekey)
        {
            try
            {
                var key = $"{_uRIs.LiveGateway}{cachekey}";
                await _cachingProvider.RemoveByPrefixAsync(key);
            }
            catch (Exception ex)
            {
                _logger.Error($"Cache not started:::: { ex.ToString()}");
            }
        }
        public async Task ResetCacheAsync(string cachekey)
        {
            try
            {
                var key = $"{_uRIs.LiveGateway}{cachekey}";
                await _cachingProvider.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.Error($"Cache not started:::: { ex.ToString()}");
            }

        }
        public async Task CatcheResponseAsync(string cacheKey, object response)
        {
            try
            {
                var key = $"{_uRIs.LiveGateway}{cacheKey}";
                if (response == null) return;
                var serializeResponse = JsonConvert.SerializeObject(response);
                await _cachingProvider.SetAsync(key, serializeResponse, TimeSpan.FromHours(12));
            }
            catch (Exception ex)
            {
                _logger.Error($"Cache not started:::: { ex.ToString()}");
            }

        }
    }
}
