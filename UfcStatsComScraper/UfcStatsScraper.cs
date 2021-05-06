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
        EventDetails ScrapeEventDetails(string url);
        FightDetails ScrapeFightDetails(string url);
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

        public EventDetails ScrapeEventDetails(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            var result = ParseEventDetails(homePage.Html);
            return result;
        }

        public FightDetails ScrapeFightDetails(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            var result = ParseFightDetails(homePage.Html);
            return result;
        }

        private FightDetails ParseFightDetails(HtmlNode node)
        {
            var content = node
                .CssSelect(".b-fight-details__content .b-fight-details__text")
                .FirstOrDefault()
                ?.ChildNodes
                .Where(x => x.FirstChild != null)
                .ToDictionary(x =>
                    x.CssSelect(".b-fight-details__label")
                        .FirstOrDefault()
                        ?.InnerText.Trim(),
                    x => x.ChildNodes
                        .Select(y => y.InnerText.Trim())
                        .LastOrDefault(y => !string.IsNullOrEmpty(y))
                );
            var result = new FightDetails
            {
                EventLink = node.CssSelect("h2.b-content__title a")
                    .Select(ParseLink)
                    .FirstOrDefault(),
                Fighters = node.CssSelect(".b-fight-details__person")
                    .Select(ParseFightDetailsFighter),
                Title = node.CssSelect("i.b-fight-details__fight-title")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Method = content["Method:"],
                Round = content["Round:"],
                Time = content["Time:"],
                TimeFormat = content["Time format:"],
                Referee = content["Referee:"]
            };
            return result;
        }

        public FightDetailsFighter ParseFightDetailsFighter(HtmlNode node)
        {
            var result = new FightDetailsFighter
            {
                Score = node.CssSelect(".b-fight-details__person-status")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                FighterDetailsLink = node.CssSelect(".b-link b-fight-details__person-link")
                    .Select(ParseLink)
                    .FirstOrDefault(),
                Title = node.CssSelect("p.b-fight-details__person-title")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim()
            };
            return result;
        }

        public EventDetails ParseEventDetails(HtmlNode node)
        {
            var listBoxItems = node
                .CssSelect("ul.b-list__box-list li.b-list__box-list-item")
                .Select(x => x.ChildNodes
                    .Select(y => y.InnerText.Trim())
                    .Where(y => !string.IsNullOrEmpty(y))
                    .ToArray())
                .ToDictionary(x => x[0], x => x[1]);
            var result = new EventDetails
            {
                Fights = node.CssSelect(".b-fight-details__table-body tr")
                    .Select(ParseFightItem),
                Date = listBoxItems["Date:"],
                Location = listBoxItems["Location:"]
            };
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
            result.KD = cells[2]
                .CssSelect("p")
                .Select(x => x.InnerText.Trim());
            result.STR = cells[3]
                .CssSelect("p")
                .Select(x => x.InnerText.Trim());
            result.TD = cells[4]
                .CssSelect("p")
                .Select(x => x.InnerText.Trim());
            result.SUB = cells[5]
                .CssSelect("p")
                .Select(x => x.InnerText.Trim());
            result.WeightClass = cells[6]
                .InnerText
                .Trim();
            result.Method = cells[7]
                .InnerText
                .Trim();
            result.Round = cells[8]
                .InnerText
                .Trim();
            result.Time = cells[9]
                .InnerText
                .Trim();
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