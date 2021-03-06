﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Management.Test.Tests.Cmdlets
{
    using System.Linq;
    using VisualStudio.TestTools.UnitTesting;
    using Stubs;
    using TestData;
    using Management.Cmdlets;
    using Management.Extensions;
    using Management.Services;

    [TestClass]
    public class SelectSubscriptionTest
    {
        [TestInitialize]
        public void SetupTest()
        {
            CmdletSubscriptionExtensions.SessionManager = new InMemorySessionManager();

            GlobalPathInfo.GlobalSettingsDirectory = Data.AzureAppDir;
        }

        [TestMethod]
        public void TestSetCurrentSubscription()
        {
            var globalComponents = GlobalComponents.CreateFromPublishSettings(GlobalPathInfo.GlobalSettingsDirectory, null, Data.ValidPublishSettings.First());

            var selectSubscriptionCommand = new SelectAzureSubscriptionCommand();

            // Check that current subscription is the default one
            Assert.AreEqual("Windows Azure Sandbox 9-220", selectSubscriptionCommand.GetCurrentSubscription().SubscriptionName);

            // Change it and make sure it got changed
            selectSubscriptionCommand.SelectSubscriptionProcess("Set", "mysub1", Data.ValidSubscriptionsData.First());
            Assert.AreEqual("mysub1", selectSubscriptionCommand.GetCurrentSubscription().SubscriptionName);

            // Clean
            globalComponents.DeleteGlobalComponents();
        }

        [TestMethod]
        public void TestClearCurrentSubscription()
        {
            var globalComponents = GlobalComponents.CreateFromPublishSettings(GlobalPathInfo.GlobalSettingsDirectory, null, Data.ValidPublishSettings.First());

            var selectSubscriptionCommand = new SelectAzureSubscriptionCommand();

            // Check that current subscription is the default one
            Assert.AreEqual("Windows Azure Sandbox 9-220", selectSubscriptionCommand.GetCurrentSubscription().SubscriptionName);

            // Change it and make sure it got changed
            selectSubscriptionCommand.SelectSubscriptionProcess("Set", "mysub1", Data.ValidSubscriptionsData.First());
            Assert.AreEqual("mysub1", selectSubscriptionCommand.GetCurrentSubscription().SubscriptionName);

            // Clear current subscription and make sure it goes back to default
            selectSubscriptionCommand.SelectSubscriptionProcess("Clear", null, Data.ValidSubscriptionsData.First());
            Assert.AreEqual("Windows Azure Sandbox 9-220", selectSubscriptionCommand.GetCurrentSubscription().SubscriptionName);

            // Clean
            globalComponents.DeleteGlobalComponents();
        }
    }
}
