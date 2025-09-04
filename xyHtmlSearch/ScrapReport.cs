using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyHtmlSearch
{
    public class ScrapReport
    {
        public enum rType
        {
            Msg,
            PageTask,
            PageStart,
            PageDone,
            FileTask,
            FileStart,
            FileDone,
            RecordList,
            NewUrlsCount,
            Error
        }

        internal rType reportType = rType.Msg;
        internal string msg = string.Empty;
        internal Exception? e;

        internal List<(string, string)>? pageTaskList;
        internal string pageUrl;
        internal (string pageUrl, string configId, bool succeed) pageRusult;

        internal Dictionary<string, string>? fileTaskDict;
        internal string fileUrl;
        internal (string fileUrl, bool succeed) fileRusult;

        private List<Dictionary<string, string>> recordList;
        private int newUrlsCount = 0;

        public rType ReportType { get => reportType; }
        public string Msg { get => msg; }
        public List<(string, string)>? PageTaskList { get => pageTaskList; }
        public Dictionary<string, string>? FileTaskDict { get => fileTaskDict; }
        public Exception? E { get => e; }
        public string FileUrl { get => fileUrl; set => fileUrl = value; }
        public (string fileUrl, bool succeed) FileRusult { get => fileRusult; set => fileRusult = value; }
        public string PageUrl { get => pageUrl; set => pageUrl = value; }
        public (string pageUrl, string configId, bool succeed) PageRusult { get => pageRusult; set => pageRusult = value; }
        public List<Dictionary<string, string>> RecordList { get => recordList; set => recordList = value; }
        public int NewUrlsCount { get => newUrlsCount; set => newUrlsCount = value; }

        static public void reportMsg(IProgress<ScrapReport> progress, string msg)
        {
            progress.Report(new ScrapReport() { msg = msg });
        }
        static public void reportError(IProgress<ScrapReport> progress,
            string msg, Exception e)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.Error,
                msg = msg,
                e = e
            });
        }

        static public void reportFileTask(IProgress<ScrapReport> progress,
            Dictionary<string, string>? fileTaskDict)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.FileTask,
                fileTaskDict = fileTaskDict
            });
        }

        static public void reportFileStart(IProgress<ScrapReport> progress,
            string fileUrl)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.FileStart,
                fileUrl = fileUrl
            });
        }

        static public void reportFileDone(IProgress<ScrapReport> progress,
            (string fileUrl, bool succeed) fileRusult)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.FileDone,
                fileRusult = fileRusult
            });
        }

        static public void reportPageTask(IProgress<ScrapReport> progress,
            List<(string, string)>? pageTaskList)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.PageTask,
                pageTaskList = pageTaskList
            });
        }

        static public void reportPageStart(IProgress<ScrapReport> progress,
            string pageUrl)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.PageStart,
                PageUrl = pageUrl
            });
        }

        static public void reportPageDone(IProgress<ScrapReport> progress,
            (string pageUrl, string configId, bool succeed) pageRusult)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.PageDone,
                PageRusult = pageRusult
            });
        }

        static public void reportRecordList(IProgress<ScrapReport> progress,
            List<Dictionary<string, string>> recordList)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.RecordList,
                recordList = recordList
            });
        }
        static public void reportNewUrlsCount(IProgress<ScrapReport> progress,
            int newUrlsCount)
        {
            progress.Report(new ScrapReport()
            {
                reportType = rType.NewUrlsCount,
                newUrlsCount = newUrlsCount,
                msg = $"Found {newUrlsCount} new urls."
            });
        }
    }
}
