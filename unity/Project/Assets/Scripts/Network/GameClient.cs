using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    private NetManager _netManager;
    private EventBasedNetListener _listener;
    private NetPeer _serverPeer;
    
    private Queue<byte[]> _snapshotQueue = new();
    private bool _isConnected;
    
    public static GameClient Instance { get; private set; }
    public bool IsConnected => _isConnected;
    
    public event Action<WorldSnapshot> OnSnapshotReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        InitializeNetwork();
    }
    
    private void InitializeNetwork()
    {
        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener)
        {
            AutoRecycle = true
        };
        
        _listener.ConnectionRequestEvent += request => request.AcceptIfKey("MOBA_KEY");
        _listener.PeerConnectedEvent += peer => OnPeerConnected(peer);
        _listener.PeerDisconnectedEvent += (peer, info) => OnPeerDisconnected();
        _listener.NetworkReceiveEvent += (peer, reader, channel, method) => OnNetworkReceive(reader);
        
        _netManager.Start();
    }
    
    public void ConnectToServer(string ip, int port)
    {
        _netManager.Connect(ip, port, "MOBA_KEY");
    }
    
    private void OnPeerConnected(NetPeer peer)
    {
        _serverPeer = peer;
        _isConnected = true;
        OnConnected?.Invoke();
        Debug.Log("[GameClient] Connected to server");
    }
    
    private void OnPeerDisconnected()
    {
        _isConnected = false;
        OnDisconnected?.Invoke();
        Debug.Log("[GameClient] Disconnected from server");
    }
    
    private void OnNetworkReceive(NetDataReader reader)
    {
        try
        {
            byte[] data = reader.GetRemainingBytes();
            _snapshotQueue.Enqueue(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[GameClient] Error receiving data: {ex.Message}");
        }
    }
    
    public void SendInput(byte inputType, int entityId, byte[] payload = null)
    {
        if (!_isConnected || _serverPeer == null) return;
        
        var writer = new NetDataWriter();
        writer.Put(inputType);
        writer.Put(entityId);
        if (payload != null)
            writer.Put(payload);
        
        _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
    
    public WorldSnapshot GetLatestSnapshot()
    {
        if (_snapshotQueue.Count == 0) return null;
        
        byte[] data = _snapshotQueue.Dequeue();
        return WorldSnapshot.Deserialize(data);
    }
    
    private void Update()
    {
        _netManager?.PollEvents();
        
        while (_snapshotQueue.Count > 0)
        {
            var snapshot = GetLatestSnapshot();
            if (snapshot != null)
            {
                OnSnapshotReceived?.Invoke(snapshot);
            }
        }
    }
    
    private void OnDestroy()
    {
        _netManager?.Stop();
    }
}
