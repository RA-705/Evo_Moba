using System.Threading.Tasks;
using UnityEngine;
using Evo.UnityClient;

namespace Evo.Client
{
    public class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (NetworkManager.Instance == null)
                new GameObject("NetworkManager").AddComponent<NetworkManager>();
            if (EntityManager.Instance == null)
                new GameObject("EntityManager").AddComponent<EntityManager>();
        }

        private async void Start()
        {
            Debug.Log("[GameBootstrap] Initialized");
        }

        public async Task<bool> ConnectToServer()
        {
            Debug.Log("[GameBootstrap] Connecting...");
            return true;
        }
    }
}
