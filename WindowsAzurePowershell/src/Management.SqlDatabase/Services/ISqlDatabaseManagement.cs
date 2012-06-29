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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Services
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Xml;

    /// <summary>
    /// The Windows Azure SQL Database related part of the external API
    /// </summary>
    public partial interface ISqlDatabaseManagement
    {
        /// <summary>
        /// Enumerates SQL Database servers that are provisioned for a subscription.  
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715269)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/servers")]
        IAsyncResult BeginGetServers(string subscriptionId, AsyncCallback callback, object state);

        SqlDatabaseServerList EndGetServers(IAsyncResult asyncResult);

        /// <summary>
        /// Adds a new SQL Database server to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715274)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers")]
        IAsyncResult BeginNewServer(string subscriptionId, NewSqlDatabaseServerInput input, AsyncCallback callback, object state);

        XmlElement EndNewServer(IAsyncResult asyncResult);

        /// <summary>
        /// Drops a SQL Database server from a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715285)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/servers/{serverName}")]
        IAsyncResult BeginRemoveServer(string subscriptionId, string serverName, AsyncCallback callback, object state);

        void EndRemoveServer(IAsyncResult asyncResult);

        /// <summary>
        /// Sets the administrative password of a SQL Database server for a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715272)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers/{serverName}?op=ResetPassword", BodyStyle = WebMessageBodyStyle.Bare)]
        IAsyncResult BeginSetPassword(string subscriptionId, string serverName, XmlElement password, AsyncCallback callback, object state);

        SqlDatabaseServerList EndSetPassword(IAsyncResult asyncResult);

        /// <summary>
        /// Retrieves a list of all the firewall rules for a SQL Database server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715278)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules")]
        IAsyncResult BeginGetServerFirewallRules(string subscriptionId, string serverName, AsyncCallback callback, object state);

        SqlDatabaseFirewallRulesList EndGetServerFirewallRules(IAsyncResult asyncResult);

        /// <summary>
        /// Updates an existing firewall rule or adds a new firewall rule for a SQL Database server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715280)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginNewServerFirewallRule(string subscriptionId, string serverName, string ruleName, NewSqlDatabaseFirewallRuleInput input, AsyncCallback callback, object state);

        SqlDatabaseFirewallRulesList EndNewServerFirewallRule(IAsyncResult asyncResult);

        /// <summary>
        /// Adds a new firewall rule or updates an existing firewall rule for a SQL Database server with requester’s IP address.
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/hh239605)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}?op=AutoDetectClientIP")]
        IAsyncResult BeginNewServerFirewallRuleWithIpDetect(string subscriptionId, string serverName, string ruleName, AsyncCallback callback, object state);

        XmlElement EndNewServerFirewallRuleWithIpDetect(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes a firewall rule from a SQL Database server that belongs to a subscription
        /// (see http://msdn.microsoft.com/en-us/library/windowsazure/gg715277)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginRemoveServerFirewallRule(string subscriptionId, string serverName, string ruleName, AsyncCallback callback, object state);

        SqlDatabaseFirewallRulesList EndRemoveServerFirewallRule(IAsyncResult asyncResult);
    }
}
