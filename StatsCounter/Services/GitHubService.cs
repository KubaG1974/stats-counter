using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using StatsCounter.Models;


namespace StatsCounter.Services;

public interface IGitHubService
{
    Task<IEnumerable<RepositoryInfo>> GetRepositoryInfosByOwnerAsync(string owner);
}

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;
  
    public GitHubService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
  
    public async Task<IEnumerable<RepositoryInfo>> GetRepositoryInfosByOwnerAsync(string owner)
    {
        owner = owner.Trim();
        _ = owner.Length > 0 ? owner : throw new ArgumentException("Owner name cannot be empty or whitespace.");
        
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync($"users/{owner}/repos");
        }
        catch(HttpRequestException ex)
        {
            throw new Exception("There was a problem reaching the service.", ex);
        }

        using (response)
        {
            return await response.Content.ReadAsAsync<List<RepositoryInfo>>();
        }
    }
}