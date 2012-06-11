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
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Xml;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// The SQL Azure server group related part of the external API
    /// </summary>
    public partial interface ISqlAzureManagement
    {
        /// <summary>
        /// Retrieves a list of all the SqlAzure servers that belong to a subscription.        
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/servers")]
        IAsyncResult BeginGetServers(string subscriptionId, AsyncCallback callback, object state);

        SqlAzureServerList EndGetServers(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers")]
        IAsyncResult BeginNewServer(string subscriptionId, NewSqlAzureServerInput input, AsyncCallback callback, object state);

        XmlElement EndNewServer(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes a SQL Azure server that belongs to a subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/servers/{serverName}")]
        IAsyncResult BeginRemoveServer(string subscriptionId, string serverName, AsyncCallback callback, object state);

        SqlAzureServerList EndRemoveServer(IAsyncResult asyncResult);

        /// <summary>
        /// Sets the SQL Azure administrator password that belongs to the corresponding server.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/servers/{serverName}?op=ResetPassword", BodyStyle = WebMessageBodyStyle.Bare)]
        IAsyncResult BeginSetPassword(string subscriptionId, string serverName, XmlElement password, AsyncCallback callback, object state);

        SqlAzureServerList EndSetPassword(IAsyncResult asyncResult);

        /// <summary>
        /// Retrieves a list of all the firewall rules for a SQL Azure server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/gg715278.aspx)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules")]
        IAsyncResult BeginGetServerFirewallRules(string subscriptionId, string serverName, AsyncCallback callback, object state);

        SqlAzureFirewallRulesList EndGetServerFirewallRules(IAsyncResult asyncResult);

        /// <summary>
        /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
        /// (see http://msdn.microsoft.com/en-us/library/gg715280.aspx)
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", UriTemplate = @"{subscriptionId}/servers/{serverName}/firewallrules/{ruleName}")]
        IAsyncResult BeginNewServerFirewallRule(string subscriptionId, string serverName, string ruleName, NewSqlAzureFirewallRuleInput input, AsyncCallback callback, object state);
        
        SqlAzureFirewallRulesList EndNewServerFirewallRule(IAsyncResult asyncResult);

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
        
        SqlAzureFirewallRulesList EndRemoveServerFirewallRule(IAsyncResult asyncResult);
    }
}