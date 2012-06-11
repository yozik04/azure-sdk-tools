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
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public static class ConfigurationConstants
    {
        public const string SqlAzureManagementEndpoint = "https://management.database.windows.net:8443";

        public static Binding WebHttpBinding()
        {
            const int MAX = 5000000;

            var binding = new WebHttpBinding(WebHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.ReaderQuotas.MaxStringContentLength = 67108864;
            binding.MaxBufferSize = MAX;
            binding.MaxReceivedMessageSize = MAX;
            binding.ReaderQuotas.MaxArrayLength = MAX;

            return binding;
        }
    }
}