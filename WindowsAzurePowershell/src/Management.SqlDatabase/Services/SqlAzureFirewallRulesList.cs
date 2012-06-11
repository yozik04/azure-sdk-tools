// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.SqlDB
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// List of firewall rules.
    /// </summary>
    [CollectionDataContract(Name = "FirewallRules", ItemName = "FirewallRule", Namespace = Constants.SqlAzureManagementNS)]
    public class SqlAzureFirewallRulesList : List<SqlAzureFirewallRule>
    {
        public SqlAzureFirewallRulesList()
        {
        }

        public SqlAzureFirewallRulesList(IEnumerable<SqlAzureFirewallRule> firewallRules)
            : base(firewallRules)
        {
        }
    }
}
