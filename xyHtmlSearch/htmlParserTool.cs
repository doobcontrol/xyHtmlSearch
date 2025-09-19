using System;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace xyHtmlSearch
{
    public class htmlParserTool
    {
        //find first one
        static public (int, int) findIndexBetween(
            string strForFind,
            string startStr,
            string endStr,
            int startFindIndex = 0)
        {
            int start_index = startFindIndex;
            int end_index = strForFind.Length;

            if (end_index <= start_index)
            {
                if (end_index == -1)
                {
                    return (-1, -1);
                }
            }

            if (startStr.Length != 0)
            {
                start_index = strForFind.IndexOf(startStr, startFindIndex);
                if (start_index == -1)
                {
                    return (-1, -1);
                }
                start_index += startStr.Length;
            }
            if (endStr.Length != 0)
            {
                end_index = strForFind.IndexOf(endStr, start_index);
                if (end_index == -1)
                {
                    return (-1, -1);
                }
            }

            if (end_index <= start_index)
            {
                return (-1, -1);
            }

            // check if substring between start_index and end_index contains startStr
            if (startStr.Length != 0 && end_index > start_index)
            {
                int start_indexIn =
                    strForFind.LastIndexOf(startStr, end_index, end_index - start_index);
                if (start_indexIn != -1)
                {
                    start_index = start_indexIn + startStr.Length;
                }
            }

            return (start_index, end_index);
        }

        //find first one
        static public string findBetween(
            string strForFind,
            string startStr,
            string endStr,
            int startFindIndex = 0)
        {
            (int, int) indexTuple = findIndexBetween(
                strForFind,
                startStr,
                endStr,
                startFindIndex);

            int startIndex = indexTuple.Item1;
            int endIndex = indexTuple.Item2;
            string retStr = null;
            if (startIndex != -1)
            {
                retStr = strForFind.Substring(startIndex, endIndex - startIndex);
            }
            return retStr;
        }

        //find all
        static public List<string> findAllBetween(
            string strForFind,
            string startStr,
            string endStr,
            int startFindIndex = 0)
        {
            List<string> retList = new List<string>();

            int beginIndex = startFindIndex;
            int endStrLen = endStr.Length;

            while (true)
            {
                (int, int) indexTuple = findIndexBetween(
                    strForFind,
                    startStr,
                    endStr,
                    beginIndex);

                int retStartIndex = indexTuple.Item1;
                int retEndIndex = indexTuple.Item2;

                if (retStartIndex == -1)
                {
                    break;
                }
                else
                {
                    string tempStr = 
                        strForFind.Substring(retStartIndex, retEndIndex - retStartIndex);                    
                    retList.Add(tempStr);
                    beginIndex = retEndIndex + endStrLen;
                }
            }

            return retList;
        }

        static public string findOne(
            string strForFind, List<SearchParsStruct> sPars)
        {
            if(sPars.Count == 0)
            {
                throw new Exception("No search pars");
            }
            string retStr = strForFind;
            foreach (SearchParsStruct sps in sPars) {
                if (sps.searchList) {
                    throw new Exception("Error pars");
                }
                else if (sps.constant != null)
                {
                    retStr = sps.constant;
                }
                else
                {
                    retStr = findBetween(retStr, 
                        (sps.start == null) ? "": sps.start, 
                        (sps.end == null) ? "" : sps.end);
                }
            }
            retStr = finishHandle(retStr, sPars.Last());
            return retStr;
        }

        static public List<string> findList(
            string strForFind, List<SearchParsStruct> sPars)
        {
            if (sPars.Count == 0)
            {
                throw new Exception("No search pars");
            }
            List<string> retList = null;

            string searchStr = strForFind;
            foreach (SearchParsStruct sps in sPars)
            {
                if (retList == null)
                {
                    if (sps.searchList)
                    {
                        retList = 
                            findAllBetween(searchStr,
                                (sps.start == null) ? "" : sps.start,
                                (sps.end == null) ? "" : sps.end);
                    }
                    else
                    {
                        searchStr = 
                            findBetween(searchStr,
                                (sps.start == null) ? "" : sps.start,
                                (sps.end == null) ? "" : sps.end);
                    }
                }
                else
                {
                    if (sps.searchList)
                    {
                        List<string> tempList = new List<string>();
                        foreach (string s in retList)
                        {
                            tempList.AddRange(
                                findAllBetween(s,
                                    (sps.start == null) ? "" : sps.start,
                                    (sps.end == null) ? "" : sps.end));
                        }
                        retList = tempList;
                    }
                    else
                    {
                        List<string> tempList = new List<string>();
                        foreach (string s in retList)
                        {
                            tempList.Add(findBetween(s,
                                (sps.start == null) ? "" : sps.start,
                                (sps.end == null) ? "" : sps.end));
                        }
                        retList = tempList;
                    }
                }

            }
            if(retList == null && searchStr != strForFind)
            {
                retList = [searchStr];
            }
            if (retList != null)
            {
                for (int i = 0; i < retList.Count; i++)
                {
                    retList[i] = finishHandle(retList[i], sPars.Last());
                }
            }
            return retList;
        }

        static public Dictionary<string,string> findMuti(
            string strForFind,
            Dictionary<string, List<SearchParsStruct>> spsDic
            )
        {
            Dictionary<string, string> retDic = 
                new Dictionary<string, string>();

            foreach( string s in spsDic.Keys)
            {
                string tempStr = findOne(strForFind, spsDic[s]);
                retDic.Add(s, tempStr);
            }
            return retDic;
        }

        static private string finishHandle(string fString, SearchParsStruct sps)
        {
            if (fString == null) return null;

            string retStr = fString;
            if(sps.HtmlDecode)
            {
                retStr = WebUtility.HtmlDecode(retStr);
            }
            if (sps.UrlDecode)
            {
                retStr = WebUtility.UrlDecode(retStr);
            }
            if(sps.Unescape)
            {
                retStr = Regex.Unescape(retStr);
            }
            if (sps.frontSplitter != null & sps.frontSplitter != "")
            {
                int fsIndex = retStr.IndexOf(sps.frontSplitter);
                if(fsIndex != -1)
                {
                    retStr = retStr.Substring(0, fsIndex);
                }
            }
            if(sps.backSplitter != null & sps.backSplitter != "")
            {
                int psIndex = retStr.LastIndexOf(sps.backSplitter);
                if (psIndex != -1)
                {
                    retStr = retStr.Substring(psIndex + sps.backSplitter.Length);
                }
            }
            if (sps.addBefore != null)
            {
                retStr = sps.addBefore + retStr;
            }
            if (sps.addAfter != null)
            {
                retStr = retStr + sps.addAfter;
            }

            if(sps.transFunc != null 
                && sps.transFunc != ""
                && SearchParsStruct.transFuncDic.ContainsKey(sps.transFunc)
                )
            {
                retStr = SearchParsStruct.transFuncDic[sps.transFunc](retStr);
            }

            return retStr.Trim();
        }

        static List<string> illegalChrs = new List<string>{
            "&nbsp;",
            "amp;",
            "\r\n",
            "\n\r",
            "\r",
            "\n",
            "#",
            "%",
            "&",
            "{",
            "}",
            "\\",
            "<",
            ">",
            "*",
            "?",
            "/",
            "$",
            "!",
            "\"",
            ":",
            ";",
            "@",
            "+",
            "`",
            "|",
            "=",
            ".",
            " " };

        static public string washPathStr(string pathStr)
        {
            string retStr = pathStr;

            foreach (string illegalChr in illegalChrs)
            {
                retStr = retStr.Replace(illegalChr, "");
            }

            return retStr;
        }

    }
    public class SearchParsStruct
    {
        public SearchParsStruct(bool searchList = false)
        {
            this.searchList = searchList;
        }

        public string start;
        public string end;
        public bool searchList;
        public Dictionary<string, List<SearchParsStruct>> recordDef;

        //Additional processing
        public string addBefore;
        public string addAfter;
        public bool UrlDecode = true;
        public bool HtmlDecode = true;
        public bool Unescape = true;
        public string frontSplitter; //Get the part before first occurrence of this string
        public string backSplitter;  //Get the part after last occurrence of this string

        public string constant; //If this variable is not null, the other variables are meaningless.
        
        public string transFunc;

        static public Dictionary<string, stringTrans> transFuncDic
            = new Dictionary<string, stringTrans>();

        static public SearchParsStruct fromJson(JsonObject spsJo)
        {
            SearchParsStruct retSps = new SearchParsStruct();

            if (spsJo[PageParserConfig.cnStart] != null) {
                retSps.start = spsJo[PageParserConfig.cnStart].GetValue<string>();
            }
            else
            {
                retSps.start = "";
            }
            if (spsJo[PageParserConfig.cnEnd] != null)
            {
                retSps.end = spsJo[PageParserConfig.cnEnd].GetValue<string>();
            }
            else
            {
                retSps.end = "";
            }
            if (spsJo[PageParserConfig.cnSearchList] != null) {
                retSps.searchList = spsJo[PageParserConfig.cnSearchList].GetValue<bool>();
            }
            if (spsJo[PageParserConfig.cnRecordDef] != null)
            {
                retSps.recordDef = new Dictionary<string, List<SearchParsStruct>>();
                JsonObject rdJo = spsJo[PageParserConfig.cnRecordDef].AsObject();
                foreach (var property in rdJo)
                {
                    JsonArray spJa = property.Value.AsArray();
                    List<SearchParsStruct> spList = new List<SearchParsStruct>();
                    foreach (JsonObject spJo in spJa)
                    {
                        spList.Add(fromJson(spJo));
                    }
                    retSps.recordDef.Add(property.Key, spList);
                }
            }
            if (spsJo[PageParserConfig.cnAddBefore] != null) {
                retSps.addBefore = spsJo[PageParserConfig.cnAddBefore].GetValue<string>();
            }
            if (spsJo[PageParserConfig.cnAddAfter] != null) {
                retSps.addAfter = spsJo[PageParserConfig.cnAddAfter].GetValue<string>();
            }
            if (spsJo[PageParserConfig.cnUrlDecode] != null) {
                retSps.UrlDecode = spsJo[PageParserConfig.cnUrlDecode].GetValue<bool>();
            }
            if (spsJo[PageParserConfig.cnHtmlDecode] != null) {
                retSps.HtmlDecode = spsJo[PageParserConfig.cnHtmlDecode].GetValue<bool>();
            }
            if(spsJo[PageParserConfig.cnUnescape] != null) {
                retSps.Unescape = spsJo[PageParserConfig.cnUnescape].GetValue<bool>();
            }
            if (spsJo[PageParserConfig.cnFrontSplitter] != null) {
                retSps.frontSplitter = spsJo[PageParserConfig.cnFrontSplitter].GetValue<string>();
            }
            if (spsJo[PageParserConfig.cnBackSplitter] != null) {
                retSps.backSplitter = spsJo[PageParserConfig.cnBackSplitter].GetValue<string>();
            }
            if (spsJo[PageParserConfig.cnConstant] != null) {
                retSps.constant = spsJo[PageParserConfig.cnConstant].GetValue<string>();
            }
            if (spsJo[PageParserConfig.cnTransFunc] != null)
            {
                retSps.transFunc = spsJo[PageParserConfig.cnTransFunc].GetValue<string>();
            }
            return retSps;
        }

        static public JsonObject toJson(SearchParsStruct sps)
        {
            JsonObject retJo = new JsonObject();

            if(sps.start != null && sps.start != "")
            {
                retJo[PageParserConfig.cnStart] =sps.start;
            }
            if (sps.end != null && sps.end != "")
            {
                retJo[PageParserConfig.cnEnd] = sps.end;
            }
            if (sps.searchList)
            {
                retJo[PageParserConfig.cnSearchList] = sps.searchList;
            }
            if (sps.recordDef != null)
            {
                JsonObject rdJo = new JsonObject();
                foreach (string key in sps.recordDef.Keys)
                {
                    JsonArray spJa = new JsonArray();
                    foreach (SearchParsStruct sp in sps.recordDef[key])
                    {
                        spJa.Add(toJson(sp));
                    }
                    rdJo[key] = spJa;
                }
                retJo[PageParserConfig.cnRecordDef] = rdJo;
            }
            if (sps.addBefore != null && sps.addBefore != "")
            {
                retJo[PageParserConfig.cnAddBefore] = sps.addBefore;
            }
            if (sps.addAfter != null && sps.addAfter != "")
            {
                retJo[PageParserConfig.cnAddAfter] = sps.addAfter;
            }
            if (sps.UrlDecode)
            {
                retJo[PageParserConfig.cnUrlDecode] = sps.UrlDecode;
            }
            if (sps.HtmlDecode)
            {
                retJo[PageParserConfig.cnHtmlDecode] = sps.HtmlDecode;
            }
            if(sps.Unescape)
            {
                retJo[PageParserConfig.cnUnescape] = sps.Unescape;
            }
            if (sps.frontSplitter != null && sps.frontSplitter != "")
            {
                retJo[PageParserConfig.cnFrontSplitter] = sps.frontSplitter;
            }
            if (sps.backSplitter != null && sps.backSplitter != "")
            {
                retJo[PageParserConfig.cnBackSplitter] = sps.backSplitter;
            }
            if (sps.constant != null && sps.constant != "")
            {
                retJo[PageParserConfig.cnConstant] = sps.constant;
            }
            if (sps.transFunc != null && sps.transFunc != "")
            {
                retJo[PageParserConfig.cnTransFunc] = sps.transFunc;
            }
            return retJo;
        }
    }
    public delegate string stringTrans(string input);
}
