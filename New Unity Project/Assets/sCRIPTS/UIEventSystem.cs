using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIEventSystem : MonoBehaviour
{

    public static UIEventSystem current;

    private void Awake()
    {
        current = this;
    }

    public event Action<string> onFlagPickUp;
    public event Action<string> onFlagCapture;
    public event Action<string> onFlagDrop;
    public event Action<string> onFlagReturn;

    public void UIUpdateFlagPickUp(string _tag)
    {
        onFlagPickUp(_tag);
    }
    public void UIUpdateFlagCapture(string _tag)
    {
        onFlagCapture(_tag);
    }
    public void UIUpdateFlagDrop(string _tag)
    {
        onFlagDrop(_tag);
    }
    public void UIUpdateFlagReturn(string _tag)
    {
        onFlagReturn(_tag);
    }

    




}
