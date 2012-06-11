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
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Xml;

    public static class SqlAzureManagementHelper
    {
        public static ISqlAzureManagement CreateSqlAzureManagementChannel(Binding binding, Uri remoteUri, X509Certificate2 cert)
        {
            WebChannelFactory<ISqlAzureManagement> factory = new WebChannelFactory<ISqlAzureManagement>(binding, remoteUri);
            factory.Endpoint.Behaviors.Add(new ClientOutputMessageInspector());
            factory.Credentials.ClientCertificate.Certificate = cert;
            return factory.CreateChannel();
        }

        public static bool TryGetExceptionDetails(CommunicationException exception, out SqlAzureManagementError errorDetails)
        {
            HttpStatusCode httpStatusCode;
            string operationId;
            return TryGetExceptionDetails(exception, out errorDetails, out httpStatusCode, out operationId);
        }

        public static bool TryGetExceptionDetails(CommunicationException exception, out SqlAzureManagementError errorDetails, out HttpStatusCode httpStatusCode, out string operationId)
        {
            errorDetails = null;
            httpStatusCode = 0;
            operationId = null;

            if (exception == null)
            {
                return false;
            }

            if (exception.Message == "Internal Server Error")
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
                return true;
            }

            WebException wex = exception.InnerException as WebException;

            if (wex == null)
            {
                return false;
            }

            HttpWebResponse response = wex.Response as HttpWebResponse;
            if (response == null)
            {
                return false;
            }

            if (response.Headers != null)
            {
                operationId = response.Headers[Constants.OperationTrackingIdHeader];
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                errorDetails = new SqlAzureManagementError();
                errorDetails.Message = response.ResponseUri.AbsoluteUri + " does not exist.";
                errorDetails.Code = response.StatusCode.ToString();
                return false;
            }

            using (var s = response.GetResponseStream())
            {
                if (s.Length == 0)
                {
                    return false;
                }

                try
                {
                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(s, new XmlDictionaryReaderQuotas());
                    DataContractSerializer ser = new DataContractSerializer(typeof(SqlAzureManagementError));
                    errorDetails = (SqlAzureManagementError)ser.ReadObject(reader, true);
                }
                catch (SerializationException)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
