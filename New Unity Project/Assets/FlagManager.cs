using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance;
    [SerializeField] List<CaputrePointFlag> flagSpawns;
    [SerializeField] List<Flag> flags;

    public void Awake()
    {
        Instance = this;
        if (GameSettings.GameMode != GameMode.CTF)
        {
            foreach (CaputrePointFlag c in flagSpawns)
            {
                c.gameObject.SetActive(false);
            }
        }
    }
    public void TrySync()
    {
        foreach (Flag f in flags){
            f.TrySync();
        }
    }

}
