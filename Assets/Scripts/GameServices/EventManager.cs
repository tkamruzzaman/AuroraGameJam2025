//using UnityEngine;
using System;

public class EventManager //: MonoBehaviour
{
    public static event EventHandler  OnGameServiceInitialized;

public  void FireGameServiceInitialized()
    {
        OnGameServiceInitialized?.Invoke(this, EventArgs.Empty); 
    }

}