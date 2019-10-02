$ServiceName = "AspNetCore.WSMQTT.VolumeControl"
$UserName = "GOLD\VolumeControl"
$BinDir = "C:\Users\User\Qsync\git\AspNetCore.WSMQTT\bin\Release\netcoreapp3.0"
$Exe = "C:\Users\User\Qsync\git\AspNetCore.WSMQTT\bin\Release\netcoreapp3.0\AspNetCore.WSMQTT.exe"

Stop-Service -Name $ServiceName
sc.exe delete $ServiceName

$acl = Get-Acl $BinDir
$aclRuleArgs = $UserName, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $BinDir

New-Service -Name $ServiceName -BinaryPathName $Exe -Credential $UserName -Description "ASP.NET Core MQTT Volume Control" -DisplayName $ServiceName -StartupType Automatic
Start-Service -Name $ServiceName
Get-Service -Name $ServiceName