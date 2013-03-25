using System.Collections.Generic;

namespace Codestellation.DarkFlow.Config
{
    public class PersistenceSettings
    {
        public string Type { get; set; }
        
        public List<MatcherSettings> Matchers { get; set; }
    }
}