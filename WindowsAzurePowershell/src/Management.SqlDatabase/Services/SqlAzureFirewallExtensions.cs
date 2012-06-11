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
    public static partial class SqlAzureManagementExtensionMethods
    {
        public static SqlAzureFirewallRulesList GetServerFirewallRules(this ISqlAzureManagement proxy, string subscriptionId, string serverName)
        {
            return proxy.EndGetServerFirewallRules(proxy.BeginGetServerFirewallRules(subscriptionId, serverName, null, null));
        }

        public static void NewServerFirewallRule(this ISqlAzureManagement proxy, string subscriptionId, string serverName, string ruleName, string startIpAddress, string endIpAddress)
        {
            var input = new NewSqlAzureFirewallRuleInput
            {
                StartIpAddress = startIpAddress,
                EndIpAddress = endIpAddress
            };

            proxy.EndNewServerFirewallRule(proxy.BeginNewServerFirewallRule(subscriptionId, serverName, ruleName, input, null, null));
        }

        public static string NewServerFirewallRuleWithIpDetect(this ISqlAzureManagement proxy, string subscriptionId, string serverName, string ruleName)
        {
            var result = proxy.EndNewServerFirewallRuleWithIpDetect(proxy.BeginNewServerFirewallRuleWithIpDetect(subscriptionId, serverName, ruleName, null, null));
            return result.InnerText;
        }

        public static void RemoveServerFirewallRule(this ISqlAzureManagement proxy, string subscriptionId, string serverName, string ruleName)
        {
            proxy.EndRemoveServerFirewallRule(proxy.BeginRemoveServerFirewallRule(subscriptionId, serverName, ruleName, null, null));
        }
    }
}
