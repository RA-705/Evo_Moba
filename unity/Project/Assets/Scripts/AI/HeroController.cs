using UnityEngine;

namespace Evo.AI
{
    [RequireComponent(typeof(CharacterController))]
    public class HeroController : MonoBehaviour
    {
        [Header("Hero Identity")]
        public int HeroId;
        public int TeamId;
        public string HeroName;

        [Header("Stats")]
        public float MaxHealth = 1000f;
        public float CurrentHealth;
        public float MaxMana = 500f;
        public float CurrentMana;
        public float AttackDamage = 60f;
        public float AbilityPower = 0f;
        public float Armor = 30f;
        public float MagicResist = 30f;
        public float MoveSpeed = 5f;
        public float AttackRange = 2f;
        public float AttackSpeed = 1f;
        public int Level = 1;
        public float TotalGold;
        public int Kills;
        public int Deaths;
        public int Assists;

        [Header("State")]
        public bool IsAlive = true;
        public bool IsMoving;
        public bool IsAttacking;
        public Vector3 MoveTarget;
        public HeroController AttackTarget;
        public float LastAttackTime;

        [Header("Components")]
        public CharacterController Controller;

        private Renderer[] _renderers;
        private Renderer _indicatorRenderer;

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void Initialize(int heroId, int teamId)
        {
            HeroId = heroId;
            TeamId = teamId;
            IsAlive = true;

            var stats = Battle.HeroStatsDatabase.GetStats(heroId);
            if (stats != null)
            {
                MaxHealth = stats.BaseHP; CurrentHealth = MaxHealth;
                MaxMana = stats.BaseMP; CurrentMana = MaxMana;
                AttackDamage = stats.AttackDamage;
                AbilityPower = stats.AbilityPower;
                Armor = stats.Armor; MagicResist = stats.MagicResist;
                MoveSpeed = stats.MoveSpeed; AttackRange = stats.AttackRange;
                AttackSpeed = stats.AttackSpeed; HeroName = stats.Name;
            }
            else
            {
                HeroName = $"Hero_{heroId}";
                CurrentHealth = MaxHealth; CurrentMana = MaxMana;
            }
            gameObject.name = $"{HeroName}_T{teamId}";
            ApplyTeamColor();
        }

        private void ApplyTeamColor()
        {
            if (_renderers == null)
                _renderers = GetComponentsInChildren<Renderer>();
            Color teamColor = TeamId == 0 ? new Color(0.2f, 0.4f, 1f) : new Color(1f, 0.2f, 0.2f);
            foreach (var r in _renderers)
            {
                if (r == null) continue;
                if (r.name == "TeamIndicator" || r.name.Contains("Body") || r.name.Contains("Head"))
                {
                    var mat = r.material;
                    if (r.name == "TeamIndicator")
                        mat.color = teamColor;
                    else
                        mat.color = TeamId == 0
                            ? new Color(0.3f, 0.5f, 1f, 0.9f)
                            : new Color(1f, 0.3f, 0.3f, 0.9f);
                }
            }
        }

        public void MoveTo(Vector3 destination)
        {
            MoveTarget = destination; IsMoving = true;
        }

        public void StopMoving()
        {
            IsMoving = false;
        }

        public void Attack(HeroController target)
        {
            if (target == null || !target.IsAlive) return;
            AttackTarget = target; IsAttacking = true;
        }

        public void StopAttack()
        {
            IsAttacking = false; AttackTarget = null;
        }

        public void TakeDamage(float amount, HeroController attacker)
        {
            if (!IsAlive) return;
            float armorMultiplier = 1f - (Armor / (Armor + 100f));
            float actualDamage = amount * armorMultiplier;
            CurrentHealth = Mathf.Max(0, CurrentHealth - actualDamage);
            if (CurrentHealth <= 0) Die(attacker);
        }

        private void Die(HeroController killer)
        {
            IsAlive = false; IsMoving = false; IsAttacking = false;
            Deaths++;
            if (killer != null) { killer.Kills++; }
            Controller.enabled = false;
            foreach (var r in _renderers) if (r != null) r.enabled = false;
            Invoke(nameof(Respawn), 5f);
            Debug.Log($"{HeroName} (T{TeamId}) died. Respawn in 5s");
        }

        private void Respawn()
        {
            Vector3 spawnPos = TeamId == 0
                ? new Vector3(-20 + Random.Range(-2f, 2f), 0, -3 + Random.Range(-2f, 2f))
                : new Vector3(20 + Random.Range(-2f, 2f), 0, -3 + Random.Range(-2f, 2f));
            transform.position = spawnPos;
            CurrentHealth = MaxHealth; CurrentMana = MaxMana;
            IsAlive = true; Controller.enabled = true;
            foreach (var r in _renderers) if (r != null) r.enabled = true;
        }

        public bool CanAttack() =>
            IsAlive && Time.time - LastAttackTime >= 1f / Mathf.Max(AttackSpeed, 0.1f);

        public bool IsInAttackRange(HeroController target)
        {
            if (target == null) return false;
            return Vector3.Distance(transform.position, target.transform.position) <= AttackRange;
        }

        private void Update()
        {
            if (!IsAlive) return;

            if (IsMoving)
            {
                var dir = (MoveTarget - transform.position).normalized; dir.y = 0;
                if (dir.magnitude > 0.1f)
                {
                    Controller?.Move(dir * MoveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(dir), Time.deltaTime * 10f);
                }
                if (Vector3.Distance(transform.position, MoveTarget) < 0.5f)
                    IsMoving = false;
            }

            if (IsAttacking && AttackTarget != null && AttackTarget.IsAlive)
            {
                float dist = Vector3.Distance(transform.position, AttackTarget.transform.position);
                if (dist > AttackRange) MoveTo(AttackTarget.transform.position);
                else
                {
                    IsMoving = false;
                    if (CanAttack()) PerformAttack();
                }
            }

        }

        private void PerformAttack()
        {
            if (AttackTarget == null || !AttackTarget.IsAlive) return;
            LastAttackTime = Time.time;
            AttackTarget.TakeDamage(AttackDamage, this);
            transform.LookAt(AttackTarget.transform);
        }

        public float GetHealthPercent() => CurrentHealth / MaxHealth;
        public float GetManaPercent() => CurrentMana / MaxMana;
    }
}
