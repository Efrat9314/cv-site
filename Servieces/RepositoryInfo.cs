using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servieces
{
    public class RepositoryInfo
    {
        public string Name { get; set; }
        public string Language { get; set; }
        public DateTime LastCommit { get; set; }
        public int Stars { get; set; }
        public int PullRequestCount { get; set; }
        public string Url { get; set; }
    }
}
