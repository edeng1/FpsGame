﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public int levelToUnlock=0;
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
    public Texture2D gunImage;
    public Vector3 playerPoseitemPosition;
    public Vector3 playerPoseitemRotation;
    public Vector3 playerPoseitemScale;








}
