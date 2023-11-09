using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class FightDetailsChart
    {
        public string Title { get; set; }
        public IEnumerable<FightDetailsChartRow> Rows { get; set; }
    }
}