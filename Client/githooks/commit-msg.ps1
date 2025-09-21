param([string]$commitMsgFile)

$versionFile = Join-Path $PSScriptRoot "..\..\Client\version.txt"

if (-not (Test-Path $versionFile)) {
    New-Item -Path $versionFile -ItemType File -Force | Out-Null
    Set-Content -Path $versionFile -Value "0.0.0" -NoNewline
}

$currentVersion = (Get-Content $versionFile -Raw).Trim()
if (-not ($currentVersion -match "^\d+\.\d+\.\d+$")) {
    Write-Host "Current version in version.txt is invalid: $currentVersion"
    exit 1
}

$currentParts = $currentVersion -split "\." | ForEach-Object { [int]$_ }

$commitMessage = Get-Content $commitMsgFile -Raw
if ($commitMessage -match "\d+\.\d+\.\d+") {
    $newVersion = $matches[0]
    $newParts = $newVersion -split "\." | ForEach-Object { [int]$_ }

    $isValid = ($newParts[0] -eq $currentParts[0] + 1 -and $newParts[1] -eq 0 -and $newParts[2] -eq 0) -or
            ($newParts[0] -eq $currentParts[0] -and $newParts[1] -eq $currentParts[1] + 1 -and $newParts[2] -eq 0) -or
            ($newParts[0] -eq $currentParts[0] -and $newParts[1] -eq $currentParts[1] -and $newParts[2] -eq $currentParts[2] + 1)

    if ($isValid) {
        Write-Host "Updating version.txt to $newVersion"
        Set-Content -Path $versionFile -Value $newVersion -NoNewline
        exit 0
    } else {
        Write-Host "Invalid version bump: $currentVersion -> $newVersion"
        Write-Host "Allowed: increment only one number by +1"
        exit 1
    }
} else {
    Write-Host "No version found in commit message. Please include version like 0.2.0"
    exit 1
}
