using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkClient
{
    private Socket _socket;
    private bool _isRunning = false;

    private MemoryStream _receiveBuffer = new MemoryStream();
    private const int HeaderSize = 4;

    public bool IsConnected => _socket != null && _socket.Connected && _isRunning;

    public async UniTask ConnectAsync(string host, int port)
    {
        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(host, port);
            _isRunning = true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Connection error: {e.Message}");
            throw;
        }
    }

    public void Disconnect()
    {
        _isRunning = false;
        if (_socket != null)
        {
            _socket.Close();
            _socket = null;
        }
    }

    private bool HasCompletePacket(MemoryStream buffer, out string jsonMessage)
    {
        jsonMessage = null;
        if (buffer.Length < HeaderSize)
        {
            return false;
        }

        buffer.Position = 0;
        byte[] headerBytes = new byte[HeaderSize];
        buffer.Read(headerBytes, 0, HeaderSize);

        int packetLength = BitConverter.ToInt32(headerBytes, 0);

        if (buffer.Length >= HeaderSize + packetLength)
        {
            byte[] bodyBytes = new byte[packetLength];
            buffer.Read(bodyBytes, 0, packetLength);
            jsonMessage = Encoding.UTF8.GetString(bodyBytes);

            long remainingLength = buffer.Length - (HeaderSize + packetLength);
            if (remainingLength > 0)
            {
                byte[] remainingBytes = new byte[remainingLength];
                buffer.Read(remainingBytes, 0, (int)remainingLength);
                buffer.SetLength(0);
                buffer.Write(remainingBytes, 0, (int)remainingLength);
            }
            else
            {
                buffer.SetLength(0);
            }

            return true;
        }

        buffer.Position = buffer.Length;
        return false;
    }


    public async UniTask<string> ReceivePacketAsync()
    {
        if (!_isRunning || _socket == null || !_socket.Connected)
        {
            return null;
        }

        try
        {
            while (_socket.Connected)
            {
                byte[] tempBuff = new byte[4096];
                int bytesRead = await _socket.ReceiveAsync(new Memory<byte>(tempBuff), SocketFlags.None);

                if (bytesRead == 0)
                {
                    Disconnect();
                    return null; // 연결 종료
                }

                _receiveBuffer.Write(tempBuff, 0, bytesRead);
                while (HasCompletePacket(_receiveBuffer, out string completeJson))
                {
                    return completeJson;
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Socket exception: {e.Message}");
            Disconnect();
        }
        catch (Exception e)
        {
            Console.WriteLine($"General receive error: {e.Message}");
            Disconnect();
        }

        return null;
    }

    public void SendPacket<T>(T packet) where T : IPacket
    {
        if (IsConnected)
        {
            string jsonPayload = JsonConvert.SerializeObject(packet);
            Debug.Log($"jsonpayload : {jsonPayload}");

            byte[] buffer = Encoding.UTF8.GetBytes(jsonPayload);
            _socket.Send(buffer);
        }
    }
}