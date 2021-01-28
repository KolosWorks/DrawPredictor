using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KolosWorks.DrawPredicter.Model
{
    class WebScraper
    {
        private const string fixturesUrl = "https://www.thesportsman.com/football/competitions/england/premier-league/fixtures";
        private const string baseUrl = "https://www.thesportsman.com";

        public WebScraper()
        {

        }

        public async Task<List<Match>> GetMatches()
        {
            var matchHrefList = await GetMatchHrefs();
            var matchList = new List<Match>();

            foreach (Match matchRef in matchHrefList)
            {
                Match match = await GetMatch(matchRef);
                matchList.Add(match);
            }

            return matchList;
        }

        public async Task<List<Match>> GetMatchHrefs()
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(fixturesUrl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var matchContainerDivs = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("flc-match-item-inner")).ToList();

            List<HtmlNode> matchHrefNodes = new List<HtmlNode>();
            matchContainerDivs.ForEach(x => matchHrefNodes.Add(x.Descendants("a").First()));

            DateTime ukDate = GetUKDate();

            var matchHrefs = new List<Match>();
            foreach (var matchContainerNode in matchContainerDivs)
            {
                var hrefString = matchContainerNode.Descendants("a").First().GetAttributeValue("href", string.Empty);
                var dateString = hrefString.Substring(16, 10);
                var matchDate = DateTime.Parse(dateString);

                var dayDiff = Convert.ToByte((matchDate - ukDate).TotalDays);
                if (dayDiff <= 14)
                {
                    var matchHref = new Match(hrefString, dateString, dayDiff);
                    matchHrefs.Add(matchHref);
                }
            }

            return matchHrefs;
        }

        public async Task<Match> GetMatch(Match matchRef)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(baseUrl + matchRef.Location);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            // Get Team Names ----------------
            var matchContainer = htmlDocument.DocumentNode.SelectNodes("//div[@class='score-container']").First();

            matchRef.HomeTeam = GetHomeAwayTeamName(matchContainer, 1);
            matchRef.AwayTeam = GetHomeAwayTeamName(matchContainer, 2);

            // Get Team Draw Rates ----------------
            var headToHeadContainer = htmlDocument.DocumentNode.SelectNodes("//div[@class='head-to-head-stats-contain']").First();

            var drawRates = GetHomeAwayTeamDrawRate(headToHeadContainer);
            matchRef.HomeTeamDrawRate = drawRates.Item1;
            matchRef.AwayTeamDrawRate = drawRates.Item2;

            return matchRef;
        }
        private string GetHomeAwayTeamName(HtmlNode matchContainer, byte homeAway)
        {
            string divClass = homeAway == 1 ? "team-left" : homeAway == 2 ? "team-right" : "";

            var teamText = "";
            if (divClass != "")
            {
                var teamDiv = matchContainer.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals(divClass)).First();

                teamText = teamDiv.Descendants("a").First().GetDirectInnerText();
            }
            
            return teamText;
        }
        private Tuple<byte, byte> GetHomeAwayTeamDrawRate(HtmlNode headToHeadContainer)
        {
            string homeTeamDrawRate = "";
            string awayTeamDrawRate = "";

            var headToHeadDiv = headToHeadContainer.Descendants("tr")
            .Where(node => node.GetAttributeValue("class", "")
            .Equals("hth-stat-row"))
            .ElementAt(3);

            homeTeamDrawRate = headToHeadDiv.Descendants("span").First().GetDirectInnerText().Split('%')[0];
            awayTeamDrawRate = headToHeadDiv.Descendants("span").ElementAt(1).GetDirectInnerText().Split('%')[0];

            return Tuple.Create(Convert.ToByte(homeTeamDrawRate), Convert.ToByte(awayTeamDrawRate));
        }

        private static DateTime GetUKDate()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDocument = web.Load("https://www.timeanddate.com/worldclock/uk/london");

            string dateString = htmlDocument.DocumentNode.SelectNodes("//span[@id='ctdat']").First().InnerHtml;
            var result = DateTime.Parse(dateString);

            return result;
        }


        public async Task<string> EbayTest()
        {
            string url = "https://www.ebay.co.uk/sch/i.html?_nkw=samsung+q9fn&_in_kw=1&_ex_kw=&_sacat=0&LH_Complete=1&_udlo=&_udhi=&_samilow=&_samihi=&_sadis=15&_stpos=SG1+2RW&_sargn=-1%26saslc%3D1&_salic=3&_sop=12&_dmd=1&_ipg=50&_fosrp=1";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            var productListItems = productsHtml[0].Descendants("li")
                 .Where(node => node.GetAttributeValue("id", "")
                 .Contains("item")).ToList();

            Random rnd = new Random();
            int itemNum = rnd.Next(0, productListItems.Count);

            return productListItems[itemNum].InnerHtml;
        }
    }
}
