[CmdletBinding()]
param(
    [string]$Project = "$PSScriptRoot/../GoalTracker.Api.csproj"
)

Write-Host "Resetting daily data..."
dotnet run --project $Project -- reset-today


