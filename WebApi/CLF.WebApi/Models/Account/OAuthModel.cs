using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CLF.WebApi.Models.Account
{
    public class OAuthModel
    {
        public string client_id { get; set; }

        public string client_type { get; set; }
        public bool grant_type { get; set; }
    }

    public enum GrantType
    {
        [Description("client_credentials")]
        ClientCredentials=0
    }
}
