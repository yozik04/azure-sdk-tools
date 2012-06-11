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
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDatabase;
    using Microsoft.WindowsAzure.Management.Cmdlets.Common;

    public class SqlDatabaseManagementCmdletBase : CmdletBase<ISqlAzureManagement>
    {
        // SQL Azure doesn't support async 
        protected static Operation WaitForSqlAzureOperation()
        {
            string operationId = RetrieveOperationId();
            Operation operation = new Operation();
            operation.OperationTrackingId = operationId;
            operation.Status = "Success";
            return operation;
        }

        protected override ISqlAzureManagement CreateChannel()
        {
            if (this.ServiceBinding == null)
            {
                this.ServiceBinding = Management.SqlDB.ConfigurationConstants.WebHttpBinding();
            }

            if (string.IsNullOrEmpty(CurrentSubscription.SqlAzureServiceEndpoint))
            {
                this.ServiceEndpoint = Management.SqlDB.ConfigurationConstants.SqlAzureManagementEndpoint;
            }
            else
            {
                this.ServiceEndpoint = CurrentSubscription.SqlAzureServiceEndpoint;
            }

            return SqlAzureManagementHelper.CreateSqlAzureManagementChannel(this.ServiceBinding, new Uri(this.ServiceEndpoint), CurrentSubscription.Certificate);
        }

        protected override void WriteErrorDetails(System.ServiceModel.CommunicationException exception)
        {
            SqlAzureManagementError error = null;
            SqlAzureManagementHelper.TryGetExceptionDetails(exception, out error);

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