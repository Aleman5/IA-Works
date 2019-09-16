using System.IO;

public interface ISerializePacket
{
    ushort packetType { get; set; }

    void Serialize(Stream stream);
    void Deserialize(Stream stream);
}
