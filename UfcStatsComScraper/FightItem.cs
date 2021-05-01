using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class FightItem
    {
        public IEnumerable<Link> Fighters { get; set; }
        public string MatchupUrl { get; set; }
    }
}