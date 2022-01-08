using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    public uint appID;
    public static SteamManager Instance;

    void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
        try
        {
            Steamworks.SteamClient.Init(appID);
            
        }
        catch (System.Exception e)
        {
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //
            Debug.LogError(e);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void OnDisable()
    {
        try
        {
            Steamworks.SteamClient.Shutdown();
        }
        catch { }
        
    }
}

