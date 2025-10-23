param(
  [string]$ApiUrl = "http://localhost:5180",
  [string]$WebUrl = "http://localhost:5166"
)

Write-Host "== Dotnet info =="; dotnet --info

$apiProj = "src/Web/Sustainacore.Api/Sustainacore.Api.csproj"
$webProj = "src/Web/Sustainacore.Web/Sustainacore.Web.csproj"
$contractsProj = "src/Contracts/Sustainacore.Contracts/Sustainacore.Contracts.csproj"

# If Central Package Management is enabled, don't try to add/update per-project versions.
$dirProps = "Directory.Packages.props"
if (!(Test-Path $dirProps)) {
  Write-Host "WARNING: Directory.Packages.props not found; you are not using central package management."
  Write-Host "You may add packages with 'dotnet add <proj> package <name> --version <ver>'."
} else {
  Write-Host "Central package management detected: $dirProps"
}

# Restore all
dotnet restore

# Build all
dotnet build -c Debug

Write-Host "== Start API =="
Start-Process powershell -ArgumentList "dotnet run --project $apiProj --urls $ApiUrl"

Start-Sleep -Seconds 3

Write-Host "== Start Web =="
Start-Process powershell -ArgumentList "dotnet run --project $webProj --urls $WebUrl"

Write-Host "Web: $WebUrl"
Write-Host "API: $ApiUrl"
