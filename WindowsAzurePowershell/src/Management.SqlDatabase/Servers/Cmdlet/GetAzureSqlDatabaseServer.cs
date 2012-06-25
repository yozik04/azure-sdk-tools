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
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Model;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
    using WAPPSCmdlet = Microsoft.WindowsAzure.Management.CloudService.WAPPSCmdlet;

    /// <summary>
    /// Retrieves a list of all the SQL Azure servers that belongs to a subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSqlDatabaseServer")]
    public class GetAzureSqlDatabaseServer : SqlDatabaseManagementCmdletBase
    {
        public GetAzureSqlDatabaseServer()
        {
        }

        public GetAzureSqlDatabaseServer(ISqlDatabaseManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Database server name.")]
        [ValidateNotNullOrEmpty]
        public string ServerName { get; set; }

        internal IEnumerable<SqlDatabaseServerContext> GetAzureSqlDatabaseServersProcess(string serverName)
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                SqlDatabaseServerList servers = null;
                WAPPSCmdlet.Operation operation = null;
                try
                {
                    servers = this.RetryCall(s => this.Channel.GetServers(s));
                    operation = WaitForSqlDatabaseOperation();
                    if (!string.IsNullOrEmpty(serverName) && operation != null)
                    {
                        var server = servers.FirstOrDefault(s => s.Name == serverName);

                        if (server != null)
                        {
                            return new List<SqlDatabaseServerContext>
                            {
                                new SqlDatabaseServerContext
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

                return servers.Select(s => new SqlDatabaseServerContext
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
                var servers = this.GetAzureSqlDatabaseServersProcess(this.ServerName);

                if (servers != null)
                {
                    WriteObject(servers, true);
                }
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}