# Run script for Banking System
# Usage: .\run.ps1

$exePath = "BankingSystem.Presentation\bin\Debug\net48\BankingSystem.Presentation.exe"

if (Test-Path $exePath) {
    Write-Host "Starting Banking System..." -ForegroundColor Green
    & $exePath
} else {
    Write-Host "Executable not found. Building first..." -ForegroundColor Yellow
    dotnet build BankingSystem.sln --configuration Debug
    
    if (Test-Path $exePath) {
        Write-Host "`nStarting Banking System..." -ForegroundColor Green
        & $exePath
    } else {
        Write-Host "`nBuild failed or executable not found." -ForegroundColor Red
        exit 1
    }
}
