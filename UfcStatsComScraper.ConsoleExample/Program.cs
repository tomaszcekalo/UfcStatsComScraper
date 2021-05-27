using System;

namespace UfcStatsComScraper.ConsoleExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var upcoming = scraper.ScrapeUpcoming();
            var completed = scraper.ScrapeCompleted();
            var eventDetailsUpcoming = scraper.ScrapeEventDetails("http://ufcstats.com/event-details/d57e6a8971b6d2bd");
            
            var fightDetails = scraper.ScrapeFightDetails(
                "http://ufcstats.com/fight-details/4a704dae3091adaf"
                //"http://ufcstats.com/fight-details/2c0cddd12deb7450"
                );
            var fighterDetails = scraper.ScrapeFighterDetails("http://ufcstats.com/fighter-details/2cd428e9606856fd");
            Console.WriteLine("Hello World!");
        }
    }
}