using System;
using System.Collections.Generic;
using UnityEngine;
using Evo.Battle;
using Evo.Client;

namespace Evo.Client
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class HeroEntity : NetworkEntity
    {
        [Header("Hero Settings")]
        public float MoveSpeed = 5f;
        public float TurnSpeed = 10f;
        public float AttackRange = 1.5f;
        public float AttackSpeed = 1f;
        public int Level { get; set; } = 1;
        public string HeroName { get; private set; }

        [Header("Components")]
        public CharacterController Controller;
        public Animator Animator;
        public Transform ModelRoot;
        public Transform WeaponAttachPoint;
        public Transform HandAttachPoint;

        [Header("Visual Effects")]
        public ParticleSystem MoveDustEffect;
        public ParticleSystem HitEffect;
        public ParticleSystem DeathEffect;
        public ParticleSystem LevelUpEffect;

        // State
        private Vector3 _moveDirection;
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private float _currentSpeed;
        private int _gold = 0;
        private int _kills = 0;
        private int _deaths = 0;
        private int _assists = 0;
        private Quaternion _targetRotation = Quaternion.identity;

        // Ability cooldowns
        private readonly Dictionary<int, float> _abilityCooldowns = new Dictionary<int, float>();

        void Awake()
        {
            Controller = GetComponent<CharacterController>();
            Animator = GetComponentInChildren<Animator>();
        }

        public new void Start()
        {
            var stats = HeroStatsDatabase.GetStats(HeroId);
            MaxHealth = stats.BaseHP;
            CurrentHealth = MaxHealth;
        }

        public override void Initialize(int entityId, int heroId, int teamId, string entityType)
        {
            base.Initialize(entityId, heroId, teamId, entityType);
            HeroId = heroId;

            LoadHeroData();
        }

        private void LoadHeroData()
        {
            switch (HeroId)
            {
                case 1: // Keth
                    HeroName = "Keth";
                    MoveSpeed = 5.5f;
                    break;
                case 2: // Cleo
                    HeroName = "Cleo";
                    MoveSpeed = 5f;
                    break;
                case 3: // Brunhild
                    HeroName = "Brunhild";
                    MoveSpeed = 4.8f;
                    break;
                default:
                    HeroName = $"Hero_{HeroId}";
                    MoveSpeed = 5f;
                    break;
            }
        }

        private void SetTeamColor(int teamId)
        {
            Color teamColor = teamId == 1 ? Color.blue : Color.red;
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                if (r.name.Contains("Team") || r.name.Contains("team"))
                {
                    var mat = new Material(r.material);
                    mat.color = teamColor;
                    r.material = mat;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!IsAlive) return;

            UpdateMovement();
            UpdateAbilityCooldowns();
        }

        private void UpdateMovement()
        {
            if (_moveDirection.magnitude > 0.1f)
            {
                _targetPosition = transform.position + _moveDirection * MoveSpeed * Time.deltaTime;

                if (Controller != null)
                {
                    Controller.Move(_moveDirection * MoveSpeed * Time.deltaTime);
                }

                if (_moveDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);
                }

                _isMoving = true;
                _currentSpeed = Mathf.Lerp(_currentSpeed, MoveSpeed, Time.deltaTime * 5f);
            }
            else
            {
                _isMoving = false;
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime * 10f);
            }

            if (Animator != null)
            {
                Animator.SetFloat("Speed", _currentSpeed / MoveSpeed);
                Animator.SetBool("IsMoving", _isMoving);
            }

            if (_isMoving && MoveDustEffect != null && !MoveDustEffect.isPlaying)
            {
                MoveDustEffect.Play();
            }
            else if (!_isMoving && MoveDustEffect != null && MoveDustEffect.isPlaying)
            {
                MoveDustEffect.Stop();
            }
        }

        public void SetMoveDirection(Vector3 direction)
        {
            _moveDirection = direction.normalized;
        }

        public void MoveTo(Vector3 targetPos)
        {
            _targetPosition = targetPos;
            _moveDirection = (targetPos - transform.position).normalized;
        }

        public void StopMoving()
        {
            _moveDirection = Vector3.zero;
        }

        public override void UpdateFromSnapshot(ServerPackets.EntitySnapshot snapshot)
        {
            base.UpdateFromSnapshot(snapshot);

            _targetPosition = snapshot.Position;
            _targetRotation = Quaternion.Euler(0, snapshot.Rotation, 0);
        }

        private void UpdateAbilityCooldowns()
        {
            var keys = new List<int>(_abilityCooldowns.Keys);
            foreach (var key in keys)
            {
                _abilityCooldowns[key] -= Time.deltaTime;
                if (_abilityCooldowns[key] <= 0)
                    _abilityCooldowns.Remove(key);
            }
        }

        public void CastAbility(int abilityId, Vector3 targetPosition)
        {
        }

        private void PlayAbilityEffect(int abilityId, Vector3 targetPosition)
        {
        }

        public void TakeDamage(float amount, int attackerId, bool isCrit = false)
        {
            if (!IsAlive) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

            if (HitEffect != null)
            {
                HitEffect.transform.position = transform.position + Vector3.up;
                HitEffect.Play();
            }

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        }

        public void AddGold(int amount) { _gold += amount; }
        public void AddKill() { _kills++; }
        public void AddDeath() { _deaths++; }
        public void AddAssist() { _assists++; }

        private void Die()
        {
            IsAlive = false;
            _deaths++;

            if (Animator != null)
            {
                Animator.SetTrigger("Die");
            }

            if (DeathEffect != null)
            {
                DeathEffect.transform.position = transform.position;
                DeathEffect.Play();
            }

            Controller.enabled = false;
            StopMoving();

            Invoke(nameof(Respawn), 5f);
        }

        private void Respawn()
        {
            Vector3 spawnPos = GetTeamSpawnPosition(TeamId);

            transform.position = spawnPos;
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
            IsAlive = true;
            Controller.enabled = true;

            if (LevelUpEffect != null)
            {
                LevelUpEffect.transform.position = transform.position;
                LevelUpEffect.Play();
            }
        }

        private Vector3 GetTeamSpawnPosition(int teamId)
        {
            return TeamId == 1 ? new Vector3(-20, 0, 0) : new Vector3(20, 0, 0);
        }

        public void LevelUp()
        {
            if (LevelUpEffect != null)
            {
                LevelUpEffect.transform.position = transform.position;
                LevelUpEffect.Play();
            }
        }
    }
}