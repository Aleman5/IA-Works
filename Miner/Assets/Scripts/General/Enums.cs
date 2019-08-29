public enum Element
{
    Base,
    Mine,
    Count
}

public enum AdyDirection
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

public enum NodeAdyType
{
    Straight = 1, // Value = 1
    Diagonal = 2  // Value = 1.4
}

public enum NodeValueMultipliers
{
    Normal = 1,
    Mud = 3,
    Danger = 2,
    Risky = 2,
}

public enum NodeState
{
    Ok,
    Open,
    Close
}

public enum PathfinderType
{
    BreadthFirst,
    DepthFirst,
    Star,
    Count
}