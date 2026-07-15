#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Fix AiderGems extraction from Champions Legion export
#>

param(
    [string]$ExportPath = "F:\Nueva carpeta\AssetRipper_export_20260713_021249",
    [string]$OutputPath = "F:\Evo_Core\samples\Evo.MOBA\src\Evo.MOBA\Data\Extracted"
)

Write-Host "=== Fixing AiderGems Extraction ===" -ForegroundColor Green

# ============================================================
# AIDER GEMS DATA - the data has fewer columns
# ============================================================
Write-Host "`nExtracting Aider Gems..." -ForegroundColor Yellow

$aiderGemsRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\AiderGemsData.bytes" -Raw -Encoding UTF8
$aiderGemsLines = $aiderGemsRaw -split "`n" | Where-Object { $_ -match '^10\d{6},' }

$aiderGems = @()
foreach ($line in $aiderGemsLines) {
    $line = $line.TrimEnd("`r")
    $parts = $line -split ','
    if ($parts.Count -ge 15) {
        $aiderGems += [pscustomobject]@{
            Id              = $parts[0]
            NameTag         = $parts[1]
            IsDefault       = $parts[2]
            GemType         = $parts[3]
            # The actual stat columns start after the empty ones
            # MaxHP through Tenacity - parse what we can
            Stats = @{
                MaxHP           = if ($parts.Count -gt 4) { $parts[4] } else { "" }
                MaxMP           = if ($parts.Count -gt 5) { $parts[5] } else { "" }
                MaxPower        = if ($parts.Count -gt 6) { $parts[6] } else { "" }
                HPRegen         = if ($parts.Count -gt 7) { $parts[7] } else { "" }
                MPRegen         = if ($parts.Count -gt 8) { $parts[8] } else { "" }
                PowerRegen      = if ($parts.Count -gt 9) { $parts[9] } else { "" }
                PhysicalAttack  = if ($parts.Count -gt 10) { $parts[10] } else { "" }
                MagicAttack     = if ($parts.Count -gt 11) { $parts[11] } else { "" }
                PhysicalArmor   = if ($parts.Count -gt 12) { $parts[12] } else { "" }
                MagicArmor      = if ($parts.Count -gt 13) { $parts[13] } else { "" }
                MovementSpeed   = if ($parts.Count -gt 14) { $parts[14] } else { "" }
                AttackSpeed     = if ($parts.Count -gt 15) { $parts[15] } else { "" }
                CooldownScale   = if ($parts.Count -gt 16) { $parts[16] } else { "" }
                CriticalChance  = if ($parts.Count -gt 17) { $parts[17] } else { "" }
                CriticalDamage  = if ($parts.Count -gt 18) { $parts[18] } else { "" }
                PhysicalPierceFixed = if ($parts.Count -gt 19) { $parts[19] } else { "" }
                PhysicalPierceRatio = if ($parts.Count -gt 20) { $parts[20] } else { "" }
                MagicPierceFixed    = if ($parts.Count -gt 21) { $parts[21] } else { "" }
                MagicPierceRatio    = if ($parts.Count -gt 22) { $parts[22] } else { "" }
                LifeSteal       = if ($parts.Count -gt 23) { $parts[23] } else { "" }
                SpellVamp       = if ($parts.Count -gt 24) { $parts[24] } else { "" }
                Tenacity        = if ($parts.Count -gt 25) { $parts[25] } else { "" }
            }
        }
    }
}

# Update aiders.json with gems
$aidersData = Get-Content -LiteralPath "$OutputPath\aiders.json" -Raw | ConvertFrom-Json
@{
    Aiders = $aidersData.Aiders
    Gems   = $aiderGems
} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\aiders.json" -Encoding UTF8

Write-Host "  Extracted $($aiderGems.Count) aider gems" -ForegroundColor Green

# Copy to Evo.MOBA Data folder
$targetDataPath = "F:\Evo_Core\samples\Evo.MOBA\src\Evo.MOBA\Data"
Copy-Item -LiteralPath "$OutputPath\aiders.json" -Destination $targetDataPath -Force

Write-Host "`n=== FIX COMPLETE ===" -ForegroundColor Green