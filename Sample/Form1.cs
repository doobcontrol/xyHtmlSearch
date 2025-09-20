using Microsoft.VisualBasic.Logging;
using NanoXLSX;
using NanoXLSX.Styles;
using System.Collections.Generic;
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
            Text = "LinkSearcher";
            progress = new SimpleProgress<ScrapReport>(scrappingReport);

            btnStart.Visible = false;
            btnYouTube.Visible = false;

            PageParserConfig.nameParser += PageParserConfigNameParser;
            PageParserConfig.RecordFields =
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
            ppc.urlSearchPars = new List<List<SearchParsStruct>> { sPars };
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
            ppc.urlSearchPars = new List<List<SearchParsStruct>> { sPars };

            //youtube videos
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("youtubeVideos", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"{""richItemRenderer"":{""content"":{""videoRenderer"":{""videoId"":""";
            sps.end = @"""trackingParams"":""";

            Dictionary<string, List<SearchParsStruct>> spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            List<SearchParsStruct> sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"""title"":{""runs"":[{""text"":""",
            end = @"}],"""} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""",
            end = @""",""",
            addBefore = "https://www.youtube.com"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

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
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @",""title"":{""runs"":[{""text"":""",
            end = @"""}],"""} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"""commandMetadata"":{""webCommandMetadata"":{""url"":""",
            end = @""",""webPageType"":""WEB_PAGE_TYPE_WATCH"",""",
            addBefore = "https://www.youtube.com"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"""shortBylineText"":{""runs"":[{""text"":""",
            end = @""","""} };
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

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
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @""">",
            end = @""} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"<a href=""",
            end = @""">"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"<div class=""c-title"">",
            end = @" | <span class="""} };
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

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
            ppc.isDefaultRecordLocal = true;

            //https://amaravati.org/teachings/audio/
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("DhammaTalksAudio", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<div class=""pod-entry__title"">";
            sps.end = @"</a>";

            spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @""">",
            end = @""} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"<a href=""",
            end = @""">"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "MP3";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            ppc.isDefaultRecordLocal = true;

            //https://amaravati.org/dhamma-audio-library/?wpv-category=audio
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("dhamma-audio-library", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<div class=""tb-fields-and-text""";
            sps.end = @"<p></p>";

            spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"content=""",
            end = @""""} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"Audio: <a href=""",
            end = @""""} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"</a><br>By:",
            end = @"<br>"} };
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "MP3";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            ppc.isDefaultRecordLocal = true;

            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.start = @"<a class=""wpv-filter-next-link js-wpv-pagination-next-link page-link ""  href=""";
            sps.end = @"""";
            sps.addBefore = "https://amaravati.org";
            sPars.Add(sps);
            ppc.urlSearchPars = new List<List<SearchParsStruct>> { sPars };

            //https://amaravati.org/teachings/chanting/
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("ChantingAudio", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<div class=""pod-entry__title"">";
            sps.end = @"<div id=""";

            spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @""">",
            end = @"</a>>"} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"<a href=""",
            end = @""">"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"<div class=""pod-entry__author"">",
            end = @"</div>"} };
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "MP3";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            ppc.isDefaultRecordLocal = true;

            //https://amaravati.org/teachings/meditation/
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("MeditationAudio", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<div class=""pod-entry__title"">";
            sps.end = @"</a>";

            spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @""">",
            end = @""} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"<a href=""",
            end = @""">"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "MP3";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            ppc.isDefaultRecordLocal = true;

            //https://media.amaravati.org/dhamma-books
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("dhamma-books", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<h3 class=""font-serif text-xl";
            sps.end = @"<div class=""flex justify-end";

            spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"title"">        ",
            end = @"      </h3>"} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"href=""",
            end = @"""      target=""_blank""      wire:key=""pdf""    >PDF"} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @" >",
            end = @"</a>"} };
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "PDF";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            ppc.isDefaultRecordLocal = true;

            //https://media.amaravati.org/dhamma-articles
            ppc = new PageParserConfig();
            PageParserConfig.ppcDic.Add("dhamma-articles", ppc);
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct(true);
            sps.start = @"<hgroup ";
            sps.end = @"</hgroup>";

            spsDic
                = new Dictionary<string, List<SearchParsStruct>>();
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"title"">        ",
            end = @"      </h3>"} };
            spsDic.Add(DRecord.BOOK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"href=""",
            end = @""""} };
            spsDic.Add(DRecord.LINK.ToString(), sps1);
            sps1 = new List<SearchParsStruct>()
            { new SearchParsStruct(){
            start = @"wire:navigate        >",
            end = @"</a>"} };
            spsDic.Add(DRecord.AUTHOR.ToString(), sps1);
            sps.recordDef = spsDic;

            sPars.Add(sps);
            ppc.dataSearchPars = new List<List<SearchParsStruct>> { sPars };

            defaultRecordValuePars
                = new Dictionary<string, List<SearchParsStruct>>();
            ppc.defaultRecordValuePars = defaultRecordValuePars;
            sPars = new List<SearchParsStruct>();
            sps = new SearchParsStruct();
            sps.constant = "WEB";
            sPars.Add(sps);
            defaultRecordValuePars.Add(DRecord.FORMAT.ToString(), sPars);
            ppc.isDefaultRecordLocal = true;
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
        }

        Workbook workbook;
        private void writeToXlsx(List<Dictionary<string, string>> vRecords)
        {
            // Create a new workbook with a worksheet named "Sheet1"

            foreach (Dictionary<string, string> vDic in vRecords)
            {
                workbook.WS.Down();
                foreach (string s in PageParserConfig.RecordFields)
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
            else if (url == "https://amaravati.org/teachings/audio/")
            {
                retStr = "DhammaTalksAudio";
            }
            else if (url.StartsWith("https://amaravati.org/dhamma-audio-library/?wpv-category=audio"))
            {
                retStr = "dhamma-audio-library";
            }
            else if (url == "https://amaravati.org/teachings/chanting/")
            {
                retStr = "ChantingAudio";
            }
            else if (url == "https://amaravati.org/teachings/meditation/")
            {
                retStr = "MeditationAudio";
            }
            else if (url == "https://media.amaravati.org/dhamma-books")
            {
                retStr = "dhamma-books";
            }
            else if (url == "https://media.amaravati.org/dhamma-articles")
            {
                retStr = "dhamma-articles";
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
                    List<Dictionary<string, string>> vRecords = data.RecordList;
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
            showMsg("Begin:\r\n");
            cts = new CancellationTokenSource();

            workbook = new Workbook("LinkSearcher.xlsx", "Sheet1");

            foreach (string s in PageParserConfig.RecordFields)
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
            showMsg("\r\nAll tasks are complete, and the results have been written to the LinkSearcher.xlsx file.");
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
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
