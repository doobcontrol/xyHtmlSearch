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
                    retList.Add(
                        strForFind.Substring(retStartIndex, retEndIndex - retStartIndex)

                        );
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
                else 
                {
                    retStr = findBetween(retStr, sps.start, sps.end);
                }
            }

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
                            findAllBetween(searchStr, sps.start, sps.end);
                    }
                    else
                    {
                        searchStr = 
                            findBetween(searchStr, sps.start, sps.end);
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
                                findAllBetween(s, sps.start, sps.end));
                        }
                        retList = tempList;
                    }
                    else
                    {
                        foreach (string s in retList)
                        {
                            retList[retList.IndexOf(s)] =
                                findBetween(s, sps.start, sps.end);
                        }
                    }
                }
            }

            return retList;
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
    public struct SearchParsStruct
    {
        public SearchParsStruct(bool searchList = false)
        {
            this.searchList = searchList;
        }

        public string start;
        public string end;
        public bool searchList;
    }
}
