# PowerBuild

[![Build status](https://ci.appveyor.com/api/projects/status/vvw9k1nrvdv2r0ep/branch/master?svg=true)](https://ci.appveyor.com/project/kovalikp/powerbuild/branch/master)

PowerBuild module provides full MSBuild integration into PowerShell pipeline. Invoke-MSBuild cmdlet writes build results into output stream, making them available for processing by other cmdlets.

Invoke MSBuild and store results to local variable.

```powershell
PS> $buildResults = Invoke-MSBuild -Project c:\Workspace\GitHub\kovalikp\PowerBuild\PowerBuild.sln `
>>   -Target Build -Verbosity Minimal -DefaultLogger Host `
>>   -Property @{Configuration="Release"}	
```
Manipulate results in pipeline.

```powershell
PS> $buildResults | Select-Object -ExpandProperty Items | Format-Table ItemSpec,MetadataCount
```

![Console](doc/Console.png)