using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace UfcStatsComScraper
{
    public interface IUfcStatsScraper
    {
        Task<IEnumerable<EventListItem>> ScrapeUpcoming(int? page = null);

        Task<IEnumerable<EventListItem>> ScrapeCompleted(int? page = null);

        Task<IEnumerable<EventListItem>> ScrapeSearch(string query);

        Task<EventDetails> ScrapeEventDetails(string url);

        Task<FightDetails> ScrapeFightDetails(string url);

        Task<FighterDetails> ScrapeFighterDetails(string url);
    }
}