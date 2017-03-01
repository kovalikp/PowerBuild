# PowerBuild

[![Build status](https://ci.appveyor.com/api/projects/status/vvw9k1nrvdv2r0ep/branch/master?svg=true)](https://ci.appveyor.com/project/kovalikp/powerbuild/branch/master)

PowerBuild module provides full MSBuild integration into PowerShell pipeline. Unlike simmilar modules that provide wrapper around `msbuild.exe`, PowerBuild's `Invoke-MSBuild` cmdlet uses `Microstft.Build` API directly. It means it can write build results into output, making the results available for processing by other cmdlets. Default logger utilizes Error, Warning and Verbose streams to log MSBuild events.

## Getting started:

You can get the module from the [PowerShell Gallery](https://www.powershellgallery.com/packages/PowerBuild) by executing following script.

```powershell
Install-Module -Name PowerBuild 
```
## Using PowerBuild

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

### Logging

Default logger can be specified using `-DefaultLogger` parameter. The parameter currently recognized these values.
 - None - Disables default logger.
 - Streams (Default) - Uses Error, Warning and Verbose streams to write logs.
 - Host - Uses PowerShell host console to write logs. This is equivalent of msbuild.exe's console logger.
 
Level of logging is affected by `-Verbose` parameter.

Additional loggers can be created using `New-Logger`, `New-ConsoleLogger` or `New-FileLogger` commnandlets and passed to `-Logger` parameter.

### Example

![Console](doc/Console.png)
