using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UfcStatsComScraper.Tests
{
    [TestClass]
    public class LiveDataTests
    {
        [TestMethod]
        public void TestScrapeUpcoming()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var upcoming = scraper.ScrapeUpcoming();
        }
        [TestMethod]
        public void TestScrapeCompleted()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var completed = scraper.ScrapeCompleted();
        }
        [TestMethod]
        public void TestScrapeEventDetails()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var eventDetailsUpcoming = scraper
                .ScrapeEventDetails("http://ufcstats.com/event-details/d57e6a8971b6d2bd");
        }
        [TestMethod]
        public void TestScrapeFightDetails()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var fightDetails = scraper.ScrapeFightDetails(
                "http://ufcstats.com/fight-details/4a704dae3091adaf"
                //"http://ufcstats.com/fight-details/2c0cddd12deb7450"
                );
        }
        [TestMethod]
        public void TestScrapeFighterDetails()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var fighterDetails = scraper.ScrapeFighterDetails("http://ufcstats.com/fighter-details/2cd428e9606856fd");
        }
    }
}