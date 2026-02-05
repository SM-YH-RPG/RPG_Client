public class RequestPacket : IPacket
{
    public string PacketType { get; set; }
 
    public RequestPacket() => PacketType = GetType().FullName;

    public virtual string GetResponseMessage()
    {
        return string.Empty;
    }
}