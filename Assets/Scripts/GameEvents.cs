
using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> TargetHit;

    public static void RaiseTargetHit(int score)
    {
        TargetHit?.Invoke(score);
    }
}
