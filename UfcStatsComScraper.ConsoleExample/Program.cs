using System;

namespace UfcStatsComScraper.ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var upcoming = scraper.ScrapeUpcoming();
            var completed = scraper.ScrapeCompleted();
            
            
            Console.WriteLine("Hello World!");
        }
    }
}
