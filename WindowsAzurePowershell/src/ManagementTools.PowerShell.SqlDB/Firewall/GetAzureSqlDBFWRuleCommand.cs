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

namespace Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Firewall
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Model;

    /// <summary>
    /// Retrieves a list of all the firewall rules for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSqlDBFWRule")]
    public class GetAzureSqlDBFWRuleCommand : SqlDBManagementCmdletBase
    {
        public GetAzureSqlDBFWRuleCommand()
        {
        }

        public GetAzureSqlDBFWRuleCommand(ISqlAzureManagement channel)
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

        public IEnumerable<SqlDBFirewallRuleContext> GetSqlAzureFirewallRulesProcess()
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    var firewallRules = this.RetryCall(s => this.Channel.GetServerFirewallRules(s, this.ServerName));
                    Operation operation = WaitForSqlAzureOperation();
                    return firewallRules
                                .Select(p => new SqlDBFirewallRuleContext()
                                {
                                    OperationId = operation.OperationTrackingId,
                                    OperationDescription = CommandRuntime.ToString(),
                                    OperationStatus = operation.Status,
                                    ServerName = this.ServerName,
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

                var rules = this.GetSqlAzureFirewallRulesProcess();
                
                if (rules != null)
                {
                    WriteObject(rules, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
