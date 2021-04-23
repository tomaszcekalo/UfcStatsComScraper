namespace UfcStatsComScraper
{
    public static class Consts
    {
        public const string UfcStatsBase = "http://ufcstats.com";
        private const string UfcStatsEvents = UfcStatsBase + "/statistics/events";
        public const string UfcStatsEventsCompleted = UfcStatsEvents + "/completed";
        public const string UfcStatsEventsUpcoming = UfcStatsEvents + "/upcoming";
        public const string UfcStatsEventSearch = UfcStatsEvents + "/search";
        public const string UfcStatsFighters = "http://ufcstats.com/statistics/fighters";
    }
}