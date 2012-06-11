// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
