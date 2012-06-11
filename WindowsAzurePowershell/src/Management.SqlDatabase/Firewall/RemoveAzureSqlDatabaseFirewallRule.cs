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

namespace Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Firewall
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Model;

    /// <summary>
    /// Deletes a firewall rule from a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureSqlDBFWRule")]
    public class RemoveAzureSqlDBFWRuleCommand : SqlDBManagementCmdletBase
    {
        public RemoveAzureSqlDBFWRuleCommand()
        {
        }

        public RemoveAzureSqlDBFWRuleCommand(ISqlAzureManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true,
            ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Azure server name.")]
        [ValidateNotNullOrEmpty]
        public string ServerName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "SQL Azure server firewall rule name.")]
        [ValidateNotNullOrEmpty]
        public string RuleName
        {
            get;
            set;
        }
        
        public string RemoveSqlAzureFirewallRuleProcess()
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.RemoveServerFirewallRule(s, this.ServerName, this.RuleName));

                    Operation operation = WaitForSqlAzureOperation();
                    var context = new SqlDBOperationContext()
                    {
                        ServerName = this.ServerName,
                        OperationId = operation.OperationTrackingId,
                        OperationDescription = CommandRuntime.ToString(),
                        OperationStatus = operation.Status
                    };

                    WriteObject(context, true);
                }
                catch (CommunicationException ex)
                {
                    this.WriteErrorDetails(ex);
                }

                return null;
            }
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                this.RemoveSqlAzureFirewallRuleProcess();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
