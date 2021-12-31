using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum GameMode
{
    FFA=0,
    TDM=1,
    CTF=2
}

public enum GameMap
{
    Bloc=0,
    Cargo=1,
    Storage=2,
    Hinge=3,
    Presidio=4,
    ResearchLab=5,
    Office=6
}


public class GameSettings : MonoBehaviour
{
    public static GameMode GameMode = GameMode.FFA;
    public static GameMap GameMap=GameMap.Bloc;
    public static bool IsAwayTeam = false;
}
