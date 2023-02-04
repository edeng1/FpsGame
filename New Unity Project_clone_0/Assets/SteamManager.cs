using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
/*
public class SteamManager : MonoBehaviour
{
    public uint appID;
    public static SteamManager Instance;
    Steamworks.InventoryResult? r;
    
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

            
            Photon.Pun.PhotonNetwork.NickName= Steamworks.SteamClient.Name;
            Steamworks.SteamInventory.OnInventoryUpdated += GiveItem;
            Steamworks.SteamInventory.OnDefinitionsUpdated+=GetDefinitions;
            Steamworks.SteamInventory.GenerateItemAsync(Steamworks.SteamInventory.FindDefinition(100),1);
            //Steamworks.SteamInventory.GetAllItems();
            Steamworks.SteamInventory.LoadItemDefinitions();

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
    public async Task GetItems()
    {
        r =await Steamworks.SteamInventory.GetAllItemsAsync();
        Debug.Log(r);
    }
    void GetDefinitions()
    {
        Steamworks.InventoryDef[] itemDefs=Steamworks.SteamInventory.Definitions;

        foreach (Steamworks.InventoryDef i in itemDefs)
        {
            Debug.Log(i.Id);

        }
    }

    void GiveItem(Steamworks.InventoryResult r)
    {
        Debug.Log(r.BelongsTo(Steamworks.SteamClient.SteamId));
        
       Steamworks.InventoryItem[] items= r.GetItems();
        foreach(Steamworks.InventoryItem i in items)
        {
            Debug.Log(i);
            
        }
    }
    void ListItems()
    {
        Steamworks.InventoryItem[] items = Steamworks.SteamInventory.Items;
        foreach (Steamworks.InventoryItem i in items)
        {
            Debug.Log(i);
        }
    }

    public void OpenShop()
    {
        Steamworks.SteamFriends.OpenWebOverlay("https://store.steampowered.com/itemstore/1863860/browse/?filter=all");
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

    */