using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstData
{
}
public enum Tag
{
    Untagged,
    Player,
    Building,
    Node,
}

public enum BuildTag
{
    None,
    BedDouble,
    Chair,
    Table
}

public enum HomeStatus 
{
    None,
    Build,
    SelectBuildingStatus
}