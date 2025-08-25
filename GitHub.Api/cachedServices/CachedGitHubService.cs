using Microsoft.Extensions.Caching.Memory;
using Octokit;
using Servieces;

namespace GitHub.Api.cachedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private const string userPortfolioKey = "userPortfolioKey";
        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }
        public async Task<List<RepositoryInfo>> GetPortfolio()
        {
            if(_memoryCache.TryGetValue(userPortfolioKey, out List<RepositoryInfo> portfolio))
                return portfolio;

            var cachOption = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            portfolio = await _gitHubService.GetPortfolio();
            _memoryCache.Set(userPortfolioKey, portfolio,cachOption);

            return portfolio;
        }

        public Task<List<Repository>> SearchRepositories(string name, string language, string username)
        {
            return _gitHubService.SearchRepositories(name, language, username);
        }
    }
}
