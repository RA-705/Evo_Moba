using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Evo.Core.ECS;
using Evo.Shared.Networking;

namespace Evo.MOBA.Networking;

public sealed class GameNetServer : INetEventListener
{
    private readonly NetManager _netManager;
    private readonly List<NetPeer> _peers = new();

    public int PeerCount => _peers.Count;
    public event Action<NetPeer, byte[]>? OnInputReceived;

    public GameNetServer()
    {
        _netManager = new NetManager(this)
        {
            UnconnectedMessagesEnabled = false,
            UpdateTime = 15,
        };
    }

    public void Start(int port)
    {
        _netManager.Start(port);
        Console.WriteLine($"[Server] Listening on port {port}");
        Console.Out.Flush();
    }

    public void PollEvents() =>
        _netManager.PollEvents();

    public void BroadcastSnapshot(World world, uint currentTick)
    {
        if (_peers.Count == 0) return;

        var snapshot = SnapshotGenerator.GenerateSnapshot(world, currentTick);
        var data = SnapshotSerializer.Serialize(snapshot);

        for (int i = _peers.Count - 1; i >= 0; i--)
        {
            var peer = _peers[i];
            if (peer.ConnectionState == ConnectionState.Connected)
                peer.Send(data, DeliveryMethod.Unreliable);
            else
                _peers.RemoveAt(i);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _peers.Add(peer);
        Console.WriteLine($"[Server] Client connected: {peer} (total: {_peers.Count})");
        Console.Out.Flush();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _peers.Remove(peer);
        Console.WriteLine($"[Server] Client disconnected: {disconnectInfo.Reason} (total: {_peers.Count})");
        Console.Out.Flush();
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Console.WriteLine($"[Server] Network error: {socketError} from {endPoint}");
        Console.Out.Flush();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var data = reader.GetRemainingBytes();
        OnInputReceived?.Invoke(peer, data);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        Console.WriteLine($"[Server] Connection request from {request.RemoteEndPoint}");
        Console.Out.Flush();
        request.AcceptIfKey("");
    }
}
