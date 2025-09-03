# PowerShell script to run all microservices in Docker

# Check if development certificate exists
$certPath = "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"
if (-not (Test-Path $certPath)) {
    Write-Host "Development certificate not found. Creating one..."
    dotnet dev-certs https -ep $certPath -p password
    dotnet dev-certs https --trust
    Write-Host "Development certificate created and trusted."
}

# Show menu
function Show-Menu {
    Clear-Host
    Write-Host "================ Microservices Docker Menu ================"
    Write-Host "1: Start all services"
    Write-Host "2: Start AuthServer only"
    Write-Host "3: Start Customer Service only"
    Write-Host "4: Start Order Service only"
    Write-Host "5: Start Product Service only"
    Write-Host "6: Stop all services"
    Write-Host "7: View running services"
    Write-Host "8: View logs"
    Write-Host "Q: Quit"
    Write-Host "========================================================"
}

function Start-AllServices {
    Write-Host "Starting all microservices..."
    docker-compose up -d
    Write-Host "Services started successfully!"
    Write-Host "Access services at:"
    Write-Host "AuthServer API: https://localhost:5002"
    Write-Host "Customer API:   https://localhost:5004"
    Write-Host "Order API:      https://localhost:5006"
    Write-Host "Product API:    https://localhost:5008"
    Write-Host "pgAdmin:        http://localhost:5050"
    Write-Host "  pgAdmin login: admin@admin.com / admin123"
}

function Start-Service($serviceName, $composeFiles) {
    Write-Host "Starting $serviceName services..."
    docker-compose up -d $composeFiles
    Write-Host "$serviceName services started successfully!"
}

function Stop-AllServices {
    Write-Host "Stopping all services..."
    docker-compose down
    Write-Host "All services stopped."
}

function View-RunningServices {
    Write-Host "Current running containers:"
    docker ps
    Write-Host "`nPress any key to continue..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function View-Logs {
    param (
        [string]$service = ""
    )
    
    if ($service -eq "") {
        $service = Read-Host "Enter service name (e.g., authserver.api, customer.api, order.api, product.api, pgadmin) or press Enter for all"
    }
    
    if ($service -eq "") {
        docker-compose logs --tail=100
    }
    else {
        docker-compose logs --tail=100 $service
    }
    
    Write-Host "`nPress any key to continue..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Main menu loop
do {
    Show-Menu
    $input = Read-Host "Please make a selection"
    
    switch ($input) {
        '1' { Start-AllServices }
        '2' { Start-Service "AuthServer" "postgres_authserver authserver.api" }
        '3' { Start-Service "Customer Service" "postgres_customer customer.api" }
        '4' { Start-Service "Order Service" "postgres_order order.api" }
        '5' { Start-Service "Product Service" "postgres_product product.api" }
        '6' { Stop-AllServices }
        '7' { View-RunningServices }
        '8' { View-Logs }
        'q' { return }
    }
    
    if ($input -ne 'q') {
        Write-Host "`nPress any key to return to menu..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
} while ($input -ne 'q')