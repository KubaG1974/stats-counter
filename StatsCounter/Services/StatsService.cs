using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatsCounter.Models;

namespace StatsCounter.Services
{
    public interface IStatsService
    {
        Task<RepositoryStats> GetRepositoryStatsByOwnerAsync(string owner);
    }

    public class StatsService : IStatsService
    {     
        private readonly IGitHubService _gitHubService;
        
        public StatsService(IGitHubService gitHubService)
        { 
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));  
        }

        public async Task<RepositoryStats> GetRepositoryStatsByOwnerAsync(string owner)
        {
            owner = owner?.Trim() ?? throw new ArgumentException("Owner cannot be null or empty.", nameof(owner));

            IEnumerable<RepositoryInfo> repositories = await _gitHubService.GetRepositoryInfosByOwnerAsync(owner) ?? Enumerable.Empty<RepositoryInfo>();

            if (repositories == null)
            {
                throw new Exception("Received null repository list from GitHub service.");
            }
            
            long totalSize = repositories.Sum(repo => repo.Size);
            long totalWatchers = repositories.Sum(repo => repo.Watchers);
            long totalForks = repositories.Sum(repo => repo.Forks);

            int repoCount = repositories.Count();

            IDictionary<string, int> languages = new Dictionary<string, int>();
            //Dictionary<string, int> languagesDict = languages.ToDictionary(k => k.Key, k => k.Value);
            foreach (RepositoryInfo repo in repositories)
            {
                foreach (KeyValuePair<string, int> lang in repo.Languages)
                {
                    string language = lang.Key; 
                    int count = lang.Value; 
                    if (!languages.TryAdd(language, 1))
                    {
                        languages[language]++;
                    }
                }
            }
            Dictionary<string, int> languagesDict = new Dictionary<string, int>(languages);
            double avgWatchers = repoCount > 0 ? (double)totalWatchers / repoCount : 0.0;
            double avgForks = repoCount > 0 ? (double)totalForks / repoCount : 0.0;

            return new RepositoryStats
            {
                Owner = owner,
                Languages = languagesDict,
                Size = totalSize,
                Repositories = repoCount,
                AvgWatchers = avgWatchers,
                AvgForks = avgForks
            };
        }
    }
}
