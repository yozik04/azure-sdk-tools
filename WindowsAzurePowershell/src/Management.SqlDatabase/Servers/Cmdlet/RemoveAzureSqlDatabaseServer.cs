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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Servers.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    [Cmdlet(VerbsCommon.Remove, "AzureSqlDatabaseServer", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveAzureSqlDatabaseServer : SqlDatabaseManagementCmdletBase
    {
        public RemoveAzureSqlDatabaseServer()
        {
        }

        public RemoveAzureSqlDatabaseServer(ISqlDatabaseManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the SQL Database server to delete.")]
        [ValidateNotNullOrEmpty]
        public string ServerName
        {
            get;
            set;
        }

        internal bool RemoveAzureSqlDatabaseServerProcess(string serverName)
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    RetryCall(subscription =>
                        Channel.RemoveServer(subscription, serverName));
                    WAPPSCmdlet.Operation operation = WaitForSqlAzureOperation();
                    var context = new SqlDatabaseOperationContext()
                    {
                        ServerName = serverName,
                        OperationId = operation.OperationTrackingId,
                        OperationDescription = CommandRuntime.ToString(),
                        OperationStatus = operation.Status
                    };

                    WriteObject(context, true);
                }
                catch (CommunicationException ex)
                {
                    this.WriteErrorDetails(ex);
                    return false;
                }
            }

            return true;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                this.RemoveAzureSqlDatabaseServerProcess(this.ServerName);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
