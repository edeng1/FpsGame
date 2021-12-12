using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName ="FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damageBody;
    public float damageArm;
    public float damageHead;
    public bool fullyAuto=false;
    public float fireRate = 10f;
    public float verticalRecoil=.5f;
    public float duration = .01f;
    public int totalAmmo;
    public int clipSize;
    public float reloadTime;
    public bool isPistol;// { get; private set; }
    







    }
