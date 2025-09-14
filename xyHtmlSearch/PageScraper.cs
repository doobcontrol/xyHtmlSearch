using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace xyHtmlSearch
{
    public class PageScraper
    {
        private static HttpClientDownloader hcd = new HttpClientDownloader();

        public static async Task<List<string>> ScrapOneAsync(
            string url,
            int level,
            IProgress<ScrapReport> progress)
        {
            PageParserConfig ppc = PageParserConfig.get(url);

            if (ppc == null)
            {
                throw new Exception("No parser model config for url: " + url);
            }

            string htmlStr =
                await hcd.GetHtmlStringAsync(url, ppc.Encoding);
            htmlStr = htmlStr.Replace("\r", "").Replace("\n", "");

            Dictionary<string, string> DefaultRecord = null;
            if (DefaultRecordStack.Count > 0)
            {
                DefaultRecord =
                    new Dictionary<string, string>(
                        DefaultRecordStack.Peek().r);
            }
            else
            {
                DefaultRecord = new Dictionary<string, string>();
                foreach (string field in PageParserConfig.RecordFields)
                {
                    DefaultRecord.Add(field, "");
                }
            }
            if (ppc.defaultRecordValuePars != null)
            {
                //parse default record
                foreach (string field in ppc.defaultRecordValuePars.Keys)
                {
                    string fValue =
                        htmlParserTool.findOne(htmlStr, 
                        ppc.defaultRecordValuePars[field]);

                    DefaultRecord[field] = fValue;
                }
                if (!ppc.isDefaultRecordLocal)
                {
                    DefaultRecordStack.Push((DefaultRecord, level));
                }
            }

            //parse data
            if (ppc.dataSearchPars != null
                && ppc.dataSearchPars.Last().recordDef != null)
            {
                List<string> dataList =
                    htmlParserTool.findList(htmlStr, ppc.dataSearchPars);

                //search record
                Dictionary<string, List<SearchParsStruct>> recordDef
                    = ppc.dataSearchPars.Last().recordDef;
                List<Dictionary<string, string>> recordList
                    = new List<Dictionary<string, string>>();
                foreach (string data in dataList)
                {
                    Dictionary<string, string> recordDic
                        = [];

                    recordList.Add(htmlParserTool.findMuti(data, recordDef));
                }

                foreach(string key in DefaultRecord.Keys)
                {
                    foreach(Dictionary<string,string> record in recordList)
                    {
                        if (!record.ContainsKey(key))
                        {
                            record.Add(key, DefaultRecord[key]);
                        }
                    }
                }

                ScrapReport.reportRecordList(progress, recordList);
            }

            List<string> urlList = null;
            if (ppc.urlSearchPars != null)
            {
                //parse navgate url
                urlList = new List<string>();
                foreach (List<SearchParsStruct> record in ppc.urlSearchPars)
                {
                    List<string> uList =
                        htmlParserTool.findList(htmlStr, record);
                    urlList.AddRange(uList);
                }
            }
            return urlList;
        }

        public static async Task ScrapAllAsync(
            List<string> urlList,
            CancellationToken token,
            IProgress<ScrapReport> progress)
        {
            Stack<(string url, int l)> uStack = [];
            pushUrlList(uStack, urlList, 0);
            int currentLevel = 0;
            while (true)
            {
                if (uStack.Count == 0)
                {
                    break;
                }

                (string url, int l) uNode = uStack.Pop();
                string url = uNode.url;
                int level = uNode.l;

                if(level <= currentLevel)
                {
                    //clean buffered default record
                    while(DefaultRecordStack.Count > 0 &&
                        DefaultRecordStack.Peek().l >= level)
                    {
                        DefaultRecordStack.Pop();
                    }
                }
                currentLevel = level;

                List<string> newList;
                ScrapReport.reportPageStart(progress, url);
                try
                {
                    newList = await ScrapOneAsync(url, level, progress);
                }
                catch (Exception ex)
                {
                    ScrapReport.reportPageDone(progress,
                        (url, "failed", false));
                    ScrapReport.reportError(progress, "Scrap failed: " + url, ex);
                    continue;
                }
                ScrapReport.reportPageDone(progress,
                    (url, "succeed", true));
                if (newList != null)
                {
                    pushUrlList(uStack, newList, level + 1);
                    ScrapReport.reportNewUrlsCount(progress, newList.Count);
                }
                if (uStack.Count == 0)
                {
                    break;
                }
                if (token.IsCancellationRequested) {
                    //save the break point?

                    break; 
                }
            }
            DefaultRecordStack.Clear();
        }

        public static Stack<
            (Dictionary<string, string> r, int l)
            > DefaultRecordStack = [];

        private static void pushUrlList(
            Stack<(string url, int l)> uStack, 
            List<string> uList,
            int level
            )
        {
            for (int i = uList.Count - 1; i >= 0; i--)
            {
                uStack.Push((uList[i], level));
            }
        } 
    }
}
