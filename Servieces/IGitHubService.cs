using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servieces
{
    public interface IGitHubService
    {
         Task<List<RepositoryInfo>> GetPortfolio();

        Task<List<Repository>> SearchRepositories(string name, string language, string username);

    }
}
