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
            _gitHubService = gitHubService;
        }

        public async Task<RepositoryStats> GetRepositoryStatsByOwnerAsync(string owner)
        {
            IEnumerable<RepositoryInfo> repositories = await _gitHubService.GetRepositoryInfosByOwnerAsync(owner);

            if (repositories == null || !repositories.Any())
            {
                return new RepositoryStats
                {
                    Owner = owner,
                    Languages = new List<string>(),
                    Size = 0,
                    PublicRepositories = 0,
                    AvgWatchers = 0.0,
                    AvgForks = 0.0
                };
            }

            int totalSize = 0;
            int totalWatchers = 0;
            int totalForks = 0;

            HashSet<string> languages = new HashSet<string>();

            foreach (RepositoryInfo repo in repositories)
            {
                totalSize += repo.Size;

                HashSet<string> uniqueLanguages = new HashSet<string>(repo.Languages);
                languages.UnionWith(uniqueLanguages);

                totalWatchers += repo.Watchers;
                totalForks += repo.Forks;
            }

            double avgWatchers = (double)totalWatchers / repositories.Count();
            double avgForks = (double)totalForks / repositories.Count();

            RepositoryStats stats = new RepositoryStats
            {
                Owner = owner,
                Languages = languages.ToList(), 
                Size = totalSize,
                PublicRepositories = repositories.Count(),
                AvgWatchers = avgWatchers,
                AvgForks = avgForks
            };

            return stats;
        }
    }
}