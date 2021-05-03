using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel
{
    public class AzureCredentials
    {
        public string S2T_subscription { get; set; }
        public string S2T_endpoint { get; set; }
        public string LUIS_subscription { get; set; }
        public string LUIS_appid { get; set; }
    }
}
