using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HinesEventBus
{
    public static event Action<string> OnCombatEvent;
    public static void GlobalHinesEvent(string keyword)
    {
        OnCombatEvent?.Invoke(keyword);
    }
}
