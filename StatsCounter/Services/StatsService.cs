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
            if(string.IsNullOrEmpty(owner))
            {
                throw new ArgumentException("Owner cannot be null or empty.", nameof(owner));
            }

            IEnumerable<RepositoryInfo> repositories = await _gitHubService.GetRepositoryInfosByOwnerAsync(owner);

            if(repositories == null)
            {
                throw new NullReferenceException("No repositories found for this owner.");
            }

            int totalSize = 0;
            int totalWatchers = 0;
            int totalForks = 0;

            List<string> languages = new List<string>();
            int repoCount = repositories.Count();

            foreach (RepositoryInfo repo in repositories)
            {
                totalSize += repo.Size;

                foreach (string lang in repo.Languages)
                {
                    _ = languages.Contains(lang) ? null : languages.Add(lang);
                }

                totalWatchers += repo.Watchers;
                totalForks += repo.Forks;
            }

            double avgWatchers = repoCount > 0 ? (double)totalWatchers / repoCount : 0.0;
            double avgForks = repoCount > 0 ? (double)totalForks / repoCount : 0.0;

            RepositoryStats stats = new RepositoryStats
            {
                Owner = owner,
                Languages = languages,
                Size = totalSize,
                PublicRepositories = repoCount,
                AvgWatchers = avgWatchers,
                AvgForks = avgForks
            };

            return stats;
            
        }
    }
}