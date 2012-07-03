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
    using System.Diagnostics;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    /// <summary>
    /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureSqlDatabaseServerFirewallRule", DefaultParameterSetName = "IpRange", ConfirmImpact = ConfirmImpact.Low)]
    public class NewAzureSqlDatabaseServerFirewallRule : SqlDatabaseManagementCmdletBase
    {
        public NewAzureSqlDatabaseServerFirewallRule()
        {
        }

        public NewAzureSqlDatabaseServerFirewallRule(ISqlDatabaseManagement channel)
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

        internal SqlDatabaseServerFirewallRuleContext NewAzureSqlDatabaseServerFirewallRuleProcess(string paramterSetName, string serverName, string ruleName, string startIpAddress, string endIpAddress)
        {
            SqlDatabaseServerFirewallRuleContext operationContext = null;

            try
            {
                switch (paramterSetName)
                {
                    case "IpRange":
                        InvokeInOperationContext(() =>
                        {
                            RetryCall(subscription =>
                                Channel.NewServerFirewallRule(subscription, serverName, ruleName, startIpAddress, endIpAddress));
                            WAPPSCmdlet.Operation operation = WaitForSqlDatabaseOperation();

                            operationContext = new SqlDatabaseServerFirewallRuleContext()
                            {
                                OperationDescription = CommandRuntime.ToString(),
                                OperationStatus = operation.Status,
                                OperationId = operation.OperationTrackingId,
                                ServerName = serverName,
                                RuleName = ruleName,
                                StartIpAddress = startIpAddress,
                                EndIpAddress = endIpAddress
                            };
                        });
                        break;
                    case "IpDetection":
                        InvokeInOperationContext(() =>
                        {
                            var detectedIpAddress = RetryCall(subscription =>
                                Channel.NewServerFirewallRuleWithIpDetect(subscription, serverName, ruleName));
                            WAPPSCmdlet.Operation operation = WaitForSqlDatabaseOperation();

                            operationContext = new SqlDatabaseServerFirewallRuleContext()
                            {
                                OperationDescription = CommandRuntime.ToString(),
                                OperationStatus = operation.Status,
                                OperationId = operation.OperationTrackingId,
                                ServerName = serverName,
                                RuleName = ruleName,
                                StartIpAddress = detectedIpAddress,
                                EndIpAddress = detectedIpAddress
                            };
                        });
                        break;
                    default:
                        // Should never get here
                        Debug.Assert(false, "Invalid parameter set!");
                        break;
                }
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return operationContext;
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                SqlDatabaseServerOperationContext context = this.NewAzureSqlDatabaseServerFirewallRuleProcess(this.ParameterSetName, this.ServerName, this.RuleName, this.StartIpAddress, this.EndIpAddress);

                if (context != null)
                {
                    WriteObject(context, true);
                }
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.WriteError, null));
            }
        }
    }
}
