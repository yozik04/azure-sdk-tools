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
    using System.IO;
    using System.Xml;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    public static partial class SqlAzureManagementExtensionMethods
    {
        public static SqlAzureServerList GetServers(this ISqlAzureManagement proxy, string subscriptionId)
        {
            return proxy.EndGetServers(proxy.BeginGetServers(subscriptionId, null, null));
        }

        public static XmlElement NewServer(this ISqlAzureManagement proxy, string subscriptionId, string administratorLogin, string administratorLoginPassword, string location)
        {
            var input = new NewSqlAzureServerInput()
            {
                AdministratorLogin = administratorLogin,
                AdministratorLoginPassword = administratorLoginPassword,
                Location = location
            };

            var inputproxy = proxy.BeginNewServer(subscriptionId, input, null, null);
            var result = proxy.EndNewServer(inputproxy);
            return result;
        }

        public static void RemoveServer(this ISqlAzureManagement proxy, string subscriptionId, string serverName)
        {
            proxy.EndRemoveServer(proxy.BeginRemoveServer(subscriptionId, serverName, null, null));
        }

        public static void SetPassword(this ISqlAzureManagement proxy, string subscriptionId, string serverName, string password)
        {
            // create an xml element for the request body
            var xml = string.Empty;

            using (var tx = new StringWriter())
            {
                var tw = new XmlTextWriter(tx);
                tw.WriteStartDocument();
                tw.WriteStartElement("AdministratorLoginPassword", Constants.SqlAzureManagementNS);
                tw.WriteString(password);
                tw.WriteEndElement();

                xml = tx.ToString();
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var el = (XmlElement)doc.FirstChild.NextSibling;

            proxy.EndSetPassword(proxy.BeginSetPassword(subscriptionId, serverName, el, null, null));
        }
    }
}