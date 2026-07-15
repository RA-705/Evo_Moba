using System.Collections.Generic;
using UnityEngine;

namespace Evo.UnityClient
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        public GameObject DamageNumberPrefab;
        public GameObject HealNumberPrefab;
        public GameObject CriticalEffectPrefab;
        public GameObject DeathEffectPrefab;
        public GameObject LevelUpEffectPrefab;
        public GameObject AbilityHitPrefab;
        public GameObject ProjectilePrefab;

        private readonly Queue<GameObject> _damageNumberPool = new Queue<GameObject>();
        private readonly Queue<GameObject> _projectilePool = new Queue<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            WarmupPool(DamageNumberPrefab, _damageNumberPool, 20);
            WarmupPool(ProjectilePrefab, _projectilePool, 30);
        }

        private void WarmupPool(GameObject prefab, Queue<GameObject> pool, int count)
        {
            if (prefab == null) return;

            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(prefab, transform);
                go.SetActive(false);
                pool.Enqueue(go);
            }
        }

        public void ShowDamageNumber(Vector3 position, float amount, bool isCritical = false, bool isHeal = false)
        {
            var pool = _damageNumberPool;
            var prefab = isHeal ? HealNumberPrefab : (isCritical ? CriticalEffectPrefab : DamageNumberPrefab);

            if (prefab == null) return;

            GameObject effect;
            if (pool.Count > 0)
            {
                effect = pool.Dequeue();
            }
            else
            {
                effect = Instantiate(prefab, transform);
            }

            effect.transform.position = position + Vector3.up * 2f;
            effect.SetActive(true);

            StartCoroutine(ReturnToPoolAfterDelay(effect, pool, 1.5f));
        }

        public void PlayDeathEffect(Vector3 position, int heroId)
        {
            if (DeathEffectPrefab == null) return;
            var effect = Instantiate(DeathEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        public void PlayLevelUpEffect(Vector3 position)
        {
            if (LevelUpEffectPrefab == null) return;
            var effect = Instantiate(LevelUpEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject effect, Queue<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            effect.SetActive(false);
            pool.Enqueue(effect);
        }
    }
}
