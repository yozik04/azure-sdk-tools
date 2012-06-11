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

namespace Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB
{
    using System;
    using System.Globalization;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.Common;

    public class SqlDBManagementCmdletBase : CmdletBase<ISqlAzureManagement>
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