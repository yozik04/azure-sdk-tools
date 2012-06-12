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
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Xml;

    public static class SqlDatabaseManagementHelper
    {
        public static ISqlDatabaseManagement CreateSqlDatabaseManagementChannel(Binding binding, Uri remoteUri, X509Certificate2 cert)
        {
            WebChannelFactory<ISqlDatabaseManagement> factory = new WebChannelFactory<ISqlDatabaseManagement>(binding, remoteUri);
            factory.Endpoint.Behaviors.Add(new ClientOutputMessageInspector());
            factory.Credentials.ClientCertificate.Certificate = cert;
            return factory.CreateChannel();
        }

        public static bool TryGetExceptionDetails(CommunicationException exception, out SqlDatabaseManagementError errorDetails)
        {
            HttpStatusCode httpStatusCode;
            string operationId;
            return TryGetExceptionDetails(exception, out errorDetails, out httpStatusCode, out operationId);
        }

        public static bool TryGetExceptionDetails(CommunicationException exception, out SqlDatabaseManagementError errorDetails, out HttpStatusCode httpStatusCode, out string operationId)
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
                errorDetails = new SqlDatabaseManagementError();
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
                    DataContractSerializer ser = new DataContractSerializer(typeof(SqlDatabaseManagementError));
                    errorDetails = (SqlDatabaseManagementError)ser.ReadObject(reader, true);
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
