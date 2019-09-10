public interface ISerializePacket
{
    ushort packetType { get; set; }

    void Serialize();
    void Deserialize();
}
