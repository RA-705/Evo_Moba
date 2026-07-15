#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Fix Equipment and AiderGems extraction from Champions Legion export
#>

param(
    [string]$ExportPath = "F:\Nueva carpeta\AssetRipper_export_20260713_021249",
    [string]$OutputPath = "F:\Evo_Core\samples\Evo.MOBA\src\Evo.MOBA\Data\Extracted"
)

Write-Host "=== Fixing Equipment & AiderGems Extraction ===" -ForegroundColor Green

# ============================================================
# EQUIPMENT DATA - Parse from raw lines (skip corrupted header)
# ============================================================
Write-Host "`n[1/2] Extracting Equipment..." -ForegroundColor Yellow

$equipRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\EquipmentData.bytes" -Raw -Encoding UTF8
$equipLines = $equipRaw -split "`n" | Where-Object { $_ -match '^\d{4},' }

$equipList = @()
foreach ($line in $equipLines) {
    $parts = $line -split ','
    # The data has 37 columns (0-36), not 42
    if ($parts.Count -ge 37) {
        $equipList += [pscustomobject]@{
            Id              = [int]$parts[0]
            NameTag         = $parts[1]
            Name            = $parts[2]
            Icon            = $parts[3]
            Description     = $parts[4]
            BriefDesc       = $parts[5]
            EquipmentType   = $parts[6]
            Quality         = [int]$parts[7]
            PurchasePrice   = [int]$parts[8]
            AssemblePrice   = [int]$parts[9]
            SalePrice       = [int]$parts[10]
            Components      = if ($parts[11]) { ($parts[11] -split ' ' | Where-Object { $_ }) | ForEach-Object { [int]$_ } } else { @() }
            Ability         = $parts[12]
            Buffs           = $parts[13]
            Stats = @{
                MaxHP           = if ($parts[14]) { [double]$parts[14] } else { 0 }
                MaxMP           = if ($parts[15]) { [double]$parts[15] } else { 0 }
                MaxPower        = if ($parts[16]) { [double]$parts[16] } else { 0 }
                HPRegen         = if ($parts[17]) { [double]$parts[17] } else { 0 }
                MPRegen         = if ($parts[18]) { [double]$parts[18] } else { 0 }
                PowerRegen      = if ($parts[19]) { [double]$parts[19] } else { 0 }
                PhysicalAttack  = if ($parts[20]) { [double]$parts[20] } else { 0 }
                MagicAttack     = if ($parts[21]) { [double]$parts[21] } else { 0 }
                PhysicalArmor   = if ($parts[22]) { [double]$parts[22] } else { 0 }
                MagicArmor      = if ($parts[23]) { [double]$parts[23] } else { 0 }
                MovementSpeed   = if ($parts[24]) { [double]$parts[24] } else { 0 }
                AttackSpeed     = if ($parts[25]) { [double]$parts[25] } else { 0 }
                CooldownScale   = if ($parts[26]) { [double]$parts[26] } else { 0 }
                CriticalChance  = if ($parts[27]) { [double]$parts[27] } else { 0 }
                CriticalDamage  = if ($parts[28]) { [double]$parts[28] } else { 0 }
                PhysicalPierceFixed = if ($parts[29]) { [double]$parts[29] } else { 0 }
                PhysicalPierceRatio = if ($parts[30]) { [double]$parts[30] } else { 0 }
                MagicPierceFixed    = if ($parts[31]) { [double]$parts[31] } else { 0 }
                MagicPierceRatio    = if ($parts[32]) { [double]$parts[32] } else { 0 }
                LifeSteal       = if ($parts[33]) { [double]$parts[33] } else { 0 }
                SpellVamp       = if ($parts[34]) { [double]$parts[34] } else { 0 }
                Tenacity        = if ($parts[35]) { [double]$parts[35] } else { 0 }
                Rating          = if ($parts[36]) { [int]$parts[36] } else { 0 }
            }
        }
    }
}

$equipList | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\equipment.json" -Encoding UTF8
Write-Host "  Extracted $($equipList.Count) equipment items" -ForegroundColor Green

# ============================================================
# AIDER GEMS DATA - Parse properly with correct column mapping
# ============================================================
Write-Host "`n[2/2] Extracting Aider Gems..." -ForegroundColor Yellow

$aiderGemsRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\AiderGemsData.bytes" -Raw -Encoding UTF8
$aiderGemsLines = $aiderGemsRaw -split "`n" | Where-Object { $_ -match '^102\d{5}' }

$aiderGems = @()
foreach ($line in $aiderGemsLines) {
    $parts = $line -split ','
    $aiderGems += [pscustomobject]@{
        Id              = $parts[0]
        NameTag         = if ($parts.Count -gt 1) { $parts[1] } else { "" }
        IsDefault       = if ($parts.Count -gt 2) { $parts[2] } else { "" }
        GemType         = if ($parts.Count -gt 3) { $parts[3] } else { "" }
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

# Update aiders.json with gems
$aidersData = Get-Content -LiteralPath "$OutputPath\aiders.json" -Raw -Encoding UTF8 | ConvertFrom-Json
@{
    Aiders = $aidersData.Aiders
    Gems   = $aiderGems
} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\aiders.json" -Encoding UTF8

Write-Host "  Extracted $($aiderGems.Count) aider gems" -ForegroundColor Green

# Copy to Evo.MOBA Data folder
$targetDataPath = "F:\Evo_Core\samples\Evo.MOBA\src\Evo.MOBA\Data"
Copy-Item -LiteralPath "$OutputPath\equipment.json" -Destination $targetDataPath -Force
Copy-Item -LiteralPath "$OutputPath\aiders.json" -Destination $targetDataPath -Force

Write-Host "`n=== FIX COMPLETE ===" -ForegroundColor Green
Write-Host "Updated: equipment.json, aiders.json" -ForegroundColor Cyan