# PowerShell script to run Order API in Docker

# Check if development certificate exists
$certPath = "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"
if (-not (Test-Path $certPath)) {
    Write-Host "Development certificate not found. Creating one..."
    dotnet dev-certs https -ep $certPath -p password
    dotnet dev-certs https --trust
    Write-Host "Development certificate created and trusted."
}

# Navigate to the solution root directory
$currentDir = Get-Location
$solutionRoot = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $solutionRoot

# Run Docker Compose
Write-Host "Starting Order API and PostgreSQL with Docker Compose..."
docker-compose -f docker-compose.order.yml up -d

# Return to original directory
Set-Location $currentDir

Write-Host "Order API is running at:"
Write-Host "HTTP:  http://localhost:5005"
Write-Host "HTTPS: https://localhost:5006"