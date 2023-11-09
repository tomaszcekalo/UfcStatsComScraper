using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace UfcStatsComScraper
{
    public class UfcStatsScraper : IUfcStatsScraper
    {
        private IConfiguration config;
        private IBrowsingContext context;

        public UfcStatsScraper()
        {
            config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);
        }
        public async Task<IEnumerable<EventListItem>> ScrapeCompleted(int? page = null)
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
            var document = await context.OpenAsync(uri.AbsoluteUri);
            var result = ParseEventList(document);
            return result;
        }

        public async Task<EventDetails> ScrapeEventDetails(string url)
        {
            var document = await context.OpenAsync(url);
            var result = ParseEventDetails(document);
            return result;
        }

        private EventDetails ParseEventDetails(IDocument node)
        {
            var listBoxItems = node
                .QuerySelectorAll("ul.b-list__box-list li.b-list__box-list-item")
                .Select(x => x.ChildNodes
                    .Select(y => y.TextContent.Trim())
                    .Where(y => !string.IsNullOrEmpty(y))
                    .ToArray())
                .ToDictionary(x => x[0], x => x[1]);
            var result = new EventDetails
            {
                Fights = node.QuerySelectorAll(".b-fight-details__table-body tr")
                    .Select(ParseFightItem),
                Date = listBoxItems["Date:"],
                Location = listBoxItems["Location:"]
            };
            return result;
        }

        private FightItem ParseFightItem(IElement node)
        {
            var cells = node.QuerySelectorAll("td");
            var result = new FightItem();
            result.Fighters = cells[1]
                .QuerySelectorAll("a")
                .Select(ParseLink);
            result.MatchupUrl = cells[0]
                .QuerySelectorAll("a")
                .Select(x => x.Attributes["href"].Value)
                .FirstOrDefault();
            result.KD = cells[2]
                .QuerySelectorAll("p")
                .Select(x => x.TextContent.Trim());
            result.STR = cells[3]
                .QuerySelectorAll("p")
                .Select(x => x.TextContent.Trim());
            result.TD = cells[4]
                .QuerySelectorAll("p")
                .Select(x => x.TextContent.Trim());
            result.SUB = cells[5]
                .QuerySelectorAll("p")
                .Select(x => x.TextContent.Trim());
            result.WeightClass = cells[6]
                .TextContent
                .Trim();
            result.Method = cells[7]
                .TextContent
                .Trim();
            result.Round = cells[8]
                .TextContent
                .Trim();
            result.Time = cells[9]
                .TextContent
                .Trim();
            return result;
        }
        public Link ParseLink(IElement node)
        {
            var result = new Link
            {
                Href = node.Attributes["href"].Value,
                Text = node.TextContent.Trim()
            };
            return result;
        }

        public async Task<FightDetails> ScrapeFightDetails(string url)
        {
            var document = await context.OpenAsync(url);
            var result = ParseFightDetails(document);
            return result;
        }

        private FightDetails ParseFightDetails(IDocument node)
        {
            var content = node
                .QuerySelector(".b-fight-details__content .b-fight-details__text")
                ?.Children
                .Where(x => x.FirstChild != null)
                .ToDictionary(x =>
                    x.QuerySelector(".b-fight-details__label")
                        ?.TextContent.Trim(),
                    x => x.ChildNodes
                        .Select(y => y.TextContent.Trim())
                        .LastOrDefault(y => !string.IsNullOrEmpty(y))
                );

            var perRound = node
                .QuerySelectorAll("section.b-fight-details__section table.b-fight-details__table tbody tr.b-fight-details__table-row");

            var result = new FightDetails
            {
                EventLink = node.QuerySelectorAll("h2.b-content__title a")
                    .Select(ParseLink)
                    .FirstOrDefault(),
                Fighters = node.QuerySelectorAll(".b-fight-details__person")
                    .Select(ParseFightDetailsFighter),
                Title = node.QuerySelector("i.b-fight-details__fight-title")
                    ?.TextContent
                    .Trim(),
                Method = content?["Method:"],
                Round = content?["Round:"],
                Time = content?["Time:"],
                TimeFormat = content?["Time format:"],
                Referee = content?["Referee:"],
                Matchup = node.QuerySelectorAll("section.b-fight-details__section")
                    .Where(x => HasMatchupLink(x))
                    .Select(x => ParseMatchup(x))
                    .FirstOrDefault(),
                BarChart = node.QuerySelectorAll(".b-fight-details__charts-col_pos_right")
                    .Select(ParseBarChart)
                    .FirstOrDefault(),
                FightDetailsCharts = node
                    .QuerySelectorAll(".b-fight-details__charts-body .b-fight-details__charts-col-row")
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
        public FightDetailsSignificantStrikes ParseSignificantStrikes(IElement node)
        {
            var cells = node.QuerySelectorAll("td")
                .Select(x => x.QuerySelectorAll("p")
                    .Select(y => y.TextContent.Trim()).ToArray())
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
        public FightDetailsTotals ParseTotalsPerRound(IElement node)
        {
            var cells = node.QuerySelectorAll("td")
                .Select(x => x.QuerySelectorAll("p")
                    .Select(y => y.TextContent.Trim()).ToArray())
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
        public FightDetailsChart ParseFightDetailsChart(IElement node)
        {
            var result = new FightDetailsChart();
            result.Title = node.QuerySelector("h4")
                ?.InnerHtml
                .Trim();
            result.Rows = node.QuerySelectorAll(".b-fight-details__charts-row")
                .Select(ParseFightDetailsChartRow);
            return result;
        }
        public FightDetailsChartRow ParseFightDetailsChartRow(IElement node)
        {
            var result = new FightDetailsChartRow();
            result.Left = node.QuerySelector(".b-fight-details__charts-num_pos_left")
                ?.InnerHtml
                .Trim();
            result.Title = node.QuerySelector(".b-fight-details__charts-row-title")
                ?.InnerHtml
                .Trim();
            result.Right = node.QuerySelector(".b-fight-details__charts-num_pos_right")
                ?.InnerHtml
                .Trim();
            return result;
        }
        public FightDetailsChart ParseBarChart(IElement node)
        {
            var result = new FightDetailsChart();
            result.Title = node.QuerySelector("h4")
                ?.InnerHtml
                .Trim();
            result.Rows = node.QuerySelectorAll(".b-fight-details__bar-charts-row")
                .Select(ParseFightDetailsBarChartRow);
            return result;
        }
        public FightDetailsChartRow ParseFightDetailsBarChartRow(IElement node)
        {
            var result = new FightDetailsChartRow();
            result.Left = node.QuerySelector(".b-fight-details__bar-chart-text_style_light-red")
                ?.InnerHtml
                .Trim();
            result.Title = node.QuerySelector(".b-fight-details__bar-chart-title")
                ?.InnerHtml
                .Trim();
            result.Right = node.QuerySelector(".b-fight-details__bar-chart-text_style_light-blue")
                ?.InnerHtml
            .Trim();
            return result;
        }
        public FightMatchup ParseMatchup(IElement node)
        {
            var result = new FightMatchup();
            result.Items = node.QuerySelectorAll("table.b-fight-details__table tbody.b-fight-details__table-body tr")
                .Where(x => x.QuerySelectorAll("td").Count() == 3)
                .Select(x => new
                {
                    Key = x.QuerySelectorAll("td").Select(td => td.TextContent.Trim()).First(),
                    Value = x.QuerySelectorAll("td").Select(td => td.TextContent.Trim()).Skip(1).ToArray()
                })
                .Where(x => !string.IsNullOrEmpty(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);
            return result;
        }
        public bool HasMatchupLink(IElement node)
        {
            return node
                .QuerySelectorAll("a.b-fight-details__collapse-link")
                .Any(y => y.TextContent.Trim().Equals("Matchup"));
        }

        private FightDetailsFighter ParseFightDetailsFighter(IElement node)
        {
            var result = new FightDetailsFighter
            {
                Score = node.QuerySelector(".b-fight-details__person-status")
                    ?.TextContent
                    .Trim(),
                FighterDetailsLink = node.QuerySelectorAll(".b-link b-fight-details__person-link")
                    .Select(ParseLink)
                    .FirstOrDefault(),
                Title = node.QuerySelector("p.b-fight-details__person-title")
                    ?.TextContent
                    .Trim()
            };
            return result;
        }

        public async Task<FighterDetails> ScrapeFighterDetails(string url)
        {
            var document = await context.OpenAsync(url);
            var result = ParseFighterDetails(document);
            return result;
        }

        private FighterDetails ParseFighterDetails(IDocument node)
        {
            var info = node.QuerySelectorAll("ul.b-list__box-list li")
                .Select(x => x.ChildNodes
                    .Select(y => y.TextContent.Trim())
                    .Where(y => !string.IsNullOrEmpty(y))
                    .ToArray())
                .Where(x => x.Length >= 2)
                .ToDictionary(x => x[0], x => x[1]);

            var result = new FighterDetails
            {
                Record = node.QuerySelector(".b-content__title-record")
                    ?.TextContent
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
            result.Fights = node.QuerySelectorAll(".b-fight-details__table-body tr")
                .Where(x => x.Children.Count() > 3)
                .Where(x => x.QuerySelectorAll("td").Count() >= 10)
                .Select(ParseFightItem)
                .ToList();
            return result;
        }

        public async Task<IEnumerable<EventListItem>> ScrapeSearch(string query)
        {
            string url = Consts.UfcStatsEventSearch;
            var uriBuilder = new UriBuilder(url);
            var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryString["page"] = "all";
            uriBuilder.Query = queryString.ToString();
            var uri = uriBuilder.Uri;
            var document = await context.OpenAsync(uri.AbsoluteUri);
            var result = ParseEventList(document);
            return result;
        }

        public async Task<IEnumerable<EventListItem>> ScrapeUpcoming(int? page = null)
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

            var document = await context.OpenAsync(uri.AbsoluteUri);
            var result = ParseEventList(document);
            return result;
        }

        private IEnumerable<EventListItem> ParseEventList(IDocument document)
        {
            var result = document.QuerySelectorAll("tr.b-statistics__table-row")
                .Select(ParseEventListItem)
                .ToList();
            return result;
        }

        private EventListItem ParseEventListItem(IElement node)
        {
            var i = node
                .QuerySelector("td.b-statistics__table-col i.b-statistics__table-content");

            var result = new EventListItem();

            result.Href = i?.QuerySelector("a")
                ?.Attributes["href"]
                .Value;
            result.Name = i?.QuerySelector("a")
                ?.TextContent
                .Trim();
            result.Date = i?.QuerySelector("span")
                ?.TextContent
                .Trim();
            result.Location = node.QuerySelector("td.b-statistics__table-col_style_big-top-padding")
                ?.TextContent
                .Trim();
            result.IsNext = node
                .QuerySelectorAll("img.b-statistics__icon")
                .Any();
            return result;
        }
    }
}