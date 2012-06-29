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

using System.Management.Automation;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Management.CloudService.Model;
using Microsoft.WindowsAzure.Management.CloudService.Test;
using Microsoft.WindowsAzure.Management.SqlDatabase.Server.Cmdlet;
using Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTest;
using Microsoft.WindowsAzure.Management.Test.Stubs;
using Microsoft.WindowsAzure.Management.SqlDatabase.Services;

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTests.Server.Cmdlet
{
    [TestClass]
    public class AzureSqlDatabaseServerTests : TestBase
    {
        [TestInitialize]
        public void SetupTest()
        {
            Management.Extensions.CmdletSubscriptionExtensions.SessionManager = new InMemorySessionManager();
        }

        [TestMethod]
        public void NewAzureSqlDatabaseServerProcessTest()
        {
            SimpleSqlDatabaseManagement channel = new SimpleSqlDatabaseManagement();
            channel.NewServerThunk = ar =>
            {
                string newServerName = "NewServerName";

                Assert.AreEqual(((NewSqlDatabaseServerInput)ar.Values["input"]).AdministratorLogin, "MyLogin");
                Assert.AreEqual(((NewSqlDatabaseServerInput)ar.Values["input"]).AdministratorLoginPassword, "MyPassword");
                Assert.AreEqual(((NewSqlDatabaseServerInput)ar.Values["input"]).Location, "MyLocation");

                XmlElement operationResult = new XmlDocument().CreateElement("ServerName", "http://schemas.microsoft.com/sqlazure/2010/12/");
                operationResult.InnerText = newServerName;
                return operationResult;
            };

            NewAzureSqlDatabaseServer newAzureSqlDatabaseServer = new NewAzureSqlDatabaseServer(channel) { ShareChannel = true };
            newAzureSqlDatabaseServer.CommandRuntime = new MockCommandRuntime();
            var newServerResult = newAzureSqlDatabaseServer.NewAzureSqlDatabaseServerProcess("MyLogin", "MyPassword", "MyLocation");
            Assert.AreEqual(newServerResult.ServerName, "NewServerName");
            Assert.AreEqual(newServerResult.OperationStatus, "Success");
        }
    }
}
