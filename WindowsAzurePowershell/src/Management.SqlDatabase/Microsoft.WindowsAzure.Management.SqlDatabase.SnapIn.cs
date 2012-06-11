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

namespace Microsoft.WindowsAzure.Samples.ManagementTools.PowerShell
{
    using System.ComponentModel;
    using System.Management.Automation;

    /// <summary>
    /// Installer class for the Windows Azure Platform PowerShell Cmdlets
    /// Windows Powershell v2.0 snap-in.
    /// </summary>
    [RunInstaller(true)]
    public class WAPPSCmdletsSnapIn : PSSnapIn
    {
        /// <summary>
        /// Gets the snap-in description.
        /// </summary>
        public override string Description
        {
            get { return "Windows Azure PowerShell Cmdlets - SqlDB"; }
        }

        /// <summary>
        /// Gets the snap-in name.
        /// </summary>
        public override string Name
        {
            get { return "WAPSCmdlets.SqlDB"; }
        }

        /// <summary>
        /// Gets the snap-in vendor.
        /// </summary>
        public override string Vendor
        {
            get { return "Microsoft Developer & Platform Evangelism"; }
        }
    }
}