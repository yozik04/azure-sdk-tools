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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test
{
    [TestClass]
    public class FunctionalTest
    {
        private string subscriptionID;
        private string certThumbprint;
        private string serverLocation;

        [TestInitialize]
        public void Setup()
        {
            this.subscriptionID = "055c4f05-8a3d-4f6b-97fc-055ff1aa1ffb";
            this.certThumbprint = "C37F325D5F41FED506B59BD2A15FBEE6F4FA7A19";
            this.serverLocation = "North Central US";

            new NewAzureSqlDatabaseFirewallRule();
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void ServerTest()
        {
            string arguments = string.Format("-subscriptionID \"{0}\" -certThumbPrint \"{1}\" -serverLocation \"{2}\"", this.subscriptionID, this.certThumbprint, this.serverLocation);
            bool testResult = PSScriptExecutor.ExecuteScript("CreateAndGetServer.ps1", arguments);
            Assert.IsTrue(testResult);
        }

    }
}
