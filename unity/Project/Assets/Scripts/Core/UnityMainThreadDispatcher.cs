using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evo.Client
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        public static UnityMainThreadDispatcher Instance { get; private set; }

        private readonly Queue<Action> _actions = new Queue<Action>();
        private readonly object _lock = new object();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Enqueue(Action action)
        {
            if (action == null) return;
            lock (_lock) { _actions.Enqueue(action); }
        }

        private void Update()
        {
            lock (_lock)
            {
                while (_actions.Count > 0)
                {
                    try { _actions.Dequeue()(); }
                    catch (Exception e) { Debug.LogError($"[Dispatcher] {e}"); }
                }
            }
        }
    }
}
