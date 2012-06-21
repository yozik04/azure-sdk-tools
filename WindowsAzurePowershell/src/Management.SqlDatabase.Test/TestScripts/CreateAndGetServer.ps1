# ----------------------------------------------------------------------------------
#
# Copyright 2011 Microsoft Corporation
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ----------------------------------------------------------------------------------

[CmdletBinding()]
Param
(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateNotNullOrEmpty()]
    [string]
    $subscriptionID, 
    [Parameter(Mandatory=$true, Position=1)]
    [ValidateNotNullOrEmpty()]
    [String]
    $certThumbPrint,
    [Parameter(Mandatory=$true, Position=2)]
    [ValidateNotNullOrEmpty()]
    [String]
    $moduleManifestFileLocation,
    [Parameter(Mandatory=$true, Position=3)]
    [ValidateNotNullOrEmpty()]
    [String]
    $serverLocation
)

Write-Output "`$subscriptionID=$subscriptionID"
Write-Output "`$certThumbPrint=$certThumbPrint"
Write-Output "`$serverLocation=$serverLocation"
Write-Output "`$moduleManifestFileLocation=$moduleManifestFileLocation"

. .\CommonFunctions.ps1

Try
{
    Init-TestEnvironment -subscriptionID $subscriptionID -certThumbPrint $certThumbPrint -moduleManifestFileLocation $moduleManifestFileLocation
    $loginName="mylogin1"
    $loginPassword="Sql@zure1"
    $isTestPass = $False
    
    # Create Server
    Write-Output "Creating server"
    $server = New-AzureSqlDatabaseServer -AdministratorLogin $loginName -AdministratorLoginPassword $loginPassword -Location $serverLocation
    Assert {$server} "Server is not created"
    Write-Output "Server $($server.ServerName) created"
    
    # Get Server
    Write-Output "Getting server"
    $getServer = Get-AzureSqlDatabaseServer | Where-Object {$_.ServerName -eq $server.ServerName}
    Assert {$getServer} "Can not get server $($server.ServerName)"
    Write-Output "Got server $($server.ServerName)"
    
    # Validate server Name
    Write-Output "Validating server"
    Assert {$server.ServerName -eq $getServer.ServerName} "ServerName didn't match Expected:$($server.ServerName)  Actual:$($getServer.ServerName)"
    Write-Output "Validation successful"
    $isTestPass = $True
}
Finally
{
    if($server)
    {
        # Drop server
        Write-Output "Dropping server $($server.ServerName)"
        Remove-AzureSqlDatabaseServer -ServerName $server.ServerName
        Write-Output "Dropped server $($server.ServerName)"
        
        #Validate Drop server
        Write-Output 'Validating drop'
        $getDroppedServer = Get-AzureSqlDatabaseServer | Where-Object {$_.ServerName -eq $server.ServerName}
        Assert {!$getDroppedServer} "Server is not dropped"
        Write-Output "Validation successful"
    }
    if($IsTestPass)
    {
        Write-Output "PASS"
    }
    else
    {
        Write-Output "FAILED"
    }
}
