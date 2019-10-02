sc.exe delete "AspNetCore.WSMQTT.VolumeControl"

$acl = Get-Acl "C:\Users\User\Qsync\git\AspNetCore.WSMQTT\bin\Release\netcoreapp3.0"
$aclRuleArgs = {GOLD\VolumeControl}, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl "C:\Users\User\Qsync\git\AspNetCore.WSMQTT\bin\Release\netcoreapp3.0"

New-Service -Name AspNetCore.WSMQTT.VolumeControl -BinaryPathName C:\Users\User\Qsync\git\AspNetCore.WSMQTT\bin\Release\netcoreapp3.0\AspNetCore.WSMQTT.exe -Credential GOLD\VolumeControl -Description "ASP.NET Core MQTT Volume Control" -DisplayName "AspNetCore.WSMQTT.VolumeControl" -StartupType Automatic