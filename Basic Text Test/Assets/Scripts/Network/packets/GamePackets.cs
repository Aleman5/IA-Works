using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePackets : NetworkPacket<GamePackets>
{
    public GamePackets() : base(PacketType.Message)
    {

    }
}
