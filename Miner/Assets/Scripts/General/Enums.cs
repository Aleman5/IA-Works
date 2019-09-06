public enum EElement
{
    Miner,
    Soldier,
    Base,
    Mine,
    Ground,
    Obstacle,
    Count
}

public enum EAdyDirection
{
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft,
    Count
}

public enum ENodeAdyType
{
    Straight = 1, // Value = 1
    Diagonal = 2  // Value = 1.4
}

public enum ENodeValueMultipliers
{
    Normal = 1,
    Mud = 3,
    Danger = 2,
    Risky = 2,
}

public enum ENodeState
{
    Ok,
    Open,
    Close
}

public enum EPathfinderType
{
    BreadthFirst,
    DepthFirst,
    Dijkstra,
    Star,
    Count
}