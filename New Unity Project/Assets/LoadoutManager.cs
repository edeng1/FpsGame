using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    [SerializeField]Object[] guns;
    [SerializeField] GameObject gunSelectPrefab;
   
    public static LoadoutManager Instance;
    
    private void Awake()
    {
        if (Instance)
        {
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        guns = Resources.LoadAll("Items/Guns", typeof(GunInfo));
        foreach (GunInfo g in guns)
        {
            if (!g.isPistol)
            {
                if (g.levelToUnlock<=RoomManager.playerData.level)
                {
                    Instantiate(gunSelectPrefab, transform.Find("ButtonContainer")).GetComponent<GunListItem>().SetUp(g);

                    
                }
            }
               
        }   
    }
    


}
