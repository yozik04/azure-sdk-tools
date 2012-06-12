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
    /// The SQL Azure server group related part of the external API
    /// </summary>
    public partial interface ISqlDatabaseManagement
    {
        /// <summary>
        /// Retrieves a list of all the SqlAzure servers that belong to a subscription.        
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/servers")]
        IAsyncResult BeginGetServers(string subscriptionId, AsyncCallback callback, object state);

        SqlDatabaseServerList EndGetServers(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers")]
        IAsyncResult BeginNewServer(string subscriptionId, NewSqlDatabaseServerInput input, AsyncCallback callback, object state);

        XmlElement EndNewServer(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes a SQL Azure server that belongs to a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/servers/{serverName}")]
        IAsyncResult BeginRemoveServer(string subscriptionId, string serverName, AsyncCallback callback, object state);

        SqlDatabaseServerList EndRemoveServer(IAsyncResult asyncResult);

        /// <summary>
        /// Sets the SQL Azure administrator password that belongs to the corresponding server.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers/{serverName}?op=ResetPassword", BodyStyle = WebMessageBodyStyle.Bare)]
        IAsyncResult BeginSetPassword(string subscriptionId, string serverName, XmlElement password, AsyncCallback callback, object state);

        SqlDatabaseServerList EndSetPassword(IAsyncResult asyncResult);

        /// <summary>
        /// Retrieves a list of all the firewall rules for a SQL Azure server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/gg715278.aspx)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules")]
        IAsyncResult BeginGetServerFirewallRules(string subscriptionId, string serverName, AsyncCallback callback, object state);

        SqlDatabaseFirewallRulesList EndGetServerFirewallRules(IAsyncResult asyncResult);

        /// <summary>
        /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/gg715280.aspx)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginNewServerFirewallRule(string subscriptionId, string serverName, string ruleName, NewSqlDatabaseFirewallRuleInput input, AsyncCallback callback, object state);
        
        SqlDatabaseFirewallRulesList EndNewServerFirewallRule(IAsyncResult asyncResult);

        /// <summary>
        /// Adds a new firewall rule or updates an existing firewall rule for a SQL Azure server with the requester’s IP address. 
        /// (see http://msdn.microsoft.com/en-us/library/hh239605.aspx)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}?op=AutoDetectClientIP")]
        IAsyncResult BeginNewServerFirewallRuleWithIpDetect(string subscriptionId, string serverName, string ruleName, AsyncCallback callback, object state);
        
        XmlElement EndNewServerFirewallRuleWithIpDetect(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes a firewall rule from a SQL Azure server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/gg715277.aspx)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginRemoveServerFirewallRule(string subscriptionId, string serverName, string ruleName, AsyncCallback callback, object state);
        
        SqlDatabaseFirewallRulesList EndRemoveServerFirewallRule(IAsyncResult asyncResult);
    }
}