using System;

public class EventManager
{
    public static event EventHandler OnGameServiceInitialized;

    public void FireGameServiceInitialized()
    {
        OnGameServiceInitialized?.Invoke(this, EventArgs.Empty);
    }
}