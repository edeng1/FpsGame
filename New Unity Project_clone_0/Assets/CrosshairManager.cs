using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{

    public static CrosshairManager Instance;
    [SerializeField] Object[] crosshairs;
    [SerializeField] RectTransform ButtonContainer;
    [SerializeField] GameObject crosshairSelectPrefab;
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

        crosshairs = Resources.LoadAll("CrosshairPack", typeof(Texture2D));
        foreach (Texture2D c in crosshairs)
        {

            Instantiate(crosshairSelectPrefab, ButtonContainer).GetComponent<CrosshairListItem>().SetUp(c);
            


        }
    }
}
