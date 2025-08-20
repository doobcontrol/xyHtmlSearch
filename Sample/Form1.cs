using xyHtmlSearch;

namespace Sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            string url = "https://Amaravati.org/teachings";
            PageParserConfig ppc = new PageParserConfig();

            HttpClientDownloader hcd = new HttpClientDownloader();
            string htmlStr =
                await hcd.GetHtmlStringAsync(url, ppc.Encoding);

            List<SearchParsStruct> sPars = new List<SearchParsStruct>();
            SearchParsStruct sps;
            sps = new SearchParsStruct();
            sps.start = @"<span>Videos<i class=";
            sps.end = @"<span>Support<i class=";
            sPars.Add(sps);
            sps = new SearchParsStruct(true);
            sps.start = @"<ul class=""sub-menu"">";
            sps.end = @"</ul>";
            sPars.Add(sps);
            sps = new SearchParsStruct(true);
            sps.start = @"""><a href=""";
            sps.end = @"""><span>";
            sPars.Add(sps);

            List<string> retList = htmlParserTool.findList(htmlStr, sPars);
            foreach (string s in retList)
            {
                textBox1.Text += s + "\r\n";
            }
            panel1.Enabled = true;
        }

        private async void btnYouTube_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            string url = 
                "https://www.youtube.com/c/amaravatibuddhistmonastery";
            PageParserConfig ppc = new PageParserConfig();

            HttpClientDownloader hcd = new HttpClientDownloader();
            string htmlStr =
                await hcd.GetHtmlStringAsync(url, ppc.Encoding);

            List<SearchParsStruct> sPars = new List<SearchParsStruct>();
            SearchParsStruct sps;
            sps = new SearchParsStruct();
            sps.start = @"{""title"":{""runs"":[{""text"":""Videos"",""navigationEndpoint"":";
            sps.end = @"""apiUrl"":""/youtubei/v1/browse""}}";
            sPars.Add(sps);
            sps = new SearchParsStruct();
            sps.start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""";
            sps.end = @""",""";
            sPars.Add(sps);

            string vediosUrl= "https://www.youtube.com" 
                + htmlParserTool.findOne(htmlStr, sPars);
            htmlStr =
                await hcd.GetHtmlStringAsync(vediosUrl, ppc.Encoding);

            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"{""richItemRenderer"":{""content"":{""videoRenderer"":{""videoId"":""";
            sps.end = @"""trackingParams"":""";
            sPars.Add(sps);

            List<string> retList = htmlParserTool.findList(htmlStr, sPars);

            Dictionary<string, SearchParsStruct> spsDic
                = new Dictionary<string, SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.start = @"""title"":{""runs"":[{""text"":""";
            sps.end = @"}],""";
            spsDic.Add("title", sps);
            sps = new SearchParsStruct();
            sps.start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""";
            sps.end = @""",""";
            spsDic.Add("url", sps);

            List<Dictionary<string,string>> vRecords 
                = new List<Dictionary<string,string>>();
            foreach(string s in retList)
            {
                vRecords.Add(htmlParserTool.findMuti(s, spsDic));
            }

            textBox1.Text = "Total count: " + retList.Count + "\r\n";
            foreach (Dictionary<string, string> vDic in vRecords)
            {
                textBox1.Text += vDic["title"] + "; " + vDic["url"] + "\r\n";
            }
            panel1.Enabled = true;
        }
    }
}
