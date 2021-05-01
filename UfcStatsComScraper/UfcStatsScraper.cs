using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UfcStatsComScraper
{
    public interface IUfcStatsScraper
    {
        IEnumerable<EventListItem> ScrapeUpcoming(int? page = null);
        IEnumerable<EventListItem> ScrapeCompleted(int? page = null);
        IEnumerable<EventListItem> ScrapeSearch(string query);
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

        public IEnumerable<EventListItem> ScrapeUpcoming(int? page = null)
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
            var result = ParseEventList(homePage.Html);
            return result;
        }
        public IEnumerable<EventListItem> ParseEventList(HtmlNode node)
        {
            var result = node.CssSelect(".b-fight-details__table-body tr")
                .Select(ParseEventListItem);
            return result;
        }

        public EventListItem ParseEventListItem(HtmlNode node)
        {
            var i = node
                .CssSelect("td.b-statistics__table-col i.b-statistics__table-content")
                .FirstOrDefault();

            var result = new EventListItem();

            result.Href = i?.CssSelect("a")
                .FirstOrDefault()
                ?.Attributes["href"]
                .Value;
            result.Name = i?.CssSelect("a")
                .FirstOrDefault()
                ?.InnerText;
            result.Date = i?.CssSelect("span")
                .FirstOrDefault()
                ?.InnerText
                .Trim();
            result.Location = node.CssSelect("td.b-statistics__table-col_style_big-top-padding")
                .FirstOrDefault()
                ?.InnerText
                .Trim();
            result.IsNext = node
                .CssSelect("img.b-statistics__icon")
                .Any();
            return result;
        }
        public IEnumerable<EventListItem> ScrapeCompleted(int? page = null)
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
            var result = ParseEventList(homePage.Html);
            return result;
        }

        public IEnumerable<EventListItem> ScrapeSearch(string query)
        {
            string url = Consts.UfcStatsEventSearch;
            var uriBuilder = new UriBuilder(url);
            var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryString["page"] = "all";
            uriBuilder.Query = queryString.ToString();
            var uri = uriBuilder.Uri;
            WebPage homePage = _browser.NavigateToPage(uri);
            var result = ParseEventList(homePage.Html);
            return result;
        }

        public EventDetails ScrapeEventDetails(string id)
        {
            string url = Consts.UfcStatsEventsCompleted + id;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            uriBuilder.Query = query.ToString();
            var uri = uriBuilder.Uri;
            WebPage homePage = _browser.NavigateToPage(uri);
            var result = ParseEventDetails(homePage.Html);
            return result;
        }

        public EventDetails ParseEventDetails(HtmlNode node)
        {
            var result = new EventDetails();
            result.Fights = node.CssSelect(".b-fight-details__table-body tr")
                .Select(ParseFightItem);

            return result;
        }

        public FightItem ParseFightItem(HtmlNode node)
        {
            var cells = node.CssSelect("td")
                .ToList();
            var result = new FightItem();
            result.Fighters = cells[1]
                .CssSelect("a")
                .Select(ParseLink);
            result.MatchupUrl = cells[4]
                .CssSelect("a")
                .Select(x => x.Attributes["href"].Value)
                .FirstOrDefault();
            return result;
        }

        public Link ParseLink(HtmlNode node)
        {
            var result = new Link
            {
                Href = node.Attributes["href"].Value,
                Text = node.InnerText.Trim()
            };
            return result;
        }
    }
}