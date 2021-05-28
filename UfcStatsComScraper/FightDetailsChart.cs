using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class FightDetailsChart
    {
        public string Title { get; internal set; }
        public IEnumerable<FightDetailsChartRow> Rows { get; internal set; }
    }
}