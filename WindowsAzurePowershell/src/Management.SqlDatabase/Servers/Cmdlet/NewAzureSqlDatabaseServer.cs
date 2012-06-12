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
    using System.Xml;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;

    /// <summary>
    /// Updates an existing firewall rule or adds a new firewall rule for a SQL Azure server that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureSqlDatabaseServer")]
    public class NewAzureSqlDatabaseServer : SqlDatabaseManagementCmdletBase
    {
        public NewAzureSqlDatabaseServer()
        {
        }

        public NewAzureSqlDatabaseServer(ISqlDatabaseManagement channel)
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

        public SqlDatabaseOperationContext NewAzureSqlDatabaseServerProcess()
        {
            XmlElement serverName = null;

            using (new OperationContextScope((IContextChannel)Channel))
            {                
                try
                {
                   serverName = this.RetryCall(s => this.Channel.NewServer(s, this.AdministratorLogin, this.AdministratorLoginPassword, this.Location));
                   Operation operation = WaitForSqlAzureOperation();
                   return new SqlDatabaseOperationContext()
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
                var context = this.NewAzureSqlDatabaseServerProcess();

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
