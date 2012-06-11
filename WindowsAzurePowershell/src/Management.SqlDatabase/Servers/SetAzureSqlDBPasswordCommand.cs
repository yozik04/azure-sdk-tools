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

namespace Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Servers
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.Common.Services.Helpers;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Model;

    [Cmdlet(VerbsCommon.Set, "AzureSqlDBPassword")]
    public class SetAzureSqlDBPasswordCommand : SqlDBManagementCmdletBase
    {
        public SetAzureSqlDBPasswordCommand()
        {
        }

        public SetAzureSqlDBPasswordCommand(ISqlAzureManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string ServerName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string NewPassword
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                this.SetSqlAzurePasswordProcess();
            }           
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(this.ProcessExceptionDetails(ex), string.Empty, ErrorCategory.CloseError, null));
            }           
        }

        private void SetSqlAzurePasswordProcess()
        {
            try
            {
                using (new OperationContextScope((IContextChannel)Channel))
                {
                    this.RetryCall(s => this.Channel.SetPassword(s, this.ServerName, this.NewPassword));
                    Operation operation = WaitForSqlAzureOperation();
                    SqlDBOperationContext context = new SqlDBOperationContext()
                    {
                        OperationDescription = CommandRuntime.ToString(),
                        OperationId = operation.OperationTrackingId,
                        OperationStatus = operation.Status
                    };
                    WriteObject(context, true);
                }  
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);                
            }
        }
    }
}
