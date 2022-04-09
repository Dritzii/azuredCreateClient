using System;
using System.Collections.Generic;
using System.Text;

namespace azuredCreateClient.Middleware
{
    class FirewallClass
    {
        public string subscriptionId { get; set; }
        public string tenantId { get; set; }
        public string displayName { get; set; }
        public decimal name { get; set; }
        public string uri { get; set; }

        public static explicit operator string(FirewallClass v)
        {
            throw new NotImplementedException();
        }
    }
}
