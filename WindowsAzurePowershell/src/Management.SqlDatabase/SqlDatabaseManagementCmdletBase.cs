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

namespace Microsoft.WindowsAzure.Management.SqlDatabase
{
    using System;
    using System.Globalization;
    using System.Management.Automation;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.WindowsAzure.Management.CloudService.Services;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    public class SqlDatabaseManagementCmdletBase : CloudCmdlet<ISqlDatabaseManagement>
    {
        /// <summary>
        /// Gets or sets a flag indicating whether CreateChannel should share
        /// the command's current Channel when asking for a new one.  This is
        /// only used for testing.
        /// </summary>
        internal bool ShareChannel { get; set; }

        protected override ISqlDatabaseManagement CreateChannel()
        {
            // If ShareChannel is set by a unit test, use the same channel that
            // was passed into out constructor.  This allows the test to submit
            // a mock that we use for all network calls.
            if (ShareChannel)
            {
                return Channel;
            }

            if (this.ServiceBinding == null)
            {
                this.ServiceBinding = ConfigurationConstants.WebHttpBinding(this.MaxStringContentLength);
            }

            if (string.IsNullOrEmpty(CurrentSubscription.ServiceEndpoint))
            {
                this.ServiceEndpoint = ConfigurationConstants.ServiceManagementEndpoint;
            }
            else
            {
                this.ServiceEndpoint = CurrentSubscription.ServiceEndpoint;
            }

            return SqlDatabaseManagementHelper.CreateSqlDatabaseManagementChannel(this.ServiceBinding, new Uri(this.ServiceEndpoint), CurrentSubscription.Certificate);
        }

        // Windows Azure SQL Database doesn't support async calls
        protected static WAPPSCmdlet.Operation WaitForSqlDatabaseOperation()
        {
            string operationId = RetrieveOperationId();
            WAPPSCmdlet.Operation operation = new WAPPSCmdlet.Operation();
            operation.OperationTrackingId = operationId;
            operation.Status = "Success";
            return operation;
        }
        
        protected override void WriteErrorDetails(System.ServiceModel.CommunicationException exception)
        {
            SqlDatabaseManagementError error = null;
            SqlDatabaseManagementHelper.TryGetExceptionDetails(exception, out error);

            if (error == null)
            {
                WriteError(new ErrorRecord(exception, string.Empty, ErrorCategory.CloseError, null));
            }
            else
            {
                string errorDetails = string.Format(
                    CultureInfo.InvariantCulture,
                    "HTTP Status Code: {0} - HTTP Error Message: {1}",
                    error.Code,
                    error.Message);

                WriteError(new ErrorRecord(new CommunicationException(errorDetails), string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}