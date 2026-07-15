using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Abilities;

public struct PendingCastComponent : IComponent
{
    public int AbilityDataId;
    public CastTarget Target;
}

public static class AbilityRegistry
{
    public static readonly Dictionary<int, AbilityData> Database = new()
    {
        // === KETH (ID 1) - Tank ===
        [101] = new AbilityData { Id = 101, Name = "Sed de sangre", HeroId = 1, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 6f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 50, PhysicalScaling = 0f, Description = "Otorga velocidad y velocidad de ataque, cura al eliminar unidades" },
        [102] = new AbilityData { Id = 102, Name = "Escudo explosivo", HeroId = 1, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, ShieldAmount = 180, PhysicalScaling = 1.2f, Description = "Obtiene escudo que absorbe daño, explota si no es destruido" },
        [103] = new AbilityData { Id = 103, Name = "Golpe de desolación", HeroId = 1, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 140f, Radius = 3f, ManaCost = 70, PhysicalScaling = 1.15f, KnockupDuration = 0.75f, Description = "Lanza onda de choque, causa daño y derriba" },
        [104] = new AbilityData { Id = 104, Name = "Armadura oscura", HeroId = 1, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Bloquea daño de ataque" },

        // === CLEO (ID 2) - Warrior ===
        [201] = new AbilityData { Id = 201, Name = "Orden de vanguardia", HeroId = 2, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Acumula Vanguardia, 5 stacks causan daño adicional y derribo" },
        [202] = new AbilityData { Id = 202, Name = "Empuje", HeroId = 2, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 8f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 60, PhysicalScaling = 1.2f, SlowDuration = 1.5f, SlowAmount = 0.5f, Description = "Carga hacia objetivo, causa daño y ralentiza" },
        [203] = new AbilityData { Id = 203, Name = "Daño y ralentización en área", HeroId = 2, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 130f, Radius = 3f, ManaCost = 70, PhysicalScaling = 1.3f, SlowDuration = 1.5f, SlowAmount = 0.5f, Description = "Blande lanza, causa daño y ralentiza, acumula armadura" },
        [204] = new AbilityData { Id = 204, Name = "Carga imparable", HeroId = 2, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 50f, CastTime = 0f, Damage = 160f, Radius = 0f, ManaCost = 100, PhysicalScaling = 1.55f, Description = "Carga masiva, causa daño y derriba" },

        // === BRUNHILD (ID 3) - Warrior ===
        [301] = new AbilityData { Id = 301, Name = "Daño a distancia", HeroId = 3, Target = TargetType.Unit, CastType = CastType.Instant, Range = 5f, CooldownTime = 6f, CastTime = 0f, Damage = 150f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.0f, Description = "Ataca dos veces, cura si golpea" },
        [302] = new AbilityData { Id = 302, Name = "Desarme", HeroId = 3, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 10f, CastTime = 0f, Damage = 80f, Radius = 0f, ManaCost = 60, PhysicalScaling = 0.8f, SlowDuration = 2f, SlowAmount = 0.5f, Description = "Arrastra enemigo, causa daño y ralentiza" },
        [303] = new AbilityData { Id = 303, Name = "Dominación", HeroId = 3, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Revela enemigos, incrementa velocidad, puede aturdir" },
        [304] = new AbilityData { Id = 304, Name = "Asesino implacable", HeroId = 3, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Daño adicional a enemigos con baja vida, cura" },

        // === MIKAL (ID 4) - Tank ===
        [401] = new AbilityData { Id = 401, Name = "Escudo explosivo", HeroId = 4, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, ShieldAmount = 180, PhysicalScaling = 1.2f, Description = "Obtiene escudo que absorbe daño" },
        [402] = new AbilityData { Id = 402, Name = "Golpe de desolación", HeroId = 4, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 140f, Radius = 3f, ManaCost = 70, PhysicalScaling = 1.15f, KnockupDuration = 0.75f, Description = "Lanza onda de choque, causa daño y derriba" },
        [403] = new AbilityData { Id = 403, Name = "Armadura oscura", HeroId = 4, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Bloquea daño de ataque" },
        [404] = new AbilityData { Id = 404, Name = "Mordida con colmillo", HeroId = 4, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 5f, CastTime = 0f, Damage = 60f, Radius = 0f, ManaCost = 40, PhysicalScaling = 0.7f, Description = "Daño directo" },

        // === WINONA (ID 5) - Mage ===
        [501] = new AbilityData { Id = 501, Name = "Impacto arcano", HeroId = 5, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 5f, CastTime = 0f, Damage = 110f, Radius = 0f, ManaCost = 50, MagicScaling = 1.15f, Description = "Lanza onda mágica, causa daño" },
        [502] = new AbilityData { Id = 502, Name = "Singularidad", HeroId = 5, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 10f, CastTime = 0f, Damage = 80f, Radius = 0f, ManaCost = 60, MagicScaling = 0.65f, StunDuration = 1f, Description = "Dispara singularidad mágica, aturde" },
        [503] = new AbilityData { Id = 503, Name = "Baile mágico", HeroId = 5, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 4f, ManaCost = 70, Description = "Libera 5 llamas que atacan enemigos" },
        [504] = new AbilityData { Id = 504, Name = "Corrosión de alma", HeroId = 5, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Reduce resistencia mágica enemiga" },

        // === MOROS (ID 6) - Mage ===
        [601] = new AbilityData { Id = 601, Name = "El espectro", HeroId = 6, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, Description = "Invoca espectro que ataca enemigos" },
        [602] = new AbilityData { Id = 602, Name = "Eco abismal", HeroId = 6, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 130f, Radius = 3f, ManaCost = 70, MagicScaling = 0.7f, Description = "Causa daño a enemigos cercanos, mejora ataques" },
        [603] = new AbilityData { Id = 603, Name = "Augurio maldito", HeroId = 6, Target = TargetType.Point, CastType = CastType.Instant, Range = 10f, CooldownTime = 40f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Se teletransporta, causa daño持续" },
        [604] = new AbilityData { Id = 604, Name = "El espectro", HeroId = 6, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Tu ataque lanza proyectiles, hace daño mágico" },

        // === NIVAN (ID 7) - Mage ===
        [701] = new AbilityData { Id = 701, Name = "Pulso arcano", HeroId = 7, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 130f, Radius = 0f, ManaCost = 50, MagicScaling = 1.3f, Description = "Lanza pulso arcano, causa daño" },
        [702] = new AbilityData { Id = 702, Name = "Parálisis del sueño", HeroId = 7, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 100f, Radius = 3f, ManaCost = 60, MagicScaling = 0.75f, KnockupDuration = 1f, Description = "Crea círculo, causa daño y derriba" },
        [703] = new AbilityData { Id = 703, Name = "Anillo del nirvana", HeroId = 7, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 150f, Radius = 6f, ManaCost = 100, MagicScaling = 1.5f, Description = "Dispara rayo arcano masivo" },
        [704] = new AbilityData { Id = 704, Name = "Afinidad oculta", HeroId = 7, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Acumula Afinidad oculta, mejora habilidades" },

        // === TIGRIA (ID 8) - Marksman ===
        [801] = new AbilityData { Id = 801, Name = "Vendaval", HeroId = 8, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 6f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.2f, SlowDuration = 2f, SlowAmount = 0.3f, Description = "Causa daño y ralentiza" },
        [802] = new AbilityData { Id = 802, Name = "Salto del tigre", HeroId = 8, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, MoveSpeedBuff = 0.5f, Description = "Salta hacia objetivo, obtiene velocidad" },
        [803] = new AbilityData { Id = 803, Name = "Clarín de la jungla", HeroId = 8, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, MoveSpeedBuff = 0.3f, Description = "Aumenta velocidad de aliados" },
        [804] = new AbilityData { Id = 804, Name = "Tigre del viento", HeroId = 8, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Obtiene ataque físico según velocidad" },

        // === VALENA (ID 9) - Marksman ===
        [901] = new AbilityData { Id = 901, Name = "Crescendo", HeroId = 9, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 130f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.0f, Description = "Lanza cuchillas, causa daño" },
        [902] = new AbilityData { Id = 902, Name = "Inmunidad", HeroId = 9, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 12f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, MoveSpeedBuff = 0.8f, Description = "Gana velocidad y escudo" },
        [903] = new AbilityData { Id = 903, Name = "Escudo lunar", HeroId = 9, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, ShieldAmount = 100, Description = "Crea escudo lunar" },
        [904] = new AbilityData { Id = 904, Name = "Eclipse lunar", HeroId = 9, Target = TargetType.Point, CastType = CastType.Instant, Range = 10f, CooldownTime = 60f, CastTime = 0f, Damage = 150f, Radius = 4f, ManaCost = 100, PhysicalScaling = 0.8f, Description = "Crea eclipse, lanza rayos" },

        // === ARAMIS (ID 10) - Marksman ===
        [1001] = new AbilityData { Id = 1001, Name = "Cazador de la luna", HeroId = 10, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Ataques otorgan acumulaciones, mejoran velocidad" },
        [1002] = new AbilityData { Id = 1002, Name = "Salto y asedio", HeroId = 10, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 140f, Radius = 0f, ManaCost = 60, PhysicalScaling = 1.5f, Description = "Lanza gancho, causa daño y aturde" },
        [1003] = new AbilityData { Id = 1003, Name = "Apuntar y recargar", HeroId = 10, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 6f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 40, Description = "Próximo ataque dispara dos balas" },
        [1004] = new AbilityData { Id = 1004, Name = "Bomba verde", HeroId = 10, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 120f, Radius = 3f, ManaCost = 70, PhysicalScaling = 1.2f, SlowDuration = 1f, SlowAmount = 0.3f, Description = "Lanza explosivo, causa daño y ralentiza" },

        // === EIDA (ID 11) - Marksman ===
        [1101] = new AbilityData { Id = 1101, Name = "Proyectil del bosque", HeroId = 11, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 6f, CastTime = 0f, Damage = 160f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.6f, Description = "Lanza misil, causa daño masivo" },
        [1102] = new AbilityData { Id = 1102, Name = "Fuerza de la naturaleza", HeroId = 11, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Próximo ataque explota en área" },
        [1103] = new AbilityData { Id = 1103, Name = "Salto del mono", HeroId = 11, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, Description = "Salta hacia objetivo, reinicia enfriamiento" },
        [1104] = new AbilityData { Id = 1104, Name = "Fuerza de la naturaleza", HeroId = 11, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 140f, Radius = 3f, ManaCost = 70, PhysicalScaling = 1.3f, Description = "Ataque explosivo en área" },

        // === STYX (ID 12) - Assassin ===
        [1201] = new AbilityData { Id = 1201, Name = "Espada sagrada", HeroId = 12, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 6f, CastTime = 0f, Damage = 100f, Radius = 3f, ManaCost = 50, PhysicalScaling = 0.9f, KnockupDuration = 0.5f, Description = "Libera arma, causa daño y derriba" },
        [1202] = new AbilityData { Id = 1202, Name = "Avalancha", HeroId = 12, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 150f, Radius = 0f, ManaCost = 60, PhysicalScaling = 0.4f, KnockupDuration = 0.75f, Description = "Carga hacia objetivo, derriba" },
        [1203] = new AbilityData { Id = 1203, Name = "Bendición divina", HeroId = 12, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Bonificación que causa daño y reduce daño recibido" },
        [1204] = new AbilityData { Id = 1204, Name = "Profecía", HeroId = 12, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Cada tercer ataque causa daño adicional" },

        // === CLAWDIA (ID 13) - Assassin ===
        [1301] = new AbilityData { Id = 1301, Name = "Jaula de sombras", HeroId = 13, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 130f, Radius = 0f, ManaCost = 60, MagicScaling = 1.3f, StunDuration = 1.5f, Description = "Libera sello de sombras, aturde" },
        [1302] = new AbilityData { Id = 1302, Name = "Aura de tinieblas", HeroId = 13, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 12f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 70, ShieldAmount = 150, MagicScaling = 1.0f, Description = "Crea escudo para ella y aliado" },
        [1303] = new AbilityData { Id = 1303, Name = "Asedio sombrío", HeroId = 13, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 100f, Radius = 4f, ManaCost = 80, MagicScaling = 0.35f, SlowDuration = 5f, SlowAmount = 0.9f, Description = "Crea área oscura, ralentiza enemigos" },
        [1304] = new AbilityData { Id = 1304, Name = "Firma del prisionero", HeroId = 13, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Marcados reciben daño adicional" },

        // === MEMPHIS (ID 14) - Tank ===
        [1401] = new AbilityData { Id = 1401, Name = "Ente oscuro", HeroId = 14, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 50, Description = "Se vuelve invisible, aumenta velocidad" },
        [1402] = new AbilityData { Id = 1402, Name = "Loto afilado", HeroId = 14, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 105f, Radius = 3f, ManaCost = 60, PhysicalScaling = 1.05f, Description = "Crea abanico de cuchillas, inmune al daño" },
        [1403] = new AbilityData { Id = 1403, Name = "Condena de sombras", HeroId = 14, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 3f, ManaCost = 80, Description = "Salto grande, contraataca con cortes" },
        [1404] = new AbilityData { Id = 1404, Name = "Fases de la luna", HeroId = 14, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Mejora siguiente ataque, transporta y desgarra" },

        // === LOTH (ID 15) - Support ===
        [1501] = new AbilityData { Id = 1501, Name = "Toque de guerra", HeroId = 15, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 45f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, MoveSpeedBuff = 0.5f, Description = "Otorga velocidad a aliados cercanos" },
        [1502] = new AbilityData { Id = 1502, Name = "Toque de guerra", HeroId = 15, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 45f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, MoveSpeedBuff = 0.3f, Description = "Otorga velocidad a aliados cercanos" },
        [1503] = new AbilityData { Id = 1503, Name = "Expulsión divina", HeroId = 15, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Habilidad de control" },
        [1504] = new AbilityData { Id = 1504, Name = "Castigo guardián", HeroId = 15, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Habilidad definitiva" },

        // === ZOYA (ID 16) - Assassin ===
        [1601] = new AbilityData { Id = 1601, Name = "Salto del tigre", HeroId = 16, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.0f, Description = "Salta hacia objetivo" },
        [1602] = new AbilityData { Id = 1602, Name = "Clarín de la jungla", HeroId = 16, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, MoveSpeedBuff = 0.3f, Description = "Aumenta velocidad de aliados" },
        [1603] = new AbilityData { Id = 1603, Name = "Tigre del viento", HeroId = 16, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Obtiene ataque según velocidad" },
        [1604] = new AbilityData { Id = 1604, Name = "Crescendo", HeroId = 16, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 60f, CastTime = 0f, Damage = 160f, Radius = 0f, ManaCost = 100, PhysicalScaling = 1.5f, Description = "Lanza cuchillas, causa daño masivo" },

        // === REISA (ID 17) - Mage ===
        [1701] = new AbilityData { Id = 1701, Name = "Químico radiactivo", HeroId = 17, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 80f, Radius = 0f, ManaCost = 50, MagicScaling = 0.23f, SlowDuration = 6f, SlowAmount = 0.3f, Description = "Lanza botella, causa daño y ralentiza" },
        [1702] = new AbilityData { Id = 1702, Name = "Oscilador alquímico", HeroId = 17, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 130f, Radius = 3f, ManaCost = 60, MagicScaling = 0.7f, Description = "Coloca dispositivo, causa daño y quema" },
        [1703] = new AbilityData { Id = 1703, Name = "Homúnculo", HeroId = 17, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 40f, CastTime = 0f, Damage = 100f, Radius = 3f, ManaCost = 100, MagicScaling = 0.8f, StunDuration = 0.75f, Description = "Invoca Homúnculo, aturde y activa químicos" },
        [1704] = new AbilityData { Id = 1704, Name = "Piedra filosofal", HeroId = 17, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Genera oro adicional al matar" },

        // === HAGEN (ID 18) - Mage ===
        [1801] = new AbilityData { Id = 1801, Name = "Presagio apocalíptico", HeroId = 18, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 8f, CastTime = 0f, Damage = 115f, Radius = 0f, ManaCost = 50, MagicScaling = 1.15f, Description = "Carga láser, causa daño" },
        [1802] = new AbilityData { Id = 1802, Name = "Camino del caos", HeroId = 18, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 100f, Radius = 3f, ManaCost = 60, MagicScaling = 0.55f, StunDuration = 0.75f, Description = "Invoca caos, causa daño y aturde" },
        [1803] = new AbilityData { Id = 1803, Name = "Lluvia de almas", HeroId = 18, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 40f, CastTime = 0f, Damage = 150f, Radius = 0f, ManaCost = 100, MagicScaling = 0.95f, Description = "Lanza fragmentos de alma" },
        [1804] = new AbilityData { Id = 1804, Name = "Devoción oscura", HeroId = 18, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Obtiene ataque mágico permanente" },

        // === CHRISTINE (ID 19) - Assassin ===
        [1901] = new AbilityData { Id = 1901, Name = "Embestida del inquisidor", HeroId = 19, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 6f, CastTime = 0f, Damage = 155f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.55f, StunDuration = 0.5f, SlowDuration = 1.5f, SlowAmount = 0.3f, Description = "Embiste al objetivo, aturde y ralentiza" },
        [1902] = new AbilityData { Id = 1902, Name = "Golpe de desolación", HeroId = 19, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 140f, Radius = 3f, ManaCost = 60, PhysicalScaling = 1.15f, KnockupDuration = 0.75f, Description = "Lanza onda de choque" },
        [1903] = new AbilityData { Id = 1903, Name = "Armadura oscura", HeroId = 19, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Bloquea daño de ataque" },
        [1904] = new AbilityData { Id = 1904, Name = "Mordida con colmillo", HeroId = 19, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 60f, CastTime = 0f, Damage = 200f, Radius = 0f, ManaCost = 100, PhysicalScaling = 2.0f, Description = "Daño directo masivo" },

        // === RAVEN (ID 20) - Assassin ===
        [2001] = new AbilityData { Id = 2001, Name = "Vuelo del cuervo", HeroId = 20, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 100f, Radius = 0f, ManaCost = 50, PhysicalScaling = 0.9f, Description = "Lanza proyectil, recoge si golpea" },
        [2002] = new AbilityData { Id = 2002, Name = "Golpe del cuervo", HeroId = 20, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 7f, CastTime = 0f, Damage = 150f, Radius = 0f, ManaCost = 60, PhysicalScaling = 1.3f, Description = "Golpea enemigo" },
        [2003] = new AbilityData { Id = 2003, Name = "Segador", HeroId = 20, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 18f, CastTime = 0f, Damage = 280f, Radius = 0f, ManaCost = 80, PhysicalScaling = 2.6f, Description = "Golpe devastador" },
        [2004] = new AbilityData { Id = 2004, Name = "Cosecha", HeroId = 20, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 420f, Radius = 0f, ManaCost = 120, PhysicalScaling = 3.9f, Description = "Golpe de ejecución" },

        // === JUDITH (ID 21) - Support ===
        [2101] = new AbilityData { Id = 2101, Name = "Bomba verde", HeroId = 21, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 120f, Radius = 3f, ManaCost = 50, MagicScaling = 1.2f, SlowDuration = 1f, SlowAmount = 0.3f, Description = "Lanza explosivo, causa daño y ralentiza" },
        [2102] = new AbilityData { Id = 2102, Name = "Salto del mono", HeroId = 21, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, Description = "Salta hacia objetivo" },
        [2103] = new AbilityData { Id = 2103, Name = "Proyectil del bosque", HeroId = 21, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 6f, CastTime = 0f, Damage = 160f, Radius = 0f, ManaCost = 50, MagicScaling = 1.6f, Description = "Lanza misil, causa daño" },
        [2104] = new AbilityData { Id = 2104, Name = "Fuerza de la naturaleza", HeroId = 21, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 140f, Radius = 3f, ManaCost = 100, MagicScaling = 1.3f, Description = "Ataque explosivo en área" },

        // === ORSOUR (ID 22) - Warrior ===
        [2201] = new AbilityData { Id = 2201, Name = "Golpe de oso", HeroId = 22, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 6f, CastTime = 0f, Damage = 170f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.2f, Description = "Golpe en forma de oso" },
        [2202] = new AbilityData { Id = 2202, Name = "Rugido de oso", HeroId = 22, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 110f, Radius = 3f, ManaCost = 60, PhysicalScaling = 0.75f, Description = "Rugido que causa daño" },
        [2203] = new AbilityData { Id = 2203, Name = "Forma de oso", HeroId = 22, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 70, Description = "Transforma en oso, obtiene escudo" },
        [2204] = new AbilityData { Id = 2204, Name = "Furia del oso", HeroId = 22, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Aumenta daño y velocidad" },

        // === IDONY (ID 23) - Support ===
        [2301] = new AbilityData { Id = 2301, Name = "Bendición de primavera", HeroId = 23, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 50, HealAmount = 100, Description = "Cura a aliado" },
        [2302] = new AbilityData { Id = 2302, Name = "Escudo de flores", HeroId = 23, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, ShieldAmount = 120, Description = "Otorga escudo a aliado" },
        [2303] = new AbilityData { Id = 2303, Name = "Lluvia de pétalos", HeroId = 23, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 5f, ManaCost = 80, Description = "Cura a aliados cercanos" },
        [2304] = new AbilityData { Id = 2304, Name = "Jardín eterno", HeroId = 23, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Crea jardín que cura continuamente" },

        // === KYOUYA (ID 24) - Warrior ===
        [2401] = new AbilityData { Id = 2401, Name = "Corte rápido", HeroId = 24, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 5f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 40, PhysicalScaling = 1.0f, Description = "Corte rápido" },
        [2402] = new AbilityData { Id = 2402, Name = "Corte giratorio", HeroId = 24, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 100f, Radius = 3f, ManaCost = 50, PhysicalScaling = 0.8f, Description = "Corte giratorio en área" },
        [2403] = new AbilityData { Id = 2403, Name = "Corte descendente", HeroId = 24, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 12f, CastTime = 0f, Damage = 180f, Radius = 0f, ManaCost = 60, PhysicalScaling = 1.5f, Description = "Corte descendente poderoso" },
        [2404] = new AbilityData { Id = 2404, Name = "Danza de la muerte", HeroId = 24, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Danza que aumenta daño" },

        // === SCORPION (ID 25) - Marksman ===
        [2501] = new AbilityData { Id = 2501, Name = "Pinchazo", HeroId = 25, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 130f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.3f, Description = "Lanza pincho" },
        [2502] = new AbilityData { Id = 2502, Name = "Cola venenosa", HeroId = 25, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 8f, CastTime = 0f, Damage = 100f, Radius = 0f, ManaCost = 60, PhysicalScaling = 1.0f, SlowDuration = 2f, SlowAmount = 0.4f, Description = "Ataca con cola venenosa" },
        [2503] = new AbilityData { Id = 2503, Name = "Escorpión blindado", HeroId = 25, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 15f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 70, ShieldAmount = 150, Description = "Obtiene armadura" },
        [2504] = new AbilityData { Id = 2504, Name = "Picadura mortal", HeroId = 25, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 60f, CastTime = 0f, Damage = 200f, Radius = 0f, ManaCost = 100, PhysicalScaling = 2.0f, Description = "Ataque devastador" },

        // === NORAH (ID 26) - Tank ===
        [2601] = new AbilityData { Id = 2601, Name = "Embestida", HeroId = 26, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 8f, CastTime = 0f, Damage = 100f, Radius = 0f, ManaCost = 50, PhysicalScaling = 0.8f, StunDuration = 0.5f, Description = "Embestida que aturde" },
        [2602] = new AbilityData { Id = 2602, Name = "Golpe de escudo", HeroId = 26, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 80f, Radius = 3f, ManaCost = 60, PhysicalScaling = 0.6f, Description = "Golpe con escudo" },
        [2603] = new AbilityData { Id = 2603, Name = "Fortaleza", HeroId = 26, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, ShieldAmount = 200, Description = "Obtiene gran escudo" },
        [2604] = new AbilityData { Id = 2604, Name = "Ira de Norah", HeroId = 26, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Aumenta daño y resistencia" },

        // === SALOME (ID 27) - Mage ===
        [2701] = new AbilityData { Id = 2701, Name = "Flecha arcana", HeroId = 27, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 164f, Radius = 0f, ManaCost = 50, MagicScaling = 1.05f, Description = "Lanza flecha arcana" },
        [2702] = new AbilityData { Id = 2702, Name = "Explosión arcana", HeroId = 27, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 130f, Radius = 3f, ManaCost = 60, MagicScaling = 0.85f, Description = "Explosión arcana en área" },
        [2703] = new AbilityData { Id = 2703, Name = "Fascinación", HeroId = 27, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 12f, CastTime = 0f, Damage = 40f, Radius = 0f, ManaCost = 70, MagicScaling = 0.22f, StunDuration = 1f, Description = "Fascina al enemigo" },
        [2704] = new AbilityData { Id = 2704, Name = "Ojo de Salome", HeroId = 27, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Regenera mana y causa daño" },

        // === WULFRIC (ID 28) - Warrior ===
        [2801] = new AbilityData { Id = 2801, Name = "Embestida", HeroId = 28, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 8f, CastTime = 0f, Damage = 140f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.3f, Description = "Embestida física" },
        [2802] = new AbilityData { Id = 2802, Name = "Golpe de escudo", HeroId = 28, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 100f, Radius = 3f, ManaCost = 60, PhysicalScaling = 0.8f, Description = "Golpe con escudo" },
        [2803] = new AbilityData { Id = 2803, Name = "Furia del lobo", HeroId = 28, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Aumenta daño y velocidad" },
        [2804] = new AbilityData { Id = 2804, Name = "Aullido del lobo", HeroId = 28, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Aullido que afecta enemigos" },

        // === OBORO (ID 29) - Assassin ===
        [2901] = new AbilityData { Id = 2901, Name = "Corte sombrío", HeroId = 29, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 5f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 40, MagicScaling = 1.0f, Description = "Corte sombrío" },
        [2902] = new AbilityData { Id = 2902, Name = "Pasaje sombrío", HeroId = 29, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 100f, Radius = 0f, ManaCost = 50, MagicScaling = 0.8f, Description = "Se teletransporta" },
        [2903] = new AbilityData { Id = 2903, Name = "Sombra mortal", HeroId = 29, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 12f, CastTime = 0f, Damage = 180f, Radius = 0f, ManaCost = 60, MagicScaling = 1.5f, Description = "Ataque sombrío poderoso" },
        [2904] = new AbilityData { Id = 2904, Name = "Oportunidad", HeroId = 29, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Aumenta daño crítico" },

        // === ADELE (ID 30) - Marksman ===
        [3001] = new AbilityData { Id = 3001, Name = "Flecha precisa", HeroId = 30, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 6f, CastTime = 0f, Damage = 130f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.3f, Description = "Lanza flecha precisa" },
        [3002] = new AbilityData { Id = 3002, Name = "Disparo rápido", HeroId = 30, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, AttackSpeedBuff = 0.5f, Description = "Aumenta velocidad de ataque" },
        [3003] = new AbilityData { Id = 3003, Name = "Disparo penetrante", HeroId = 30, Target = TargetType.Unit, CastType = CastType.Instant, Range = 12f, CooldownTime = 15f, CastTime = 0f, Damage = 200f, Radius = 0f, ManaCost = 70, PhysicalScaling = 2.0f, Description = "Disparo que penetra" },
        [3004] = new AbilityData { Id = 3004, Name = "Lluvia de flechas", HeroId = 30, Target = TargetType.Point, CastType = CastType.Instant, Range = 10f, CooldownTime = 60f, CastTime = 0f, Damage = 150f, Radius = 5f, ManaCost = 100, PhysicalScaling = 1.5f, Description = "Lluvia de flechas en área" },

        // === JOEY (ID 31) - Support ===
        [3101] = new AbilityData { Id = 3101, Name = "Golpe de escudo", HeroId = 31, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 6f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.3f, Description = "Golpe con escudo" },
        [3102] = new AbilityData { Id = 3102, Name = "Protección", HeroId = 31, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 10f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, ShieldAmount = 160, Description = "Otorga escudo a aliado" },
        [3103] = new AbilityData { Id = 3103, Name = "Barrera", HeroId = 31, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, ShieldAmount = 300, Description = "Crea gran barrera" },
        [3104] = new AbilityData { Id = 3104, Name = "Contraataque", HeroId = 31, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 220f, Radius = 0f, ManaCost = 100, PhysicalScaling = 2.3f, Description = "Contraataque poderoso" },

        // === XENOS (ID 32) - Warrior ===
        [3201] = new AbilityData { Id = 3201, Name = "Embestida", HeroId = 32, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 6f, CastTime = 0f, Damage = 90f, Radius = 0f, ManaCost = 50, PhysicalScaling = 0.85f, Description = "Embestida física" },
        [3202] = new AbilityData { Id = 3202, Name = "Explosión", HeroId = 32, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 150f, Radius = 3f, ManaCost = 60, PhysicalScaling = 1.4f, Description = "Explosión en área" },
        [3203] = new AbilityData { Id = 3203, Name = "Corte", HeroId = 32, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 10f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 70, PhysicalScaling = 1.1f, Description = "Corte físico" },
        [3204] = new AbilityData { Id = 3204, Name = "Furia", HeroId = 32, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 200f, Radius = 0f, ManaCost = 80, PhysicalScaling = 1.9f, Description = "Ataque devastador" },

        // === GARSEA (ID 33) - Warrior ===
        [3301] = new AbilityData { Id = 3301, Name = "Golpe de espada", HeroId = 33, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 5f, CastTime = 0f, Damage = 110f, Radius = 0f, ManaCost = 40, PhysicalScaling = 1.0f, Description = "Golpe de espada" },
        [3302] = new AbilityData { Id = 3302, Name = "Corte circular", HeroId = 33, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 90f, Radius = 3f, ManaCost = 50, PhysicalScaling = 0.8f, Description = "Corte circular" },
        [3303] = new AbilityData { Id = 3303, Name = "Carga", HeroId = 33, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 12f, CastTime = 0f, Damage = 150f, Radius = 0f, ManaCost = 60, PhysicalScaling = 1.3f, Description = "Carga hacia objetivo" },
        [3304] = new AbilityData { Id = 3304, Name = "Furia de batalla", HeroId = 33, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Aumenta daño y velocidad" },

        // === AUSTEJA (ID 34) - Marksman ===
        [3401] = new AbilityData { Id = 3401, Name = "Flecha de miel", HeroId = 34, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 150f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.4f, Description = "Lanza flecha de miel" },
        [3402] = new AbilityData { Id = 3402, Name = "Enjambre", HeroId = 34, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 80f, Radius = 3f, ManaCost = 60, PhysicalScaling = 0.6f, SlowDuration = 2f, SlowAmount = 0.3f, Description = "Lanza enjambre" },
        [3403] = new AbilityData { Id = 3403, Name = "Escudo de cera", HeroId = 34, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 15f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 70, ShieldAmount = 120, Description = "Obtiene escudo de cera" },
        [3404] = new AbilityData { Id = 3404, Name = "Furia de abeja", HeroId = 34, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 200f, Radius = 0f, ManaCost = 100, PhysicalScaling = 1.7f, MoveSpeedBuff = 0.2f, Description = "Aumenta velocidad y causa daño" },

        // === CHLOE (ID 35) - Support ===
        [3501] = new AbilityData { Id = 3501, Name = "Burbuja mágica", HeroId = 35, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 50, ShieldAmount = 100, Description = "Crea burbuja para aliado" },
        [3502] = new AbilityData { Id = 3502, Name = "Habitación hiperespacial", HeroId = 35, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 12f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, Description = "Se vuelve invulnerable" },
        [3503] = new AbilityData { Id = 3503, Name = "Toque de guerra", HeroId = 35, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 45f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, MoveSpeedBuff = 0.5f, Description = "Otorga velocidad a aliados" },
        [3504] = new AbilityData { Id = 3504, Name = "Expulsión divina", HeroId = 35, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Habilidad definitiva de curación" },

        // === LARS (ID 36) - Tank ===
        [3601] = new AbilityData { Id = 3601, Name = "Embestida", HeroId = 36, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 6f, CastTime = 0f, Damage = 110f, Radius = 0f, ManaCost = 50, PhysicalScaling = 0.7f, KnockupDuration = 0.7f, Description = "Embestida que derriba" },
        [3602] = new AbilityData { Id = 3602, Name = "Golpe de escudo", HeroId = 36, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 150f, Radius = 3f, ManaCost = 60, PhysicalScaling = 1.0f, Description = "Golpe con escudo" },
        [3603] = new AbilityData { Id = 3603, Name = "Salto devastador", HeroId = 36, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 18f, CastTime = 0f, Damage = 200f, Radius = 3f, ManaCost = 80, PhysicalScaling = 1.0f, KnockupDuration = 1f, Description = "Salto que derriba" },
        [3604] = new AbilityData { Id = 3604, Name = "Bendición divina", HeroId = 36, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Otorga daño adicional y reducción" },

        // === NARAKU (ID 37) - Mage ===
        [3701] = new AbilityData { Id = 3701, Name = "Onda arcana", HeroId = 37, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 6f, CastTime = 0f, Damage = 130f, Radius = 3f, ManaCost = 50, MagicScaling = 0.6f, SlowDuration = 2f, SlowAmount = 0.3f, Description = "Lanza onda arcana" },
        [3702] = new AbilityData { Id = 3702, Name = "Explosión arcana", HeroId = 37, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 130f, Radius = 3f, ManaCost = 60, MagicScaling = 0.65f, Description = "Explosión arcana" },
        [3703] = new AbilityData { Id = 3703, Name = "Anillo del nirvana", HeroId = 37, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 20f, CastTime = 0f, Damage = 220f, Radius = 0f, ManaCost = 80, MagicScaling = 1.0f, Description = "Causa daño masivo" },
        [3704] = new AbilityData { Id = 3704, Name = "Devoción oscura", HeroId = 37, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 0f, CastTime = 0f, Damage = 0f, Radius = 0f, Description = "Obtiene ataque mágico permanente" },

        // === NATASHA (ID 38) - Mage ===
        [3801] = new AbilityData { Id = 3801, Name = "Escudo de sombras", HeroId = 38, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 50, ShieldAmount = 250, Description = "Obtiene escudo" },
        [3802] = new AbilityData { Id = 3802, Name = "Onda de sombras", HeroId = 38, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 110f, Radius = 3f, ManaCost = 60, MagicScaling = 0.5f, Description = "Lanza onda de sombras" },
        [3803] = new AbilityData { Id = 3803, Name = "Quemadura", HeroId = 38, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 15f, CastTime = 0f, Damage = 80f, Radius = 3f, ManaCost = 70, MagicScaling = 0.45f, Description = "Causa daño持续" },
        [3804] = new AbilityData { Id = 3804, Name = "Explosión final", HeroId = 38, Target = TargetType.Point, CastType = CastType.Instant, Range = 8f, CooldownTime = 30f, CastTime = 0f, Damage = 160f, Radius = 3f, ManaCost = 80, MagicScaling = 1.0f, Description = "Explosión final" },

        // === SOLO (ID 39) - Marksman ===
        [3901] = new AbilityData { Id = 3901, Name = "Disparo preciso", HeroId = 39, Target = TargetType.Unit, CastType = CastType.Instant, Range = 10f, CooldownTime = 6f, CastTime = 0f, Damage = 120f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.2f, Description = "Disparo preciso" },
        [3902] = new AbilityData { Id = 3902, Name = "Disparo rápido", HeroId = 39, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, AttackSpeedBuff = 0.5f, Description = "Aumenta velocidad de ataque" },
        [3903] = new AbilityData { Id = 3903, Name = "Disparo penetrante", HeroId = 39, Target = TargetType.Unit, CastType = CastType.Instant, Range = 12f, CooldownTime = 15f, CastTime = 0f, Damage = 180f, Radius = 0f, ManaCost = 70, PhysicalScaling = 1.8f, Description = "Disparo que penetra" },
        [3904] = new AbilityData { Id = 3904, Name = "Lluvia de balas", HeroId = 39, Target = TargetType.Point, CastType = CastType.Instant, Range = 10f, CooldownTime = 60f, CastTime = 0f, Damage = 140f, Radius = 5f, ManaCost = 100, PhysicalScaling = 1.4f, Description = "Lluvia de balas en área" },

        // === SARN (ID 40) - Warrior ===
        [4001] = new AbilityData { Id = 4001, Name = "Golpe fuerte", HeroId = 40, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 6f, CastTime = 0f, Damage = 140f, Radius = 0f, ManaCost = 50, PhysicalScaling = 1.2f, Description = "Golpe fuerte" },
        [4002] = new AbilityData { Id = 4002, Name = "Corte circular", HeroId = 40, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 120f, Radius = 3f, ManaCost = 60, PhysicalScaling = 1.0f, Description = "Corte circular" },
        [4003] = new AbilityData { Id = 4003, Name = "Carga", HeroId = 40, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 12f, CastTime = 0f, Damage = 180f, Radius = 0f, ManaCost = 70, PhysicalScaling = 1.5f, Description = "Carga hacia objetivo" },
        [4004] = new AbilityData { Id = 4004, Name = "Furia de batalla", HeroId = 40, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 40f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Aumenta daño y velocidad" },

        // === MINA (ID 41) - Tank ===
        [4101] = new AbilityData { Id = 4101, Name = "Escudo protector", HeroId = 41, Target = TargetType.Unit, CastType = CastType.Instant, Range = 8f, CooldownTime = 8f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 50, ShieldAmount = 150, Description = "Otorga escudo a aliado" },
        [4102] = new AbilityData { Id = 4102, Name = "Golpe de escudo", HeroId = 41, Target = TargetType.Unit, CastType = CastType.Instant, Range = 3f, CooldownTime = 10f, CastTime = 0f, Damage = 100f, Radius = 0f, ManaCost = 60, PhysicalScaling = 0.8f, StunDuration = 0.5f, Description = "Golpe con escudo que aturde" },
        [4103] = new AbilityData { Id = 4103, Name = "Barrera", HeroId = 41, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, ShieldAmount = 250, Description = "Crea gran barrera" },
        [4104] = new AbilityData { Id = 4104, Name = "Protección divina", HeroId = 41, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Protege a todos los aliados" },

        // === BERENICE (ID 42) - Tank ===
        [4201] = new AbilityData { Id = 4201, Name = "Embestida", HeroId = 42, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 8f, CastTime = 0f, Damage = 100f, Radius = 0f, ManaCost = 50, PhysicalScaling = 0.8f, StunDuration = 0.5f, Description = "Embestida que aturde" },
        [4202] = new AbilityData { Id = 4202, Name = "Golpe de escudo", HeroId = 42, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 10f, CastTime = 0f, Damage = 80f, Radius = 3f, ManaCost = 60, PhysicalScaling = 0.6f, Description = "Golpe con escudo" },
        [4203] = new AbilityData { Id = 4203, Name = "Fortaleza", HeroId = 42, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, ShieldAmount = 200, Description = "Obtiene gran escudo" },
        [4204] = new AbilityData { Id = 4204, Name = "Ira de Berenice", HeroId = 42, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 60f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 100, Description = "Aumenta daño y resistencia" },

        // === BRANDO (ID 44) - Tank ===
        [4401] = new AbilityData { Id = 4401, Name = "Embestida", HeroId = 44, Target = TargetType.Unit, CastType = CastType.Instant, Range = 6f, CooldownTime = 6f, CastTime = 0f, Damage = 80f, Radius = 0f, ManaCost = 40, PhysicalScaling = 0.6f, Description = "Embestida física" },
        [4402] = new AbilityData { Id = 4402, Name = "Golpe de escudo", HeroId = 44, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 8f, CastTime = 0f, Damage = 60f, Radius = 3f, ManaCost = 50, PhysicalScaling = 0.4f, SlowDuration = 2f, SlowAmount = 0.5f, Description = "Golpe con escudo, ralentiza" },
        [4403] = new AbilityData { Id = 4403, Name = "Fortaleza", HeroId = 44, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 12f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 60, Description = "Aumenta vida máxima" },
        [4404] = new AbilityData { Id = 4404, Name = "Ira de Brando", HeroId = 44, Target = TargetType.Self, CastType = CastType.Instant, Range = 0f, CooldownTime = 30f, CastTime = 0f, Damage = 0f, Radius = 0f, ManaCost = 80, Description = "Aumenta daño y resistencia" },
    };
}

public sealed class CooldownSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<AbilitySlotComponent>())
        {
            ref var slots = ref world.GetComponent<AbilitySlotComponent>(id);

            if (slots.Q.CurrentCooldown > 0f)
                slots.Q = slots.Q with { CurrentCooldown = Math.Max(0f, slots.Q.CurrentCooldown - deltaTime) };

            if (slots.W.CurrentCooldown > 0f)
                slots.W = slots.W with { CurrentCooldown = Math.Max(0f, slots.W.CurrentCooldown - deltaTime) };

            if (slots.E.CurrentCooldown > 0f)
                slots.E = slots.E with { CurrentCooldown = Math.Max(0f, slots.E.CurrentCooldown - deltaTime) };

            if (slots.R.CurrentCooldown > 0f)
                slots.R = slots.R with { CurrentCooldown = Math.Max(0f, slots.R.CurrentCooldown - deltaTime) };

            world.SetComponent(id, slots);
        }
    }
}

public sealed class AbilityValidationSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<PendingCastComponent>())
        {
            if (!world.TryGetComponent<PendingCastComponent>(id, out var pending) ||
                !world.TryGetComponent<AbilitySlotComponent>(id, out var slots))
            {
                world.RemoveComponent<PendingCastComponent>(id);
                continue;
            }

            if (!AbilityRegistry.Database.TryGetValue(pending.AbilityDataId, out var data))
            {
                world.RemoveComponent<PendingCastComponent>(id);
                continue;
            }

            AbilitySlot foundSlot = default;
            if (!slots.TryGetSlot(pending.AbilityDataId, ref foundSlot))
            {
                world.RemoveComponent<PendingCastComponent>(id);
                continue;
            }

            if (foundSlot.CurrentCooldown > 0f)
            {
                world.RemoveComponent<PendingCastComponent>(id);
                continue;
            }

            if (!world.TryGetComponent<PositionComponent>(id, out var casterPos))
            {
                world.RemoveComponent<PendingCastComponent>(id);
                continue;
            }

            var inRange = true;

            if (data.Target == TargetType.Point || data.Target == TargetType.Unit)
            {
                var targetPos = pending.Target.TargetPoint;

                if (pending.Target.IsUnit && pending.Target.TargetUnitId != Entity.Null.Id)
                {
                    if (world.TryGetComponent<PositionComponent>(pending.Target.TargetUnitId, out var unitPos))
                        targetPos = unitPos.Value;
                }

                if (EvoVector3.Distance(casterPos.Value, targetPos) > data.Range)
                    inRange = false;
            }

            if (!inRange)
            {
                world.RemoveComponent<PendingCastComponent>(id);
                continue;
            }

            world.AddComponent(
                new Entity(id),
                new CastStateComponent
                {
                    State = new ActiveCastState
                    {
                        AbilityDataId = pending.AbilityDataId,
                        Target = pending.Target,
                        TotalCastTime = data.CastTime,
                        ElapsedTime = 0f,
                    },
                });

            var updatedSlot = new AbilitySlot
            {
                DataId = foundSlot.DataId,
                CurrentCooldown = data.CooldownTime,
            };

            if (slots.Q.DataId == foundSlot.DataId) slots.Q = updatedSlot;
            else if (slots.W.DataId == foundSlot.DataId) slots.W = updatedSlot;
            else if (slots.E.DataId == foundSlot.DataId) slots.E = updatedSlot;
            else if (slots.R.DataId == foundSlot.DataId) slots.R = updatedSlot;

            world.SetComponent(id, slots);
            world.RemoveComponent<PendingCastComponent>(id);
        }
    }
}

public struct ProjectileComponent : IComponent
{
    public int AbilityDataId;
    public float Speed;
    public float Damage;
    public EntityId OwnerId;
    public CastTarget Target;
    public float MaxLifetime;
    public float CurrentLifetime;
}

public sealed class ProjectileMovementSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        var ids = world.GetEntityIds<ProjectileComponent>().ToList();

        foreach (var id in ids)
        {
            if (!world.TryGetComponent<PositionComponent>(id, out var pos) ||
                !world.TryGetComponent<ProjectileComponent>(id, out var projectile))
                continue;

            projectile.CurrentLifetime += deltaTime;

            if (projectile.CurrentLifetime >= projectile.MaxLifetime)
            {
                world.Destroy(new Entity(id));
                continue;
            }

            var targetPos = projectile.Target.TargetPoint;
            var direction = (targetPos - pos.Value).Normalized();
            pos.Value += direction * projectile.Speed * deltaTime;
            world.SetComponent(id, pos);

            if (EvoVector3.Distance(pos.Value, targetPos) < 0.5f)
            {
                if (world.TryGetComponent<TeamComponent>(id, out var projTeam) &&
                    AbilityRegistry.Database.TryGetValue(projectile.AbilityDataId, out var data))
                {
                    DamageHelper.ApplyAoE(world, projectile.Target.TargetPoint,
                        data.Radius, projectile.Damage, projTeam.TeamId, projectile.OwnerId);
                }

                world.Destroy(new Entity(id));
            }
        }
    }
}

public sealed class CastExecutionSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        var ids = world.GetEntityIds<CastStateComponent>().ToList();

        foreach (var id in ids)
        {
            ref var castState = ref world.GetComponent<CastStateComponent>(id);

            if (!castState.IsCasting)
            {
                world.RemoveComponent<CastStateComponent>(id);
                continue;
            }

            castState.State.ElapsedTime += deltaTime;

            if (!castState.State.IsComplete)
                continue;

            if (AbilityRegistry.Database.TryGetValue(castState.State.AbilityDataId, out var data))
            {
                if (castState.State.Target.IsUnit)
                {
                    if (world.TryGetComponent<HealthComponent>(castState.State.Target.TargetUnitId, out var targetHealth))
                    {
                        targetHealth.Current -= data.Damage;
                        world.SetComponent(castState.State.Target.TargetUnitId, targetHealth);
                    }
                }
                else
                {
                    if (world.TryGetComponent<PositionComponent>(id, out var casterPos))
                    {
                        var projectile = world.Create();
                        world.AddComponent(projectile, new PositionComponent { Value = casterPos.Value });
                        world.AddComponent(projectile, new ProjectileComponent
                        {
                            AbilityDataId = data.Id,
                            Speed = 20f,
                            Damage = data.Damage,
                            OwnerId = id,
                            Target = castState.State.Target,
                            MaxLifetime = 3f,
                            CurrentLifetime = 0f,
                        });

                        if (world.TryGetComponent<TeamComponent>(id, out var team))
                            world.AddComponent(projectile, new TeamComponent { TeamId = team.TeamId });
                    }
                }
            }

            world.RemoveComponent<CastStateComponent>(id);
        }
    }
}
