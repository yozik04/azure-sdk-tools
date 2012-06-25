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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Firewall.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    /// <summary>
    /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureSqlDatabaseFirewallRule", DefaultParameterSetName = "IpRange")]
    public class NewAzureSqlDatabaseFirewallRule : SqlDatabaseManagementCmdletBase
    {
        public NewAzureSqlDatabaseFirewallRule()
        {
        }

        public NewAzureSqlDatabaseFirewallRule(ISqlDatabaseManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Database server name.")]
        [ValidateNotNullOrEmpty]
        public string ServerName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "SQL Database server firewall rule name.")]
        [ValidateNotNullOrEmpty]
        public string RuleName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "Start of the IP Range.", ParameterSetName = "IpRange")]
        [ValidateNotNullOrEmpty]
        public string StartIpAddress
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "End of the IP Range.", ParameterSetName = "IpRange")]
        [ValidateNotNullOrEmpty]
        public string EndIpAddress
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "Use the requester’s IP address.", ParameterSetName = "IpDetection")]
        public SwitchParameter UseIpAddressDetection
        {
            get;
            set;
        }

        internal void NewAzureSqlDatabaseFirewallRuleProcess()
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    if (this.ParameterSetName == "IpRange")
                    {
                        this.RetryCall(s => this.Channel.NewServerFirewallRule(s, this.ServerName, this.RuleName, this.StartIpAddress, this.EndIpAddress));
                    }
                    else if (this.ParameterSetName == "IpDetection")
                    {
                        var address = this.RetryCall(s => this.Channel.NewServerFirewallRuleWithIpDetect(s, this.ServerName, this.RuleName));
                        this.StartIpAddress = address;
                        this.EndIpAddress = address;
                    }

                    WAPPSCmdlet.Operation operation = WaitForSqlAzureOperation();
                    var context = new SqlDatabaseFirewallRuleContext()
                    {
                        ServerName = this.ServerName,
                        OperationId = operation.OperationTrackingId,
                        OperationDescription = CommandRuntime.ToString(),
                        OperationStatus = operation.Status,
                        RuleName = this.RuleName,
                        StartIpAddress = this.StartIpAddress,
                        EndIpAddress = this.EndIpAddress
                    };

                    WriteObject(context, true);
                }
                catch (CommunicationException ex)
                {
                    this.WriteErrorDetails(ex);
                }
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
                this.NewAzureSqlDatabaseFirewallRuleProcess();
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.WriteError, null));
            }
        }
    }
}
