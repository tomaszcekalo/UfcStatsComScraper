using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class FightItem
    {
        public IEnumerable<Link> Fighters { get; set; }
        public string MatchupUrl { get; set; }
        public IEnumerable<string> KD { get; set; }
        public IEnumerable<string> STR { get; set; }
        public IEnumerable<string> TD { get; set; }
        public IEnumerable<string> SUB { get; set; }
        public string WeightClass { get; set; }
        public string Method { get; set; }
        public string Round { get; set; }
        public string Time { get; set; }
    }
}