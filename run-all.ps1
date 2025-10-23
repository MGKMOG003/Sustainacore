param(
  [int]$ApiPort = 5180,
  [int]$WebPort = 5166
)

Write-Host "Killing old dotnet watchers..." -ForegroundColor Yellow
Get-Process dotnet -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowTitle -like "*watch*" } | Stop-Process -Force -ErrorAction SilentlyContinue

$apiProj = ".\src\Web\Sustainacore.Api\Sustainacore.Api.csproj"
$webProj = ".\src\Web\Sustainacore.Web\Sustainacore.Web.csproj"

# Start API
$apiEnv = @{ ASPNETCORE_URLS = "http://localhost:$ApiPort" }
Start-Process pwsh -ArgumentList @("-NoLogo","-NoProfile","-Command","`$env:ASPNETCORE_URLS='$($apiEnv.ASPNETCORE_URLS)'; dotnet watch run --project `"$apiProj`"") `
  -WorkingDirectory (Get-Location).Path -WindowStyle Minimized

# Start Web
$webEnv = @{ ASPNETCORE_URLS = "http://localhost:$WebPort" }
Start-Process pwsh -ArgumentList @("-NoLogo","-NoProfile","-Command","`$env:ASPNETCORE_URLS='$($webEnv.ASPNETCORE_URLS)'; dotnet watch run --project `"$webProj`"") `
  -WorkingDirectory (Get-Location).Path -WindowStyle Minimized

Start-Sleep -Seconds 3
Write-Host "API: http://localhost:$ApiPort" -ForegroundColor Green
Write-Host "Web: http://localhost:$WebPort" -ForegroundColor Green
Start-Process "http://localhost:$ApiPort/swagger"
Start-Process "http://localhost:$WebPort"
