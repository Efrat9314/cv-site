using Servieces;
using Octokit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace Servieces
{
    
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;
        private readonly IMemoryCache _memoryCache;
        private const string userPortfolioKey = "userPortfolioKey";

        public GitHubService(IOptions<GitHubIntegrationOptions> options, IMemoryCache memoryCache)
        {
            _client = new GitHubClient(new ProductHeaderValue("My-GitHub-App"));
            _options = options.Value;
            _client.Credentials = new Credentials(_options.UserName, _options.Token); // הוספת אישורי הגישה
            _memoryCache = memoryCache;
        }

        public async Task<List<RepositoryInfo>> GetPortfolio()
        {
            if (!_memoryCache.TryGetValue(userPortfolioKey, out List<RepositoryInfo> repositoryInfos))
            {
                var repositories = await _client.Repository.GetAllForUser(_options.UserName);
                repositoryInfos = new List<RepositoryInfo>();

                foreach (var repo in repositories)
                {
                    var commits = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                    var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);
                    var lastCommit = commits.FirstOrDefault();
                    repositoryInfos.Add(new RepositoryInfo
                    {
                        Name = repo.Name,
                        Language = repo.Language,
                        LastCommit = lastCommit?.Commit.Committer.Date.DateTime ?? DateTime.MinValue,
                        Stars = repo.StargazersCount,
                        PullRequestCount = pullRequests.Count,
                        Url = repo.HtmlUrl
                    });
                }
                var cachOption = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

                //repositoryInfos = await _gitHubService.GetPortfolio();
                _memoryCache.Set(userPortfolioKey, repositoryInfos, cachOption);
            }
            return repositoryInfos;
        }

        //public async Task<List<RepositoryInfo>> GetPortfolio()
        //{
        //    var repositories = await _client.Repository.GetAllForUser(_options.UserName);
        //    var repositoryInfos = new List<RepositoryInfo>();

        //    foreach (var repo in repositories)
        //    {
        //        var commits = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
        //        var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);
        //        var lastCommit = commits.FirstOrDefault();
        //        repositoryInfos.Add(new RepositoryInfo
        //        {
        //            Name = repo.Name,
        //            Language = repo.Language,
        //            LastCommit = lastCommit?.Commit.Committer.Date.DateTime ?? DateTime.MinValue,
        //            Stars = repo.StargazersCount,
        //            PullRequestCount = pullRequests.Count,
        //            Url = repo.HtmlUrl
        //        });
        //    }

        //    return repositoryInfos;
        //}


        public async Task<List<Repository>> SearchRepositories(string name = null, string language = null, string username = null)
        {
            // אם כל הפרמטרים ריקים, החזר את כל המאגרים
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(language) && string.IsNullOrWhiteSpace(username))
            {
                var allRepositories = await _client.Repository.GetAllForCurrent();
                return allRepositories.ToList();
            }

            // צור את הבקשה לחיפוש עם הפרמטרים שנמסרו
            var searchRequest = new SearchRepositoriesRequest(name ?? string.Empty)
            {
                Language = string.IsNullOrWhiteSpace(language) ? null : (Language)Enum.Parse(typeof(Language), language, true),
                User = string.IsNullOrWhiteSpace(username) ? null : username
            };
            var searchResults = await _client.Search.SearchRepo(searchRequest);
            return searchResults.Items.ToList();
        }

    }
}
