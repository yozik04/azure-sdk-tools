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
    using System.Xml;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Model;

    /// <summary>
    /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureSqlDBServer")]
    public class NewAzureSqlDBServerCommand : SqlDBManagementCmdletBase
    {
        public NewAzureSqlDBServerCommand()
        {
        }

        public NewAzureSqlDBServerCommand(ISqlAzureManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "SQL Azure server name.")]
        [ValidateNotNullOrEmpty]
        public string AdministratorLogin
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "SQL Azure Administrator login password.")]
        [ValidateNotNullOrEmpty]
        public string AdministratorLoginPassword
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "SQL Azure server location.")]
        [ValidateNotNullOrEmpty]
        public string Location
        {
            get;
            set;
        }      

        public SqlDBOperationContext NewSqlAzureServerProcess()
        {
            XmlElement serverName = null;

            using (new OperationContextScope((IContextChannel)Channel))
            {                
                try
                {
                   serverName = this.RetryCall(s => this.Channel.NewServer(s, this.AdministratorLogin, this.AdministratorLoginPassword, this.Location));
                   Operation operation = WaitForSqlAzureOperation();
                   return new SqlDBOperationContext()
                   {
                       ServerName = serverName.InnerText,
                       OperationStatus = operation.Status,
                       OperationDescription = CommandRuntime.ToString(),
                       OperationId = operation.OperationTrackingId
                   };
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
                var context = this.NewSqlAzureServerProcess();

                if (context != null)
                {
                    WriteObject(context, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }       
    }
}
