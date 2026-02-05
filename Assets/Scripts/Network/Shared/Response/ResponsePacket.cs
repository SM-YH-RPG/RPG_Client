
public class ResponsePacket : IPacket
{
    public string PacketType { get; set; }

    public bool Success { get; set; }
    public ENetworkStatusCode Code { get; set; }
    public string Message { get; set; }

    public ResponsePacket() => PacketType = this.GetType().FullName;

    public virtual string GetResponseMessage()
    {
        return string.Empty;
    }
}