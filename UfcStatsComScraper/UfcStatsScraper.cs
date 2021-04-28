using HtmlAgilityPack;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScrapySharp.Extensions;

namespace UfcStatsComScraper
{
    public interface IUfcStatsScraper
    {
        List<EventListItem> ScrapeUpcoming(int? page = null);
        List<EventListItem> ScrapeCompleted(int? page = null);
        List<EventListItem> ScrapeSearch(string query);
    }

    public class UfcStatsScraper : IUfcStatsScraper
    {
        private readonly ScrapingBrowser _browser;
        public UfcStatsScraper()
        {
            _browser = new ScrapingBrowser();
        }

        public UfcStatsScraper(ScrapingBrowser browser)
        {
            _browser = browser;
        }

        public List<EventListItem> ScrapeUpcoming(int? page = null)
        {
            string url = Consts.UfcStatsEventsUpcoming;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (page != null)
                query["page"] = page.ToString();
            else
                query["page"] = "all";

            uriBuilder.Query = query.ToString();
            var uri = uriBuilder.Uri;

            WebPage homePage = _browser.NavigateToPage(uri);
            var result = new List<EventListItem>();
            return result;
        }
        public List<EventListItem> ParseUpcoming(HtmlNode node)
        {
            var result = node.CssSelect(".b-fight-details__table-body tr")
                .Select(ParseUpcomingItem)
                .ToList();
            return result;
        }

        public EventListItem ParseUpcomingItem(HtmlNode node)
        {
            var result = new EventListItem();
            result.Href = node.Attributes["data-link"]
                .Value;
            result.
            return result;
        }
        public List<EventListItem> ScrapeCompleted(int? page = null)
        {
            string url = Consts.UfcStatsEventsCompleted;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (page != null)
                query["page"] = page.ToString();
            else
                query["page"] = "all";
            uriBuilder.Query = query.ToString();
            var uri = uriBuilder.Uri;
            WebPage homePage = _browser.NavigateToPage(uri);
            var result = ParseCompleted(homePage.Html);
            return result;
        }

        public List<EventListItem> ParseCompleted(HtmlNode node)
        {
            var result = new List<EventListItem>();
            return result;
        }
        public List<EventListItem> ScrapeSearch(string query)
        {
            string url = Consts.UfcStatsEventSearch;
            var uriBuilder = new UriBuilder(url);
            var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryString["page"] = "all";
            uriBuilder.Query = query.ToString();
            var uri = uriBuilder.Uri;
            WebPage homePage = _browser.NavigateToPage(uri);
            var result = ParseSearch(homePage.Html);
            return result;
        }
        public List<EventListItem> ParseSearch(HtmlNode node)
        {
            var result = new List<EventListItem>();
            return result;
        }
    }
}