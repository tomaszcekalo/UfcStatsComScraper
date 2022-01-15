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

        FighterDetails ScrapeFighterDetails(string url);
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
            var result = node.CssSelect("tr.b-statistics__table-row")
                .Select(ParseEventListItem)
                .ToList();
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
                ?.InnerText
                .Trim();
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

            var perRound = node
                .CssSelect("section.b-fight-details__section table.b-fight-details__table tbody.b-fight-details__table-body tr.b-fight-details__table-row");

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
                Method = content?["Method:"],
                Round = content?["Round:"],
                Time = content?["Time:"],
                TimeFormat = content?["Time format:"],
                Referee = content?["Referee:"],
                Matchup = node.CssSelect("section.b-fight-details__section")
                    .Where(x => HasMatchupLink(x))
                    .Select(x => ParseMatchup(x))
                    .FirstOrDefault(),
                BarChart = node.CssSelect(".b-fight-details__charts-col_pos_right")
                    .Select(ParseBarChart)
                    .FirstOrDefault(),
                FightDetailsCharts = node
                    .CssSelect(".b-fight-details__charts-body .b-fight-details__charts-col-row")
                    .Select(ParseFightDetailsChart)
                    .Take(2),
                TotalsPerRound = perRound
                    //.Where(x => x.ChildNodes.Count == 21
                    .Take(5)
                    .Select(ParseTotalsPerRound)
                    .ToList(),
                SignificantStrikesPerRounds = perRound
                    //.Where(x => x.ChildNodes.Count == 19)
                    .Skip(5)
                    .Take(5)
                    .Select(ParseSignificantStrikes)
                    .ToList()
            };
            return result;
        }

        public FightDetailsSignificantStrikes ParseSignificantStrikes(HtmlNode node)
        {
            var cells = node.CssSelect("td")
                .Select(x => x.CssSelect("p")
                    .Select(y => y.InnerText.Trim()).ToArray())
                .ToArray();

            var result = new FightDetailsSignificantStrikes()
            {
                SigStr = cells[1],
                SigStrPerc = cells[2],
                Head = cells[3],
                Body = cells[4],
                Leg = cells[5],
                Distance = cells[6],
                Clinch = cells[7],
                Ground = cells[8]
            };

            return result;
        }

        public FightDetailsTotals ParseTotalsPerRound(HtmlNode node)
        {
            var cells = node.CssSelect("td")
                .Select(x => x.CssSelect("p")
                    .Select(y => y.InnerText.Trim()).ToArray())
                .ToArray();
            var result = new FightDetailsTotals
            {
                KD = cells[1],
                SigStr = cells[2],
                SigStrPerc = cells[3],
                TotalStr = cells[4],
                TD = cells[5],
                TDP = cells[6],
                SubAtt = cells[7],
                Rev = cells[8],
                Ctrl = cells[9]
            };
            return result;
        }

        public FightDetailsChart ParseBarChart(HtmlNode node)
        {
            var result = new FightDetailsChart();
            result.Title = node.CssSelect("h4")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            result.Rows = node.CssSelect(".b-fight-details__bar-charts-row")
                .Select(ParseFightDetailsBarChartRow);
            return result;
        }

        public FightDetailsChartRow ParseFightDetailsBarChartRow(HtmlNode node)
        {
            var result = new FightDetailsChartRow();
            result.Left = node.CssSelect(".b-fight-details__bar-chart-text_style_light-red")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            result.Title = node.CssSelect(".b-fight-details__bar-chart-title")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            result.Right = node.CssSelect(".b-fight-details__bar-chart-text_style_light-blue")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            return result;
        }

        public FightDetailsChart ParseFightDetailsChart(HtmlNode node)
        {
            var result = new FightDetailsChart();
            result.Title = node.CssSelect("h4")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            result.Rows = node.CssSelect(".b-fight-details__charts-row")
                .Select(ParseFightDetailsChartRow);
            return result;
        }

        public FightDetailsChartRow ParseFightDetailsChartRow(HtmlNode node)
        {
            var result = new FightDetailsChartRow();
            result.Left = node.CssSelect(".b-fight-details__charts-num_pos_left")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            result.Title = node.CssSelect(".b-fight-details__charts-row-title")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            result.Right = node.CssSelect(".b-fight-details__charts-num_pos_right")
                .FirstOrDefault()
                ?.InnerHtml
                .Trim();
            return result;
        }

        public bool HasMatchupLink(HtmlNode node)
        {
            //return true;
            return node.CssSelect("a.b-fight-details__collapse-link").Any(y => y.InnerText.Trim().Equals("Matchup"));
        }

        public FightMatchup ParseMatchup(HtmlNode node)
        {
            var result = new FightMatchup();
            result.Items = node.CssSelect("table.b-fight-details__table tbody.b-fight-details__table-body tr")
                .Where(x => x.CssSelect("td").Count() == 3)
                .Select(x => new
                {
                    Key = x.CssSelect("td").Select(td => td.InnerText.Trim()).First(),
                    Value = x.CssSelect("td").Select(td => td.InnerText.Trim()).Skip(1).ToArray()
                })
                .Where(x => !string.IsNullOrEmpty(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);
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
            result.MatchupUrl = cells[0]
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

        public FighterDetails ScrapeFighterDetails(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            var result = ParseFighterDetails(homePage.Html);
            return result;
        }

        public FighterDetails ParseFighterDetails(HtmlNode node)
        {
            var info = node.CssSelect("ul.b-list__box-list li")
                .Select(x => x.ChildNodes
                    .Select(y => y.InnerText.Trim())
                    .Where(y => !string.IsNullOrEmpty(y))
                    .ToArray())
                .Where(x => x.Length >= 2)
                .ToDictionary(x => x[0], x => x[1]);

            var result = new FighterDetails
            {
                Record = node.CssSelect(".b-content__title-record")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Height = info["Height:"],
                Weight = info["Weight:"],
                Reach = info["Reach:"],
                Stance = info["STANCE:"],
                DateOfBirth = info["DOB:"],
                StrikesLandedPerMinute = info["SLpM:"],
                StrikingAccuracy = info["Str. Acc.:"],
                StrikesAbsorbedPerMinute = info["SApM:"],
                StrikeDefence = info["Str. Def:"],
                AverageTakedownsLandedPer15Minutes = info["TD Avg.:"],
                TakedownAccuracy = info["TD Acc.:"],
                TakedownDefense = info["TD Def.:"],
                AverageSugmissionsAttemptedPer15Minutes = info["Sub. Avg.:"]
            };
            result.Fights = node.CssSelect(".b-fight-details__table-body tr")
                .Where(x => x.ChildNodes.Count > 3)
                .Where(x => x.CssSelect("td").Count() >= 10)
                .Select(ParseFightItem)
                .ToList();
            return result;
        }
    }
}