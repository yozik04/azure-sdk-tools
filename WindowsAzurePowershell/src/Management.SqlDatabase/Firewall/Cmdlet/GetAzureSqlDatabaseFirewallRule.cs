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
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;

    /// <summary>
    /// Retrieves a list of all the firewall rules for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSqlDatabaseFirewallRule")]
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

        internal IEnumerable<SqlDatabaseFirewallRuleContext> GetAzureSqlDatabaseFirewallRuleProcess(string serverName)
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    var firewallRules = this.RetryCall(s => this.Channel.GetServerFirewallRules(s, this.ServerName));
                    Operation operation = WaitForSqlAzureOperation();
                    return firewallRules
                                .Select(p => new SqlDatabaseFirewallRuleContext()
                                {
                                    OperationId = operation.OperationTrackingId,
                                    OperationDescription = CommandRuntime.ToString(),
                                    OperationStatus = operation.Status,
                                    ServerName = serverName,
                                    RuleName = p.Name,
                                    StartIpAddress = p.StartIpAddress,
                                    EndIpAddress = p.EndIpAddress
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
            }

            return null;
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var rules = this.GetAzureSqlDatabaseFirewallRuleProcess(this.ServerName);

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
