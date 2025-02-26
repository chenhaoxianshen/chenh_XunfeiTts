using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chenhXunfeiTts
{
    public class AikitBuildModel
    {
        public string appID { get; set; }
        public  string apiKey { get; set; }
        public  string apiSecret { get; set; }
         
        public  string ability { get; set; }
        public  int authType { get; set; }= 0;
        public  string resDir { get; set; }
        public  string workDir { get; set; }
        public  string text { get; set; }
        public  string fileName { get; set; }
        public  string vcn { get; set; } = "xiaoyan";
    }
}
