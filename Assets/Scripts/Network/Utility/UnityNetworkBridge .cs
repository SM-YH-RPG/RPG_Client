using Cysharp.Threading.Tasks;
using System;
using System.Net;
using UnityEngine;

public class UnityNetworkBridge : MonoSingleton<UnityNetworkBridge>
{
    [SerializeField]
    private string _serverURL = "ec2-3-233-98-127.compute-1.amazonaws.com";

    [SerializeField]
    private int _serverPort = 8080;


    private NetworkClient _client;
    private PacketDispatcher _dispatcher;
    private bool _isRunning;
    private float _lastSnedPacketTime;
    public float LastSendPacketTime => _lastSnedPacketTime;

    public bool IsConnected 
    {
        get
        {
            return _isLocal ? false : _isRunning && _client.IsConnected; 
        } 
    }

    [SerializeField]
    private bool _isLocal = true;

    [SerializeField]
    private bool _isLocalServer = true;

    protected override void Awake()
    {
        base.Awake();

        if(_isLocal == false)
        {
            _dispatcher = new PacketDispatcher();
            _dispatcher.Initialize();

            _client = new NetworkClient();

            if(_isLocalServer)
            {
                ConnectAsync("127.0.0.1", _serverPort).Forget();
            }
            else
            {
                ConnectAsync(_serverURL, _serverPort).Forget();
            }
        }
    }

    private void OnDestroy()
    {
        _isRunning = false;
        _client.Disconnect();
    }

    /// <summary>
    /// 서버 연결
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    private async UniTask ConnectAsync(string host, int port)
    {
        try
        {
            await _client.ConnectAsync(host, port);
            _isRunning = true;

            ReceivePacketLoopAsync().Forget();
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection error: {e.Message}");
        }
    }

    /// <summary>
    /// 패킷 받아서 디스패처로 넘기는 부분
    /// </summary>
    /// <returns></returns>
    private async UniTask ReceivePacketLoopAsync()
    {
        while (_isRunning && _client.IsConnected)
        {
            try
            {
                string message = await _client.ReceivePacketAsync();

                if (!string.IsNullOrEmpty(message))
                {
                    await UniTask.SwitchToMainThread();
                    Debug.Log($"RECEVIE PACKET {message}");
                    _dispatcher.DispatchPacket(message);
                    await UniTask.SwitchToThreadPool();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in receive loop: {e.Message}");
                _isRunning = false;
            }
        }
    }


    public void SendPacket<T>(T packet) where T : IPacket
    {
        if (_client != null && _client.IsConnected)
        {
            _client.SendPacket(packet);
            _lastSnedPacketTime = Time.unscaledTime;
        }
    }
}