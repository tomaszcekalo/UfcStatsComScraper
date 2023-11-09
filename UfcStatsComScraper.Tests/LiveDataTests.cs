using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UfcStatsScraper.ScrapySharp;

namespace UfcStatsComScraper.Tests
{
    [TestClass]
    public class LiveDataTests
    {
        //anglesharp
        [TestMethod]
        public async Task TestScrapeUpcomingAngleSharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var upcoming = await scraper.ScrapeUpcoming();
        }

        [TestMethod]
        public async Task TestScrapeCompletedAngleSharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var completed = await scraper.ScrapeCompleted();
        }

        [TestMethod]
        public async Task TestScrapeEventDetailsv()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var eventDetailsUpcoming = await scraper
                .ScrapeEventDetails("http://ufcstats.com/event-details/d57e6a8971b6d2bd");
        }

        [TestMethod]
        public async Task TestScrapeFightDetailsAngleSharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var fightDetails = await scraper.ScrapeFightDetails(
                "http://ufcstats.com/fight-details/4a704dae3091adaf"
                //"http://ufcstats.com/fight-details/2c0cddd12deb7450"
                );
        }

        [TestMethod]
        public async Task TestScrapeFighterDetailsAngleSharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraper();
            var fighterDetails = await scraper.ScrapeFighterDetails("http://ufcstats.com/fighter-details/2cd428e9606856fd");
        }

        //scrapysharp
        [TestMethod]
        public async Task TestScrapeUpcomingScrapySharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraperScrapySharp();
            var upcoming = await scraper.ScrapeUpcoming();
        }

        [TestMethod]
        public async Task TestScrapeCompletedScrapySharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraperScrapySharp();
            var completed = await scraper.ScrapeCompleted();
        }

        [TestMethod]
        public async Task TestScrapeEventDetailsScrapySharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraperScrapySharp();
            var eventDetailsUpcoming = await scraper
                .ScrapeEventDetails("http://ufcstats.com/event-details/d57e6a8971b6d2bd");
        }

        [TestMethod]
        public async Task TestScrapeFightDetailsScrapySharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraperScrapySharp();
            var fightDetails = await scraper.ScrapeFightDetails(
                "http://ufcstats.com/fight-details/4a704dae3091adaf"
                //"http://ufcstats.com/fight-details/2c0cddd12deb7450"
                );
        }

        [TestMethod]
        public async Task TestScrapeFighterDetailsScrapySharp()
        {
            IUfcStatsScraper scraper = new UfcStatsScraperScrapySharp();
            var fighterDetails = await scraper.ScrapeFighterDetails("http://ufcstats.com/fighter-details/2cd428e9606856fd");
        }

        //compare both
        [TestMethod]
        public async Task TestScrapeUpcoming()
        {
            IUfcStatsScraper scraperAngleSharp = new UfcStatsScraper();
            var upcomingAngleSharp = await scraperAngleSharp.ScrapeUpcoming();
            IUfcStatsScraper scraperScrapySharp = new UfcStatsScraperScrapySharp();
            var upcomingScrapySharp = await scraperScrapySharp.ScrapeUpcoming();
            upcomingAngleSharp.Should().BeEquivalentTo(upcomingScrapySharp);
        }

        [TestMethod]
        public async Task TestScrapeCompleted()
        {
            IUfcStatsScraper scraperAngleSharp = new UfcStatsScraper();
            var completedgAngleSharp = await scraperAngleSharp.ScrapeCompleted();
            IUfcStatsScraper scraperScrapySharp = new UfcStatsScraperScrapySharp();
            var completedScrapySharp = await scraperScrapySharp.ScrapeCompleted();
            completedgAngleSharp.Should().BeEquivalentTo(completedScrapySharp);
        }

        [TestMethod]
        public async Task TestScrapeEventDetails()
        {
            IUfcStatsScraper scraperAngleSharp = new UfcStatsScraper();
            var eventDetailsUpcominggAngleSharp = await scraperAngleSharp
                .ScrapeEventDetails("http://ufcstats.com/event-details/d57e6a8971b6d2bd");
            IUfcStatsScraper scraperScrapySharp = new UfcStatsScraperScrapySharp();
            var eventDetailsUpcomingScrapySharp = await scraperScrapySharp
                .ScrapeEventDetails("http://ufcstats.com/event-details/d57e6a8971b6d2bd");
            eventDetailsUpcominggAngleSharp.Should().BeEquivalentTo(eventDetailsUpcomingScrapySharp);
        }

        [TestMethod]
        public async Task TestScrapeFightDetails()
        {
            IUfcStatsScraper scraperAngleSharp = new UfcStatsScraper();
            var fightDetailsgAngleSharp = await scraperAngleSharp.ScrapeFightDetails(
                "http://ufcstats.com/fight-details/4a704dae3091adaf"
                //"http://ufcstats.com/fight-details/2c0cddd12deb7450"
                );
            IUfcStatsScraper scraperScrapySharp = new UfcStatsScraperScrapySharp();
            var fightDetailsScrapySharp = await scraperScrapySharp.ScrapeFightDetails(
                "http://ufcstats.com/fight-details/4a704dae3091adaf"
                //"http://ufcstats.com/fight-details/2c0cddd12deb7450"
                );
            fightDetailsgAngleSharp.Should().BeEquivalentTo(fightDetailsScrapySharp);
        }

        [TestMethod]
        public async Task TestScrapeFighterDetails()
        {
            IUfcStatsScraper scraperscraperAngleSharp = new UfcStatsScraper();
            var fighterDetailsgAngleSharp = await scraperscraperAngleSharp.ScrapeFighterDetails("http://ufcstats.com/fighter-details/2cd428e9606856fd");
            IUfcStatsScraper scraperScrapySharp = new UfcStatsScraperScrapySharp();
            var fighterDetailsScrapySharp = await scraperScrapySharp.ScrapeFighterDetails("http://ufcstats.com/fighter-details/2cd428e9606856fd");
            fighterDetailsgAngleSharp.Should().BeEquivalentTo(fighterDetailsScrapySharp);
        }
    }
}