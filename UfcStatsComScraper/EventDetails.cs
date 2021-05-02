using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class EventDetails
    {
        public IEnumerable<FightItem> Fights { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
    }
}