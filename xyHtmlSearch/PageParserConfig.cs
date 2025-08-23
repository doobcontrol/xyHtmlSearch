using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyHtmlSearch
{
    public class PageParserConfig
    {
        //Page encoding
        public string encoding = "utf-8";
        public string Encoding { get => encoding; set => encoding = value; }

        public Dictionary<string, List<SearchParsStruct>> dataSearchPars
            = new Dictionary<string, List<SearchParsStruct>>();

        public List<SearchParsStruct> urlSearchPars;

        public static Dictionary<string, PageParserConfig> ppcDic
            = new Dictionary<string, PageParserConfig>();
        public static PageParserConfig get(string url)
        {
            return ppcDic[nameParser(url)];
        }
        public static nameParser nameParser;

        public Dictionary<string, List<SearchParsStruct>> defaultRecordValuePars;
        public bool isDefaultRecordLocal = false;
    }
    public delegate string nameParser(string url);
}
