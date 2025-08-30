using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace xyHtmlSearch
{
    public class PageParserConfig
    {
        //Page encoding
        public string encoding = "utf-8";
        public string Encoding { get => encoding; set => encoding = value; }

        public List<SearchParsStruct> dataSearchPars
            = new List<SearchParsStruct>();

        public List<SearchParsStruct> urlSearchPars;

        public Dictionary<string, List<SearchParsStruct>> defaultRecordValuePars;
        public bool isDefaultRecordLocal = false;

        public UrlMatchingType urlMatchingType = UrlMatchingType.startWith;
        public string modelID = "";
        public List<string> matchUrls;
        public List<string> StartUrls;
        public List<(string start, string end)> StartEndUrls;


        public static List<string> RecordFields; //Definition of a search record

        public static Dictionary<string, PageParserConfig> ppcDic
            = new Dictionary<string, PageParserConfig>();
        public static PageParserConfig get(string url)
        {
            return ppcDic[nameParser(url)];
        }
        public static nameParser nameParser;

        private static string configFile;

        private static List<PageParserConfig> allConfigs;
        public static List<PageParserConfig> AllConfigs
        {
            get
            {
                if(allConfigs == null)
                {                    
                    init();
                }
                return allConfigs;
            }
        }
        public static void init(string cFile = "xySearch.cfg")
        {
            configFile = cFile;

            if (!File.Exists(configFile))
            {
                JsonObject newCfg = new JsonObject();
                newCfg[cnFields] = new JsonArray();
                newCfg[cnUrlModels] = new JsonArray();
                string jsonString = JsonSerializer.Serialize(newCfg);
                File.WriteAllText(configFile, jsonString);
            }

            string json = File.ReadAllText(configFile);
            JsonObject? rootJo = JsonSerializer.Deserialize<JsonObject>(json);
            if (rootJo == null)
            {
                throw new Exception("Load configure failure");
            }

            //RecordFields
            JsonArray RfJa = rootJo[cnFields].GetValue<JsonArray>();
            RecordFields = new List<string>();
            foreach (JsonObject fieldJo in RfJa)
            {
                RecordFields.Add(fieldJo.GetValue<string>());
            }

            //urlModels
            JsonArray UmJa = rootJo[cnUrlModels].GetValue<JsonArray>();
            allConfigs = new List<PageParserConfig>();
            foreach (JsonObject ppcJo in UmJa)
            {
                allConfigs.Add(fromJson(ppcJo));
            }
        }

        static public PageParserConfig fromJson(JsonObject ppcJo)
        {
            PageParserConfig retPpc = new PageParserConfig();
            //encoding
            if (ppcJo.ContainsKey(cnEncoding))
            {
                retPpc.encoding = ppcJo[cnEncoding].GetValue<string>();
            }
            //urlPars
            if (ppcJo.ContainsKey(cnUrlPar))
            {
                JsonArray urlParJa = ppcJo[cnUrlPar].GetValue<JsonArray>();
                retPpc.urlSearchPars = new List<SearchParsStruct>();
                foreach (JsonObject spJo in urlParJa)
                {
                    //retPpc.urlSearchPars.Add(
                    //    SearchParsStruct.fromJson(spJo));
                }
            }

            return retPpc;
        }

        static public JsonObject toJson(PageParserConfig ppc)
        {
            JsonObject retJo= new JsonObject();
            

            return retJo;
        }

        #region Config file JSON definition

        public static string cnFields = "fields";
        public static string cnUrlModels = "urlModels";
        public static string cnModelID = "mID";
        public static string cnUrlPar = "urlPar";
        public static string cnStart = "start";
        public static string cnEnd = "end";
        public static string cnEncoding = "encoding";

        #endregion
    }
    public delegate string nameParser(string url);
    public enum UrlMatchingType
    {
        exact,
        startWith,
        startAndEndWith
    }
}
