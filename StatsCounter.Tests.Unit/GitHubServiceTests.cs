using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using StatsCounter.Models;
using StatsCounter.Services;
using Xunit;

namespace StatsCounter.Tests.Unit;

public class GitHubServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandler;
    private readonly IGitHubService _gitHubService;

    public GitHubServiceTests()
    {
        _httpMessageHandler = new Mock<HttpMessageHandler>();
        _gitHubService = new GitHubService(
            new HttpClient(_httpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            });
    }
        
    [Fact]
    public async Task ShouldDeserializeResponse()
    {
        // given
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[{'id':1,'name':'name','stargazers_count':2,'watchers_count':3,'forks_count':4,'size':5}]")
            });
            
        // when
        var result = await _gitHubService.GetRepositoryInfosByOwnerAsync("owner");
            
        // then
        result.Should().BeEquivalentTo(
            new List<RepositoryInfo>
            {
                new RepositoryInfo
                {
                    Id = 1,
                    Name = "name",
                    Watchers = 3,
                    Forks = 4,
                    Size = 5
                }
            }.AsEnumerable());
    }
}