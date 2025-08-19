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
        private string encoding = "utf-8";
        public string Encoding { get => encoding; set => encoding = value; }

        private Dictionary<string, object> keyValuePairs 
            = new Dictionary<string, object>();
    }
}
