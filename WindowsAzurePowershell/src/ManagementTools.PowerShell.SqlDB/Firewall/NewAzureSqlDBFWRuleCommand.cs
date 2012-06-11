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
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Model;

    /// <summary>
    /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureSqlDBFWRule", DefaultParameterSetName = "IpRange")]
    public class NewAzureSqlDBFWRuleCommand : SqlDBManagementCmdletBase
    {
        public NewAzureSqlDBFWRuleCommand()
        {
        }

        public NewAzureSqlDBFWRuleCommand(ISqlAzureManagement channel)
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
        [ValidateNotNullOrEmpty]
        public SwitchParameter UseIpAddressDetection
        {
            get;
            set;
        }

        public void NewSqlAzureFirewallRuleProcess()
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

                    Operation operation = WaitForSqlAzureOperation();
                    var context = new SqlDBFirewallRuleContext()
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
                this.NewSqlAzureFirewallRuleProcess();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.WriteError, null));
            }
        }
    }
}
