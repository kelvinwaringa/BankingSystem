# Build script for Banking System
# Usage: .\build.ps1

Write-Host "Building Banking System..." -ForegroundColor Green
dotnet build BankingSystem.sln --configuration Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild successful!" -ForegroundColor Green
    Write-Host "Executable: BankingSystem.Presentation\bin\Debug\net48\BankingSystem.Presentation.exe" -ForegroundColor Cyan
} else {
    Write-Host "`nBuild failed. Please check errors above." -ForegroundColor Red
    exit 1
}
