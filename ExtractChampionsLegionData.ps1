#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Extract Champions Legion data from AssetRipper export and convert to Evo.MOBA JSON format
#>

param(
    [string]$ExportPath = "F:\Nueva carpeta\AssetRipper_export_20260713_021249",
    [string]$OutputPath = "F:\Evo_Core\samples\Evo.MOBA\src\Evo.MOBA\Data\Extracted"
)

# Create output directory
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

Write-Host "=== Extracting Champions Legion Data ===" -ForegroundColor Green
Write-Host "Source: $ExportPath" -ForegroundColor Cyan
Write-Host "Output: $OutputPath" -ForegroundColor Cyan

# Helper function to parse CSV with corrupted headers
function Parse-CSVData {
    param([string]$Path, [string[]]$ExpectedHeaders)
    $raw = Get-Content -LiteralPath $Path -Raw
    $lines = $raw -split "`n" | Where-Object { $_ -match '^[0-9]' }
    $results = @()
    foreach ($line in $lines) {
        $parts = $line -split ','
        if ($parts.Count -ge $ExpectedHeaders.Count) {
            $obj = [ordered]@{}
            for ($i = 0; $i -lt $ExpectedHeaders.Count; $i++) {
                $obj[$ExpectedHeaders[$i]] = if ($i -lt $parts.Count) { $parts[$i].Trim() } else { "" }
            }
            $results += [pscustomobject]$obj
        }
    }
    return $results
}

# ============================================================
# 1. HERO DATA
# ============================================================
Write-Host "`n[1/7] Extracting Heroes..." -ForegroundColor Yellow

$heroHeaders = @("Id", "NameTag", "HeroClass", "HeroClassVice", "AssetData", "ScoreGoldRatio", "ScoreDamageCausedRatio", "ScoreDamageTakenRatio", "ScoreKillRatio", "ScoreAssistRatio", "ScoreDeathRatio", "ScoreKdaRatio")
$heroes = Parse-CSVData "$ExportPath\Assets\Resources\config\HeroData.bytes" $heroHeaders

$heroInfoHeaders = @("ItemId", "HeroId", "HeroName", "HasIllustration", "IllustrationUrl")
$heroInfos = Parse-CSVData "$ExportPath\Assets\Resources\config\HeroInfo.bytes" $heroInfoHeaders

$heroAvatarHeaders = @("ItemId", "NameTag", "HeroId", "ResId", "MaxHp", "HasIllustration", "IllustrationUrl")
$heroAvatars = Parse-CSVData "$ExportPath\Assets\Resources\config\HeroAvatar.bytes" $heroAvatarHeaders

$heroExpHeaders = @("Level", "Exp")
$heroExp = Parse-CSVData "$ExportPath\Assets\Resources\config\HeroExpData.bytes" $heroExpHeaders

# Merge hero data
$heroList = @()
foreach ($h in $heroes) {
    $info = $heroInfos | Where-Object { [int]$_.HeroId -eq [int]$h.Id }
    $avatar = $heroAvatars | Where-Object { [int]$_.HeroId -eq [int]$h.Id }
    
    $heroList += [pscustomobject]@{
        Id           = [int]$h.Id
        Name         = $h.NameTag
        Class        = [int]$h.HeroClass
        ClassVice    = if ($h.HeroClassVice) { [int]$h.HeroClassVice } else { 0 }
        AssetPath    = $h.AssetData
        PortraitUrl  = $info.IllustrationUrl
        SkinResId    = $avatar.ResId
        SkinItemId   = if ($avatar.ItemId) { [long]$avatar.ItemId } else { 0 }
        ScoreRatios  = @{
            Gold       = [double]$h.ScoreGoldRatio
            DamageCaused   = [double]$h.ScoreDamageCausedRatio
            DamageTaken    = [double]$h.ScoreDamageTakenRatio
            Kill       = [double]$h.ScoreKillRatio
            Assist     = [double]$h.ScoreAssistRatio
            Death      = [double]$h.ScoreDeathRatio
            KDA        = [double]$h.ScoreKdaRatio
        }
    }
}

$heroList | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\heroes.json" -Encoding UTF8
Write-Host "  Extracted $($heroList.Count) heroes" -ForegroundColor Green

# ============================================================
# 2. EQUIPMENT DATA
# ============================================================
Write-Host "`n[2/7] Extracting Equipment..." -ForegroundColor Yellow

$equipRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\EquipmentData.bytes" -Raw
$equipLines = $equipRaw -split "`n" | Where-Object { $_ -match '^[0-9]{4},' }

