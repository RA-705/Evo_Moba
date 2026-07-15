using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Evo.Client.ServerPackets;
using Evo.Client;
using Evo.UnityClient;

namespace Evo.Client
{
    public class NetworkManager : MonoBehaviour, INetEventListener
    {
        public static NetworkManager Instance { get; private set; }

        private NetManager _netManager;
        private NetPeer _serverPeer;
        private NetPacketProcessor _packetProcessor;

        private bool _isConnected = false;
        private readonly Queue<Action> _mainThreadActions = new Queue<Action>();
        private readonly object _queueLock = new object();

        public bool IsConnected => _isConnected && _serverPeer?.ConnectionState == ConnectionState.Connected;
        public NetPeer ServerPeer => _serverPeer;

        public event Action<NetPeer> OnPeerConnectedEvent;
        public event Action<NetPeer, DisconnectInfo> OnPeerDisconnectedEvent;

        public string ServerAddress { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 7777;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _netManager = new NetManager(this)
            {
                UpdateTime = 15,
                DisconnectTimeout = 10000,
                UnconnectedMessagesEnabled = true,
                NatPunchEnabled = false
                // SimulatePacketLoss = 0f,
                // SimulateLatency = 0f
            };

            _netManager.Start();
            RegisterPackets();
        }

        private void RegisterPackets()
        {
            _packetProcessor = new NetPacketProcessor();
            _packetProcessor.SubscribeReusable<ServerPackets.PlayerAssigned>(OnPlayerAssigned);
            _packetProcessor.SubscribeReusable<ServerPackets.MatchStateUpdate>(OnMatchStateUpdate);
            _packetProcessor.SubscribeReusable<ServerPackets.EntitySnapshot>(OnEntitySnapshot);
            _packetProcessor.SubscribeReusable<ServerPackets.MatchStart>(OnMatchStart);
            _packetProcessor.SubscribeReusable<ServerPackets.MatchEnd>(OnMatchEnd);
            _packetProcessor.SubscribeReusable<ServerPackets.AbilityCast>(OnAbilityCast);
            _packetProcessor.SubscribeReusable<ServerPackets.DamageDealt>(OnDamageDealt);
            _packetProcessor.SubscribeReusable<ServerPackets.ChatMessage>(OnChatMessage);
        }

        public async Task<bool> ConnectAsync(string address, int port, string appKey = "")
        {
            if (IsConnected) return true;

            ServerAddress = address;
            ServerPort = port;

            var tcs = new TaskCompletionSource<bool>();

            void OnConnected(NetPeer peer)
            {
                if (peer == _serverPeer)
                {
                    _isConnected = true;
                    tcs.TrySetResult(true);
                }
            }

            void OnDisconnected(NetPeer peer, DisconnectInfo info)
            {
                if (peer == _serverPeer)
                {
                    _isConnected = false;
                    _serverPeer = null;
                    tcs.TrySetResult(false);
                }
            }

            OnPeerConnectedEvent += OnConnected;
            OnPeerDisconnectedEvent += OnDisconnected;

            try
            {
                _serverPeer = _netManager.Connect(address, port, appKey);
                if (_serverPeer == null)
                {
                    Debug.LogError("[NetworkManager] Failed to initiate connection");
                    return false;
                }

                Debug.Log($"[NetworkManager] Connecting to {address}:{port}...");

                var timeoutTask = Task.Delay(10000);
                var completed = await Task.WhenAny(Task.Run(async () => { await tcs.Task; }), timeoutTask);

                if (completed == timeoutTask)
                {
                    Debug.LogError("[NetworkManager] Connection timeout");
                    _netManager.DisconnectAll();
                    return false;
                }

                return await tcs.Task;
            }
            finally
            {
                OnPeerConnectedEvent -= OnConnected;
                OnPeerDisconnectedEvent -= OnDisconnected;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            return await ConnectAsync(ServerAddress, ServerPort);
        }

        public void Disconnect()
        {
            if (_serverPeer != null)
            {
                _serverPeer.Disconnect();
                _serverPeer = null;
            }
            _isConnected = false;
        }

        public void Send<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, INetSerializable, new()
        {
            if (!IsConnected || _serverPeer == null) return;

            var writer = new NetDataWriter();
            _packetProcessor.Write(writer, packet);
            _serverPeer.Send(writer, method);
        }

        public void RegisterPacketHandler<T>(Action<T> handler) where T : class, INetSerializable, new()
        {
            _packetProcessor.SubscribeReusable(handler);
        }

        private void Update()
        {
            PollEvents();
        }

        public void PollEvents()
        {
            _netManager?.PollEvents();
            ProcessMainThreadQueue();
        }

        public void EnqueueMainThread(Action action)
        {
            lock (_queueLock)
            {
                _mainThreadActions.Enqueue(action);
            }
        }

        private void ProcessMainThreadQueue()
        {
            lock (_queueLock)
            {
                while (_mainThreadActions.Count > 0)
                {
                    var action = _mainThreadActions.Dequeue();
                    try { action(); }
                    catch (Exception e) { Debug.LogError($"[NetworkManager] Main thread action error: {e}"); }
                }
            }
        }

        #region INetEventListener

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log($"[NetworkManager] Connected to server: {peer.EndPoint}");
            _serverPeer = peer;
            _isConnected = true;
            OnPeerConnectedEvent?.Invoke(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"[NetworkManager] Disconnected: {disconnectInfo.Reason}");
            _isConnected = false;
            _serverPeer = null;
            OnPeerDisconnectedEvent?.Invoke(peer, disconnectInfo);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.LogError($"[NetworkManager] Network error: {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            _packetProcessor?.ReadAllPackets(reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }

        #endregion

        #region Packet Handlers

        private void OnPlayerAssigned(ServerPackets.PlayerAssigned pkt)
        {
        }

        private void OnMatchStateUpdate(ServerPackets.MatchStateUpdate pkt)
        {
        }

        private void OnEntitySnapshot(ServerPackets.EntitySnapshot pkt)
        {
            EntityManager.Instance?.UpdateEntitySnapshot(pkt);
        }

        private void OnMatchStart(ServerPackets.MatchStart pkt)
        {
            UIManager.Instance?.ShowHUD();
        }

        private void OnMatchEnd(ServerPackets.MatchEnd pkt)
        {
            UIManager.Instance?.ShowLobby();
        }

        private void OnAbilityCast(ServerPackets.AbilityCast pkt)
        {
        }

        private void OnDamageDealt(ServerPackets.DamageDealt pkt)
        {
        }

        private void OnChatMessage(ServerPackets.ChatMessage pkt)
        {
            UnityMainThreadDispatcher.Instance?.Enqueue(() => UIManager.Instance?.AddChatMessage(pkt.PlayerName, pkt.Message, (ChatChannel)pkt.Channel));
        }

        #endregion

        private void OnApplicationQuit()
        {
            Disconnect();
            _netManager?.Stop();
        }
    }
}