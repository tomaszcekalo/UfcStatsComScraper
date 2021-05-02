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

            Console.WriteLine("Hello World!");
        }
    }
}