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

        public List<List<SearchParsStruct>> urlSearchPars;

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
            if(url == null)
            {
                return null;
            }

            PageParserConfig retPpc = null;

            foreach (PageParserConfig ppc in AllConfigs)
            {
                if (ppc.urlMatchingType == UrlMatchingType.exact
                    && ppc.matchUrls.Contains(url))
                {
                    return ppc;
                }
            }
            foreach (PageParserConfig ppc in AllConfigs)
            {
                if (ppc.urlMatchingType == UrlMatchingType.startAndEndWith)
                {
                    foreach((string start, string end) in ppc.StartEndUrls)
                    {
                        if (url.StartsWith(start) && url.EndsWith(end))
                        {
                            return ppc;
                        }
                    }
                }
            }
            foreach (PageParserConfig ppc in AllConfigs)
            {
                if (ppc.urlMatchingType == UrlMatchingType.startWith)
                {
                    foreach (string start in ppc.StartUrls)
                    {
                        if (url.StartsWith(start))
                        {
                            return ppc;
                        }
                    }
                }
            }
            return retPpc;
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
                retPpc.urlSearchPars = new List<List<SearchParsStruct>>();

                foreach (JsonArray spJa in urlParJa)
                {
                    List<SearchParsStruct> spsList
                        = new List<SearchParsStruct>();
                    retPpc.urlSearchPars.Add(spsList);
                    foreach (JsonObject spJo in spJa)
                    {
                        spsList.Add(SearchParsStruct.fromJson(spJo));
                    }
                }
            }

            //dataSearchPars
            if(ppcJo.ContainsKey(cnDataPar))
            {
                JsonArray dataParJa = ppcJo[cnDataPar].AsArray();
                retPpc.dataSearchPars = new List<SearchParsStruct>();
                foreach (JsonObject spJo in dataParJa)
                {
                    retPpc.dataSearchPars.Add(
                        SearchParsStruct.fromJson(spJo));
                }
            }

            //defaultRecordValuePars
            if(ppcJo.ContainsKey(cnDefaultPar))
            {
                JsonObject drJo = ppcJo[cnDefaultPar].AsObject();
                retPpc.defaultRecordValuePars
                    = new Dictionary<string, List<SearchParsStruct>>();
                foreach (var item in drJo)
                {
                    JsonArray spJa = item.Value.AsArray();
                    List<SearchParsStruct> spsList
                        = new List<SearchParsStruct>();
                    foreach (JsonObject spJo in spJa)
                    {
                        spsList.Add(SearchParsStruct.fromJson(spJo));
                    }
                    retPpc.defaultRecordValuePars.Add(item.Key, spsList);
                }
            }

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
                JsonArray upJas = new JsonArray();
                foreach (List<SearchParsStruct> sps in ppc.urlSearchPars)
                {
                    JsonArray upJa = new JsonArray();
                    upJas.Add(upJa);
                    foreach (SearchParsStruct spss in sps)  
                        upJa.Add(SearchParsStruct.toJson(spss));
                }
                retJo[cnUrlPar] = upJas;
            }

            //dataSearchPars
            if (ppc.dataSearchPars != null)
            {
                JsonArray dpJa = new JsonArray();
                foreach (SearchParsStruct sps in ppc.dataSearchPars)
                {
                    dpJa.Add(SearchParsStruct.toJson(sps));
                }
                retJo[cnDataPar] = dpJa;
            }

            //defaultRecordValuePars
            if (ppc.defaultRecordValuePars != null)
            {
                JsonObject drJo = new JsonObject();
                foreach (string field in ppc.defaultRecordValuePars.Keys)
                {
                    JsonArray spJa = new JsonArray();
                    foreach (SearchParsStruct sps in ppc.defaultRecordValuePars[field])
                    {
                        spJa.Add(SearchParsStruct.toJson(sps));
                    }
                    drJo[field] = spJa;
                }
                retJo[cnDefaultPar] = drJo;
            }

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
        public static string cnDataPar = "dataPar";
        public static string cnDefaultPar = "defaultRecordValuePars";

        public static string cnStart = "start";
        public static string cnEnd = "end";
        public static string cnSearchList = "searchList";
        public static string cnRecordDef = "recordDef";
        public static string cnAddBefore = "addBefore";
        public static string cnAddAfter = "addAfter";
        public static string cnUrlDecode = "UrlDecode";
        public static string cnHtmlDecode = "HtmlDecode";
        public static string cnUnescape = "Unescape";
        public static string cnFrontSplitter = "frontSplitter";
        public static string cnBackSplitter = "backSplitter";
        public static string cnConstant = "constant";
        public static string cnTransFunc = "transFunc";

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