$equipList = @()
foreach ($line in $equipLines) {
    $parts = $line -split ','
    if ($parts.Count -ge 42) {
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
            Components      = if ($parts[11]) { $parts[11] -split ' ' | ForEach-Object { [int]$_ } } else { @() }
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
# 3. EQUIPMENT PLANS (Recommended builds per hero)
# ============================================================
Write-Host "`n[3/7] Extracting Equipment Plans..." -ForegroundColor Yellow

$planRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\EquipmentPlanData.bytes" -Raw
$planLines = $planRaw -split "`n" | Where-Object { $_ -match '^[0-9]+,' }

$planList = @()
foreach ($line in $planLines) {
    $parts = $line -split ','
    if ($parts.Count -ge 5) {
        $planList += [pscustomobject]@{
            Id        = [int]$parts[0]
            HeroId    = [int]$parts[1]
            ItemId    = $parts[2]
            Name      = $parts[3]
            Items     = ($parts[4] -split ' ') | ForEach-Object { [int]$_ }
        }
    }
}

$planList | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\equipment_plans.json" -Encoding UTF8
Write-Host "  Extracted $($planList.Count) equipment plans" -ForegroundColor Green

# ============================================================
# 4. MAP / GAME MODE DATA
# ============================================================
Write-Host "`n[4/7] Extracting Map & Game Mode Data..." -ForegroundColor Yellow

$mapDef = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\MapDef.bytes" -Raw | ConvertFrom-Json
$mapDef | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\maps.json" -Encoding UTF8
Write-Host "  Extracted $($mapDef.Maps.Count) maps" -ForegroundColor Green

$summonerSpells = Parse-CSVData "$ExportPath\Assets\Resources\config\SummonerSpellData.bytes" @("Id", "Name", "AssetData", "UnlockLevel", "EffectImage")
$summonerSpells | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\summoner_spells.json" -Encoding UTF8
Write-Host "  Extracted $($summonerSpells.Count) summoner spells" -ForegroundColor Green

# ============================================================
# 5. DAMAGE MODIFIERS
# ============================================================
Write-Host "`n[5/7] Extracting Damage Modifiers..." -ForegroundColor Yellow

$dmgRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\GlobalDamageModifierData.bytes" -Raw
$dmgLines = $dmgRaw -split "`n" | Where-Object { $_ -match '^[0-9],' }

$dmgList = @()
foreach ($line in $dmgLines) {
    $parts = $line -split ','
    if ($parts.Count -ge 10) {
        $dmgList += [pscustomobject]@{
            Id     = [int]$parts[0]
            Type   = $parts[1]
            Modifiers = @{
                Hero    = [double]$parts[2]
                Monster = [double]$parts[3]
                Melee   = [double]$parts[4]
                Range   = [double]$parts[5]
                Cannon  = [double]$parts[6]
                Tower   = [double]$parts[7]
                Base    = [double]$parts[8]
                Spring  = [double]$parts[9]
            }
        }
    }
}

$dmgList | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\damage_modifiers.json" -Encoding UTF8
Write-Host "  Extracted $($dmgList.Count) damage modifier types" -ForegroundColor Green

# ============================================================
# 6. PAWN DATA (Creeps, Monsters, Guards)
# ============================================================
Write-Host "`n[6/7] Extracting Pawn Data..." -ForegroundColor Yellow

$pawnHeaders = @("Id", "Name", "PawnType", "AssetData")
$pawns = Parse-CSVData "$ExportPath\Assets\Resources\config\pawndata.bytes" $pawnHeaders
$pawns | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\pawns.json" -Encoding UTF8
Write-Host "  Extracted $($pawns.Count) pawn types" -ForegroundColor Green

# ============================================================
# 7. AIDER (PET) DATA
# ============================================================
Write-Host "`n[7/7] Extracting Aider (Pet) Data..." -ForegroundColor Yellow

$aiderHeaders = @("Id", "Remark", "Name", "Sort", "EffectDesc", "EffectBriefDesc", "AssetData", "BuffData", "UnlockDesc", "GoPos", "LobbyModel", "EffectImage", "SoundBank")
$aiders = Parse-CSVData "$ExportPath\Assets\Resources\config\Aider.bytes" $aiderHeaders

$aiderGemsRaw = Get-Content -LiteralPath "$ExportPath\Assets\Resources\config\AiderGemsData.bytes" -Raw
$aiderGemsLines = $aiderGemsRaw -split "`n" | Where-Object { $_ -match '^10[0-9]{6},' }

$aiderGems = @()
foreach ($line in $aiderGemsLines) {
    $parts = $line -split ','
    if ($parts.Count -ge 14) {
        $aiderGems += [pscustomobject]@{
            Id              = $parts[0]
            NameTag         = $parts[1]
            Name            = $parts[2]
            Icon            = $parts[3]
            Description     = $parts[4]
            BriefDesc       = $parts[5]
            AssetData       = $parts[6]
            Type            = $parts[7]
            SubType         = $parts[8]
            Quality         = $parts[9]
            IsUnique        = $parts[10]
            ReturnType      = $parts[11]
            ReturnId        = $parts[12]
            ReturnNum       = $parts[13]
        }
    }
}

@{
    Aiders = $aiders
    Gems   = $aiderGems
} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$OutputPath\aiders.json" -Encoding UTF8

Write-Host "  Extracted $($aiders.Count) aiders and $($aiderGems.Count) gems" -ForegroundColor Green

# ============================================================
# SUMMARY
# ============================================================
Write-Host "`n=== EXTRACTION COMPLETE ===" -ForegroundColor Green
Write-Host "Output files in: $OutputPath" -ForegroundColor Cyan
Get-ChildItem -LiteralPath $OutputPath | ForEach-Object { Write-Host "  $($_.Name) - $($_.Length) bytes" }

# Copy to Evo.MOBA Data folder if it exists
$targetDataPath = "F:\Evo_Core\samples\Evo.MOBA\src\Evo.MOBA\Data"
if (Test-Path -LiteralPath $targetDataPath) {
    Copy-Item -LiteralPath "$OutputPath\*" -Destination $targetDataPath -Force
    Write-Host "`nCopied to Evo.MOBA Data folder" -ForegroundColor Green
}