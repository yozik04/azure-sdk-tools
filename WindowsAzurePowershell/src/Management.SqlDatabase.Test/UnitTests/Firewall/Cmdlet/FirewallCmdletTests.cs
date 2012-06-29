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

using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Management.CloudService.Test;
using Microsoft.WindowsAzure.Management.SqlDatabase.Firewall.Cmdlet;
using Microsoft.WindowsAzure.Management.SqlDatabase.Services;
using Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTest;
using Microsoft.WindowsAzure.Management.Test.Stubs;

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTests.Firewall.Cmdlet
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
        public void NewAzureSqlDatabaseFirewallRuleProcessTest()
        {
            MockCommandRuntime commandRuntime = new MockCommandRuntime();
            SimpleSqlDatabaseManagement channel = new SimpleSqlDatabaseManagement();
            bool newFirewallRuleCalled = false;
            channel.NewServerFirewallRuleThunk = ar =>
            {
                newFirewallRuleCalled = true;
                Assert.AreEqual("Server1", (string)ar.Values["serverName"]);
                Assert.AreEqual("Rule1", (string)ar.Values["ruleName"]);
                Assert.AreEqual(((NewSqlDatabaseFirewallRuleInput)ar.Values["input"]).StartIpAddress, "0.0.0.0");
                Assert.AreEqual(((NewSqlDatabaseFirewallRuleInput)ar.Values["input"]).EndIpAddress, "1.1.1.1");
            };

            // New firewall rule with IpRange parameter set
            NewAzureSqlDatabaseFirewallRule newAzureSqlDatabaseFirewallRule = new NewAzureSqlDatabaseFirewallRule(channel) { ShareChannel = true };
            newAzureSqlDatabaseFirewallRule.CommandRuntime = commandRuntime;
            var newFirewallResult = newAzureSqlDatabaseFirewallRule.NewAzureSqlDatabaseFirewallRuleProcess("IpRange", "Server1", "Rule1", "0.0.0.0", "1.1.1.1");
            Assert.AreEqual("Server1", newFirewallResult.ServerName);
            Assert.AreEqual("Rule1", newFirewallResult.RuleName);
            Assert.AreEqual("0.0.0.0", newFirewallResult.StartIpAddress);
            Assert.AreEqual("1.1.1.1", newFirewallResult.EndIpAddress);
            Assert.AreEqual("Success", newFirewallResult.OperationStatus);
            Assert.AreEqual(true, newFirewallRuleCalled);
            Assert.AreEqual(0, commandRuntime.ErrorRecords.Count);

            channel.NewServerFirewallRuleThunk = null;
            newFirewallRuleCalled = false;
            channel.NewServerFirewallRuleWithIpDetectThunk = ar =>
            {
                newFirewallRuleCalled = true;
                Assert.AreEqual("Server2", (string)ar.Values["serverName"]);
                Assert.AreEqual("Rule2", (string)ar.Values["ruleName"]);

                XmlElement operationResult = new XmlDocument().CreateElement("IpAddress", "http://schemas.microsoft.com/sqlazure/2010/12/");
                operationResult.InnerText = "1.2.3.4";
                return operationResult;
            };
            
            // New Firewall rule with IpDetect parameter set
            newFirewallResult = newAzureSqlDatabaseFirewallRule.NewAzureSqlDatabaseFirewallRuleProcess("IpDetection", "Server2", "Rule2", null, null);
            Assert.AreEqual("Server2", newFirewallResult.ServerName);
            Assert.AreEqual("Rule2", newFirewallResult.RuleName);
            Assert.AreEqual("1.2.3.4", newFirewallResult.StartIpAddress);
            Assert.AreEqual("1.2.3.4", newFirewallResult.EndIpAddress);
            Assert.AreEqual("Success", newFirewallResult.OperationStatus);
            Assert.AreEqual(true, newFirewallRuleCalled);
            Assert.AreEqual(0, commandRuntime.ErrorRecords.Count);
        }
    }
}
