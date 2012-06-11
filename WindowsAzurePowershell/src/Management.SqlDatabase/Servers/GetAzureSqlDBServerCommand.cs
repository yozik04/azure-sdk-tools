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
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure.Management.SqlDB;
    using Microsoft.WindowsAzure.ManagementTools.PowerShell.SqlDB.Model;

    /// <summary>
    /// Retrieves a list of all the SQL Azure servers that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSqlDBServer")]
    public class GetAzureSqlDBServerCommand : SqlDBManagementCmdletBase
    {
        public GetAzureSqlDBServerCommand()
        {
        }

        public GetAzureSqlDBServerCommand(ISqlAzureManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Azure server name.")]
        [ValidateNotNullOrEmpty]
        public string ServerName { get; set; }        

        public IEnumerable<SqlDBServerContext> GetSqlAzureServersProcess()
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                SqlAzureServerList servers = null;
                Operation operation = null;
                try
                {
                    servers = this.RetryCall(s => this.Channel.GetServers(s));
                    operation = WaitForSqlAzureOperation();
                    if (!string.IsNullOrEmpty(this.ServerName) && operation != null)
                    {
                        var server = servers.FirstOrDefault(s => s.Name == this.ServerName);

                        if (server != null)
                        {
                            return new List<SqlDBServerContext>
                            {
                                new SqlDBServerContext
                                {
                                    OperationId = operation.OperationTrackingId,
                                    OperationDescription = CommandRuntime.ToString(),
                                    OperationStatus = operation.Status,
                                    ServerName = server.Name,
                                    Location = server.Location,
                                    AdministratorLogin = server.AdministratorLogin
                                }
                            };
                        }
                        else
                        {
                            throw new Exception("The server was not found.");
                        }
                    }
                }
                catch (CommunicationException ex)
                {
                    if (ex is EndpointNotFoundException && IsVerbose() == false)
                    {
                        return null;
                    }

                    this.WriteErrorDetails(ex);
                }

                return servers.Select(s => new SqlDBServerContext
                    {
                        ServerName = s.Name,
                        Location = s.Location,
                        AdministratorLogin = s.AdministratorLogin,
                        OperationDescription = CommandRuntime.ToString(),
                        OperationStatus = operation.Status,
                        OperationId = operation.OperationTrackingId
                    });
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
                var servers = this.GetSqlAzureServersProcess();

                if (servers != null)
                {
                    WriteObject(servers, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}