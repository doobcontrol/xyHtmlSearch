using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xyHtmlSearch
{
    public class PageParserConfig : INotifyPropertyChanged
    {
        //Page encoding
        public string encoding = "utf-8";
        public string Encoding { get => encoding; set => encoding = value; }

        public List<SearchParsStruct> dataSearchPars;

        public List<SearchParsStruct> urlSearchPars;

        public Dictionary<string, List<SearchParsStruct>> defaultRecordValuePars;
        public bool isDefaultRecordLocal = false;

        public UrlMatchingType urlMatchingType = UrlMatchingType.startWith;
        private string modelID = "";
        public string ModelID { get => modelID; set {
                modelID = value; 
                OnPropertyChanged(nameof(ModelID));
            }  
        }
        public List<string> matchUrls;
        public List<string> StartUrls;
        public List<(string start, string end)> StartEndUrls;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, 
                new PropertyChangedEventArgs(propertyName));
        }

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

        public static void Save()
        {
            JsonObject newCfg = new JsonObject();
            newCfg[cnFields] = new JsonArray();
            newCfg[cnUrlModels] = new JsonArray();
            foreach (string field in RecordFields)
            {
                newCfg[cnFields].AsArray().Add(field);
            }
            foreach (PageParserConfig ppc in allConfigs)
            {
                newCfg[cnUrlModels].AsArray().Add(toJson(ppc));
            }
            string jsonString = JsonSerializer.Serialize(newCfg);
            File.WriteAllText(configFile, jsonString);
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
            JsonArray RfJa = rootJo[cnFields].AsArray();
            RecordFields = new List<string>();
            foreach (JsonValue fieldJo in RfJa)
            {
                RecordFields.Add(fieldJo.GetValue<string>());
            }

            //urlModels
            JsonArray UmJa = rootJo[cnUrlModels].AsArray();
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
            retPpc.encoding = ppcJo[cnEncoding].GetValue<string>();
            retPpc.ModelID = ppcJo[cnModelID].GetValue<string>();

            //urlMatchingType
            retPpc.urlMatchingType =
                (UrlMatchingType)Enum.Parse(
                    typeof(UrlMatchingType),
                    ppcJo[cnMatchType].GetValue<string>());
            switch (retPpc.urlMatchingType)
            {
                case UrlMatchingType.exact:
                    JsonArray emJa = ppcJo[cnExact].AsArray();
                    retPpc.matchUrls = new List<string>();
                    foreach (JsonValue mu in emJa)
                    {
                        retPpc.matchUrls.Add(mu.GetValue<string>());
                    }
                    break;
                case UrlMatchingType.startWith: 
                    JsonArray swJa = ppcJo[cnStartWith].AsArray();
                    retPpc.StartUrls = new List<string>();
                    foreach (JsonValue mu in swJa)
                    {
                        retPpc.StartUrls.Add(mu.GetValue<string>());
                    }
                    break;
                case UrlMatchingType.startAndEndWith:
                    JsonArray seJa = ppcJo[cnStartEnd].AsArray();
                    retPpc.StartEndUrls = new List<(string start, string end)>();
                    foreach (JsonObject seJo in seJa)
                    {
                        retPpc.StartEndUrls.Add((
                            seJo[cnStart].GetValue<string>(),
                            seJo[cnEnd].GetValue<string>()));
                    }
                    break;
            }

            //urlPars
            if (ppcJo.ContainsKey(cnUrlPar))
            {
                JsonArray urlParJa = ppcJo[cnUrlPar].AsArray();
                retPpc.urlSearchPars = new List<SearchParsStruct>();
                foreach (JsonObject spJo in urlParJa)
                {
                    retPpc.urlSearchPars.Add(
                        SearchParsStruct.fromJson(spJo));
                }
            }

            //dataSearchPars

            //defaultRecordValuePars

            return retPpc;
        }

        static public JsonObject toJson(PageParserConfig ppc)
        {
            JsonObject retJo= new JsonObject();

            retJo[cnModelID] = ppc.ModelID;
            retJo[cnEncoding] = ppc.Encoding;

            //urlMatchingType
            retJo[cnMatchType] = ppc.urlMatchingType.ToString();
            JsonArray muJa;
            switch (ppc.urlMatchingType)
            {
                case UrlMatchingType.exact:
                    muJa = new JsonArray();
                    foreach (string mu in ppc.matchUrls)
                    {
                        muJa.Add(mu);
                    }
                    retJo[cnExact] = muJa;
                    break;
                case UrlMatchingType.startWith:
                    muJa = new JsonArray();
                    foreach (string mu in ppc.StartUrls)
                    {
                        muJa.Add(mu);
                    }
                    retJo[cnStartWith] = muJa;
                    break;
                case UrlMatchingType.startAndEndWith:
                    muJa = new JsonArray();
                    foreach ((string start, string end) in ppc.StartEndUrls)
                    {
                        JsonObject seJo = new JsonObject();
                        seJo[cnStart] = start;
                        seJo[cnEnd] = end;
                        muJa.Add(seJo);
                    }
                    retJo[cnStartEnd] = muJa;
                    break;
            }

            //urlPars
            if (ppc.urlSearchPars != null)
            {
                JsonArray upJa = new JsonArray();
                foreach (SearchParsStruct sps in ppc.urlSearchPars)
                {
                    upJa.Add(SearchParsStruct.toJson(sps));
                }
                retJo[cnUrlPar] = upJa;
            }

            //dataSearchPars

            //defaultRecordValuePars

            return retJo;
        }

        #region Config file JSON definition

        public static string cnFields = "fields";
        public static string cnUrlModels = "urlModels";
        public static string cnModelID = "mID";
        public static string cnEncoding = "encoding";
        public static string cnMatchType = "UrlMatchingType";
        public static string cnExact = "ExactMatchingList";
        public static string cnStartWith = "StartMatchingList";
        public static string cnStartEnd = "StartEndMatchingList";

        public static string cnUrlPar = "urlPar";

        public static string cnStart = "start";
        public static string cnEnd = "end";
        public static string cnSearchList = "searchList";
        public static string cnRecordDef = "recordDef";
        public static string cnAddBefore = "addBefore";
        public static string cnAddAfter = "addAfter";
        public static string cnCnd = "constant";

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
