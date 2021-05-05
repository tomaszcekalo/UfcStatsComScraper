using System.Collections.Generic;

namespace UfcStatsComScraper
{
    public class FightDetails
    {
        public Link EventLink { get; set; }
        private IEnumerable<FightDetailsFighter> Fighters { get; set; }
        public string Title { get; set; }
        public string Method { get; set; }
        public string Round { get; set; }
        public string Time { get; set; }
        public string TimeFormat { get; set; }
        public string Referee { get; set; }
        public string Details { get; set; }
        //Totals
        //per round
        //Significant Strikes
        //per round
        //Significant Strikes 2
        //Landed By Taeget
        //Landed By Position
        //Per Round
    }
}