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
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    /// <summary>
    /// Retrieves a list of all the firewall rules for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSqlDatabaseFirewallRule", ConfirmImpact = ConfirmImpact.None)]
    public class GetAzureSqlDatabaseFirewallRule : SqlDatabaseManagementCmdletBase
    {
        public GetAzureSqlDatabaseFirewallRule()
        {
        }

        public GetAzureSqlDatabaseFirewallRule(ISqlDatabaseManagement channel)
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

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Database server firewall rule name.")]
        [ValidateNotNullOrEmpty]
        public string RuleName { get; set; }

        internal IEnumerable<SqlDatabaseFirewallRuleContext> GetAzureSqlDatabaseFirewallRuleProcess(string serverName, string ruleName)
        {
            IEnumerable<SqlDatabaseFirewallRuleContext> processResult = null;

            try
            {
                InvokeInOperationContext(() =>
                {
                    SqlDatabaseFirewallRulesList firewallRules = RetryCall(subscription => 
                        Channel.GetServerFirewallRules(subscription, this.ServerName));
                    WAPPSCmdlet.Operation operation = WaitForSqlDatabaseOperation();

                    if (string.IsNullOrEmpty(ruleName))
                    {
                        // Firewall rule name is not specified, select all 
                        processResult = firewallRules.Select(p => new SqlDatabaseFirewallRuleContext()
                        {
                            OperationDescription = CommandRuntime.ToString(),
                            OperationId = operation.OperationTrackingId,
                            OperationStatus = operation.Status,
                            ServerName = serverName,
                            RuleName = p.Name,
                            StartIpAddress = p.StartIpAddress,
                            EndIpAddress = p.EndIpAddress
                        });
                    }
                    else
                    {
                        var firewallRule = firewallRules.FirstOrDefault(p => p.Name == ruleName);
                        if (firewallRule != null)
                        {
                            processResult = new List<SqlDatabaseFirewallRuleContext>
                            {
                                new SqlDatabaseFirewallRuleContext
                                {
                                    OperationDescription = CommandRuntime.ToString(),
                                    OperationId = operation.OperationTrackingId,
                                    OperationStatus = operation.Status,
                                    ServerName = serverName,
                                    RuleName = firewallRule.Name,
                                    StartIpAddress = firewallRule.StartIpAddress,
                                    EndIpAddress = firewallRule.EndIpAddress
                                }
                            };
                        }
                        else
                        {
                            throw new Exception("The firewall rule was not found.");
                        }
                    }

                });
            }
            catch (CommunicationException ex)
            {
                if (ex is EndpointNotFoundException && IsVerbose() == false)
                {
                    return null;
                }

                this.WriteErrorDetails(ex);
            }

            return processResult;
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var rules = this.GetAzureSqlDatabaseFirewallRuleProcess(this.ServerName, this.RuleName);

                if (rules != null)
                {
                    WriteObject(rules, true);
                }
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
