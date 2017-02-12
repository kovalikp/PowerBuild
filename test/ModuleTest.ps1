try {
    Remove-Module -Name PowerBuild -ErrorAction Ignore
    Import-Module $PSScriptRoot\..\src\PowerBuild\bin\Debug\PowerBuild.psd1

    Get-Command -Module PowerBuild

    Invoke-MSBuild $PSScriptRoot\..\src\PowerBuild\PowerBuild.csproj -Target GetTargetFrameworkMoniker | % { $_.GetMetadata("FullPath") }
}
Finally {
    Remove-Module -Name PowerBuild -Force -ErrorAction Ignore
}