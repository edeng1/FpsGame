using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance;
    [SerializeField] List<Flag> flags;

    public void Awake()
    {
        Instance = this;
    }
    public void TrySync()
    {
        foreach (Flag f in flags){
            f.TrySync();
        }
    }

}
