using System;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Threading.Tasks;
using KolosWorks.DrawPredicter.Model;
using KolosWorks.Util.Controls;
using System.Collections.Generic;

[assembly: WebResource("KolosWorks.DrawPredicter.Resources.Default.js", "text/javascript")]
namespace KolosWorks.DrawPredicter.FromBehind
{
    public partial class _Default : PageX
    {
        private static string mainContainerDivId = "mainContainerDiv";
        private string testDivId = "testDiv";
        private string testDivContentId = "testDivContent";
        private string testDivCloseButtonId = "testDivCloseButton";
        private string doScrapTestButtonId = "doScrapTestButton";

        private HtmlGenericControl testDiv;
        private HtmlGenericControl testDivContent;

        private static WebScraper scraper;

        protected void Page_Load(object sender, EventArgs e)
        {
            scraper = new WebScraper();

            CreateControls();
        }

        public static async Task<List<Match>> FootieTest()
        {
            var matchList = await scraper.GetMatches();

            return matchList;
        }

        private void CreateControls()
        {
            AddDiv(mainContainerDivId);

            UpdatePanel updatePanel = new UpdatePanel();

            testDiv = AddDiv(testDivId, null, updatePanel);
            testDiv.Attributes.Add("class", "kwHidden");
            testDivContent = AddDiv(testDivContentId, null, testDiv);

            AddButton(doScrapTestButtonId, "Test", DoScrapTest_Click, updatePanel, true);
            AddButton(testDivCloseButtonId, "X", TestDivClose_Click, testDiv);

            Form.Controls.Add(updatePanel);
        }

        protected async void DoScrapTest_Click(object sender, EventArgs e)
        {
            testDiv.Attributes.Remove("class");

            string data = await scraper.EbayTest();
            testDivContent.InnerHtml = data;
        }
        private void TestDivClose_Click(object sender, EventArgs e)
        {
            testDiv.Attributes.Add("class", "kwHidden");
        }




        [WebMethod]
        public static object GetMatches()
        {
            var data = Task.Run(() => FootieTest());
            data.Wait();
            return data.Result;
        }

        [WebMethod]
        public static new object OnFrontEndConstruct()
        {
            var list = GetPropertyList(() => mainContainerDivId);
            return list;
        }

        protected override void OnPreRender(EventArgs e)
        {
            Emit("DadaPage", "KolosWorks.DrawPredicter.Resources.Default.js");
        }
    }
}