using System.Xml.Linq;
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Management.SqlDatabase.Firewall.Cmdlet;
using Microsoft.WindowsAzure.Management.SqlDatabase.Test.Utilities;
using Microsoft.WindowsAzure.Management.Utilities;
using Microsoft.WindowsAzure.Management.XmlSchema;


namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test
{
    [TestClass]
    public class FunctionalTest
    {
        private string subscriptionID;
        private string serializedCert;
        private string serverLocation;

        [TestInitialize]
        public void Setup()
        {

            PublishData publishData = General.DeserializeXmlFile<PublishData>("Azure.publishsettings");
            PublishDataPublishProfile publishProfile = publishData.Items[0];
            this.serializedCert = publishProfile.ManagementCertificate;
            this.subscriptionID = publishProfile.Subscription[0].Id;

            XElement root = XElement.Load("SqlDatabaseSettings.xml");
            this.serverLocation = root.Element("ServerLocation").Value;

            new NewAzureSqlDatabaseServerFirewallRule();
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void ServerTest()
        {
            string arguments = string.Format("-subscriptionID \"{0}\" -serializedCert \"{1}\" -serverLocation \"{2}\"", this.subscriptionID, this.serializedCert, this.serverLocation);
            bool testResult = PSScriptExecutor.ExecuteScript("CreateGetDeleteServer.ps1", arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void FirewallTest()
        {
            string arguments = string.Format("-subscriptionID \"{0}\" -serializedCert \"{1}\" -serverLocation \"{2}\"", this.subscriptionID, this.serializedCert, this.serverLocation);
            bool testResult = PSScriptExecutor.ExecuteScript("CreateGetDropFirewall.ps1", arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void ResetServerPassword()
        {
            string arguments = string.Format("-subscriptionID \"{0}\" -serializedCert \"{1}\" -serverLocation \"{2}\"", this.subscriptionID, this.serializedCert, this.serverLocation);
            bool testResult = PSScriptExecutor.ExecuteScript("ResetPassword.ps1", arguments);
            Assert.IsTrue(testResult);
        }
    }
}
