param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue).Source

if (-not $dotnet -and (Test-Path "C:\Program Files\dotnet\dotnet.exe")) {
    $dotnet = "C:\Program Files\dotnet\dotnet.exe"
}

if (-not $dotnet) {
    throw "dotnet SDK was not found. Install the .NET SDK or add dotnet.exe to PATH."
}

& $dotnet build (Join-Path $root "src\CustomItemRegistry\CustomItemRegistry.csproj") -c $Configuration
& $dotnet build (Join-Path $root "src\ExampleCustomItemPlugin\ExampleCustomItemPlugin.csproj") -c $Configuration
