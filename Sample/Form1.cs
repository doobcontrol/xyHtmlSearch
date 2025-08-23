using Microsoft.VisualBasic.Logging;
using NanoXLSX;
using NanoXLSX.Styles;
using System.Resources;
using xyHtmlSearch;

namespace Sample
{
    public partial class Form1 : Form
    {
        IProgress<ScrapReport> progress;
        CancellationTokenSource cts;
        public Form1()
        {
            InitializeComponent();

            progress = new SimpleProgress<ScrapReport>(scrappingReport);

            PageParserConfig.nameParser += PageParserConfigNameParser;
            PageScraper.RecordFields = 
                Enum.GetNames(typeof(DRecord)).ToList<string>();

            //https://Amaravati.org/teachings
            PageParserConfig ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("Amaravati", ppc);
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
            ppc.urlSearchPars = sPars;
            Dictionary<string, List<SearchParsStruct>> defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars; 
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "ENG";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.LANG_TREE_CODE.ToString(), sPars);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "Amaravati";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.AUTHOR.ToString(), sPars);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "Amaravati.org";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.PUBLISHER.ToString(), sPars);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "Amaravati Buddhist Monastery St Margarets Great Gaddesden Hertfordshire, HP1 3BZ England";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.LINK_CONTACT.ToString(), sPars);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "A";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.STATUS.ToString(), sPars);

            //youtube channel
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("youtubeC", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""";
            sps.end = @"/videos"",""";
            sps.addBefore = "https://www.youtube.com";
            sps.addAfter = "/videos";
            sPars.Add(sps);
            ppc.urlSearchPars = sPars;

            //youtube videos
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("youtubeVideos", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"{""richItemRenderer"":{""content"":{""videoRenderer"":{""videoId"":""";
            sps.end = @"""trackingParams"":""";

            Dictionary<string, SearchParsStruct> spsDic
                = new Dictionary<string, SearchParsStruct>();
            SearchParsStruct sps1 = new SearchParsStruct();
            sps1.start = @"""title"":{""runs"":[{""text"":""";
            sps1.end = @"}],""";
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new SearchParsStruct();
            sps1.start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""";
            sps1.end = @""",""";
            sps1.addBefore = "https://www.youtube.com";
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            Dictionary<string, List<SearchParsStruct>> dataSearchPars
            = new Dictionary<string, List<SearchParsStruct>>();
            dataSearchPars.Add("default", sPars);
            ppc.dataSearchPars = dataSearchPars;

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "YOUTUBE";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);

            //youtube video list
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("youtubeVideosList", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"{""playlistVideoRenderer"":{""videoId"":""";
            sps.end = @"}]}}}],""videoInfo"":{""runs"":[{""text"":""";

            spsDic
                = new Dictionary<string, SearchParsStruct>();
            sps1 = new SearchParsStruct();
            sps1.start = @",""title"":{""runs"":[{""text"":""";
            sps1.end = @"""}],""";
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new SearchParsStruct();
            sps1.start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""";
            sps1.end = @""",""webPageType"":""WEB_PAGE_TYPE_WATCH"",""";
            sps1.addBefore = "https://www.youtube.com";
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new SearchParsStruct();
            sps1.start = @"""shortBylineText"":{""runs"":[{""text"":""";
            sps1.end = @""",""";
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            dataSearchPars = new Dictionary<string, List<SearchParsStruct>>();
            dataSearchPars.Add("default", sPars);
            ppc.dataSearchPars = dataSearchPars;

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "YOUTUBE";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);

            //https://meridian-trust.org/category/teachers/ajahn-sumedho/
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("meridian-trust", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<div class=""video-thumb-title"">";
            sps.end = @"</a>";

            spsDic
                = new Dictionary<string, SearchParsStruct>();
            sps1 = new SearchParsStruct();
            sps1.start = @""">";
            sps1.end = @"";
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new SearchParsStruct();
            sps1.start = @"<a href=""";
            sps1.end = @""">";
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new SearchParsStruct();
            sps1.start = @"<div class=""c-title"">";
            sps1.end = @" | <span class=""";
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            dataSearchPars = new Dictionary<string, List<SearchParsStruct>>();
            dataSearchPars.Add("default", sPars);
            ppc.dataSearchPars = dataSearchPars;

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "MP3";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "admin@meridian-trust.org";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.LINK_CONTACT.ToString(), sPars);

            //https://amaravati.org/teachings/audio/
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

            string vediosUrl = "https://www.youtube.com"
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

            List<Dictionary<string, string>> vRecords
                = new List<Dictionary<string, string>>();
            foreach (string s in retList)
            {
                vRecords.Add(htmlParserTool.findMuti(s, spsDic));
            }

            textBox1.Text = "Total count: " + retList.Count + "\r\n";
            foreach (Dictionary<string, string> vDic in vRecords)
            {
                textBox1.Text += vDic["title"] + "; " + vDic["url"] + "\r\n";
            }

            writeToXlsx(vRecords);

            panel1.Enabled = true;
        }

        Workbook workbook;
        private void writeToXlsx(List<Dictionary<string, string>> vRecords)
        {
            // Create a new workbook with a worksheet named "Sheet1"

            foreach (Dictionary<string, string> vDic in vRecords)
            {
                workbook.WS.Down();
                foreach (string s in PageScraper.RecordFields)
                {
                    workbook.WS.Value(vDic[s]);
                }
            }

            // Save the workbook
            //workbook.Save();
        }

        private string PageParserConfigNameParser(string url)
        {
            string retStr = null;
            
            if (url == "https://Amaravati.org/teachings")
            {
                retStr = "Amaravati";
            }
            else if (url.StartsWith("https://www.youtube.com/c/"))
            {
                retStr = "youtubeC";
            }
            else if (url.StartsWith("https://www.youtube.com/playlist?list="))
            {
                retStr = "youtubeVideosList";
            }
            else if (url.StartsWith("https://www.youtube.com/")
                && url.EndsWith("/videos"))
            {
                retStr = "youtubeVideos";
            }
            else if (url == "https://meridian-trust.org/category/teachers/ajahn-sumedho/")
            {
                retStr = "meridian-trust";
            }
            else
            {
                throw new Exception(
                    "Unable to parse the URL. Please provide a configuration for it: "
                    + url
                    );
            }

            return retStr;
        }
        private void scrappingReport(ScrapReport data)
        {
            switch (data.ReportType)
            {
                case ScrapReport.rType.Msg:
                    //tslbMsg.Text = data.Msg;
                    //showMsg(data.Msg);
                    //XyLog.log(data.Msg);
                    break;
                case ScrapReport.rType.Error:
                    showMsg("Error: " + data.Msg);
                    showMsg("    Info: " + data.E.Message);
                    //tslbMsg.Text = data.Msg;
                    //showMsg(data.Msg);
                    //XyLog.log(data.Msg);
                    //string errorInfo = data.E.Message + "\r\n" + data.E.StackTrace;
                    //if (data.E.InnerException != null)
                    //{
                    //    errorInfo += "\r\nInnerException: " + data.E.InnerException.Message
                    //        + "\r\n" + data.E.InnerException.StackTrace;
                    //}
                    //XyLog.log(errorInfo);
                    break;

                case ScrapReport.rType.FileTask:
                    //fileTaskDict = data.FileTaskDict;
                    //spbFileTask.Minimum = 0;
                    //spbFileTask.Maximum = data.FileTaskDict.Count;
                    //spbFileTask.Value = 0;
                    //spbFileTask.Visible = true;
                    //showFileTask(data.FileTaskDict);
                    //showPageTaskInfo(null, "0/" + data.FileTaskDict.Count);
                    break;
                case ScrapReport.rType.FileStart:
                    //changeRowColor(fileRowDic[data.FileUrl],
                    //    ColorTranslator.FromHtml("#EAE0D5"),
                    //    ColorTranslator.FromHtml("#22333B")
                    //    );
                    break;
                case ScrapReport.rType.FileDone:
                    //spbFileTask.Value = spbFileTask.Maximum - fileTaskDict.Count + 1;
                    //showPageTaskInfo(null,
                    //    spbFileTask.Value + "/" + spbFileTask.Maximum);
                    //if (data.FileRusult.succeed)
                    //{
                    //    changeRowColor(fileRowDic[data.FileRusult.fileUrl],
                    //    ColorTranslator.FromHtml("#81E979"),
                    //    ColorTranslator.FromHtml("#595A4A"));
                    //    updateFileStatistic(true);
                    //}
                    //else
                    //{
                    //    changeRowColor(fileRowDic[data.FileRusult.fileUrl],
                    //    ColorTranslator.FromHtml("#DBC2CF"),
                    //    ColorTranslator.FromHtml("#DBC2CF"));
                    //    updateFileStatistic(false);
                    //}
                    break;

                case ScrapReport.rType.PageTask:
                    //pageTaskList = data.PageTaskList;
                    //showPageTaskInfo("");
                    break;
                case ScrapReport.rType.PageStart:
                    showMsg("Start: " + data.PageUrl);
                    //showPageTaskInfo(data.PageUrl);
                    break;
                case ScrapReport.rType.PageDone:
                    showMsg("Done: " + data.PageRusult.pageUrl);
                    //showPageTaskInfo(data.PageRusult.pageUrl + " ("
                    //    + (data.PageRusult.succeed ?
                    //        Resources.msg_succeed : Resources.msg_fail)
                    //    + ")");
                    //spbFileTask.Visible = false;
                    //showMsg("");
                    //XyLog.log("");
                    //if (data.PageRusult.succeed)
                    //{
                    //    updatePageStatistic(true, data.PageRusult.configId);
                    //}
                    //else
                    //{
                    //    updatePageStatistic(false, data.PageRusult.configId);
                    //}
                    break;
                case ScrapReport.rType.RecordList:
                    List<Dictionary<string, string>>  vRecords = data.RecordList;
                    //showMsg("Total count: " + vRecords.Count);
                    //foreach (Dictionary<string, string> vDic in vRecords)
                    //{
                    //    showMsg(vDic["title"] + "; " + vDic["url"]);
                    //}
                    showMsg("Get books: " + vRecords.Count);
                    writeToXlsx(vRecords);
                    break;
            }
        }

        private async void btnScrap_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;

            cts = new CancellationTokenSource();

            workbook = new Workbook("Amaravati.xlsx", "Sheet1");

            foreach (string s in PageScraper.RecordFields)
            {
                workbook.WS.Value(s, BasicStyles.Bold);
            }

            await PageScraper.ScrapAllAsync(
                new List<string> { "https://Amaravati.org/teachings" },
                cts.Token,
                progress
                );

            panel1.Enabled = true;

            // Save the workbook
            workbook.Save();
        }

        private void showMsg(string msg)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(() =>
                {
                    showMsg(msg);
                }
                );
            }
            else
            {
                textBox1.AppendText(msg + "\r\n");
            }
        }
    }
    public enum DRecord
    {
        AUTHOR,//:   Author Name
        AUTHOR_SORT,//:   IGNORE
        LANG_TREE_CODE,//:   3-letter format - ENG, HIN, SPA, etc
        BOOK,//:   Name of BOOK, YOUTUBE, Audio, etc
        FORMAT,//:   PDF, MP3, MP4, YOUTUBE, etc
        KEYWORD,//:   IGNORE
        PUBLISHER,//:   Website
        LINK,//:   LINK to the BOOK
        LINK_CONTACT,//:   Website Contact
        STATUS,//:   A (this is a constant)
    }
}
