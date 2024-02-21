using Newtonsoft.Json;
using System.Collections.Generic;

namespace StatsCounter.Models
{
    public class RepositoryInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("watchers_count")]
        public long Watchers { get; set; }
        
        [JsonProperty("forks_count")]
        public long Forks { get; set; }
        
        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("languages")]
        public Dictionary<string, int> Languages { get; set; }
    }
}