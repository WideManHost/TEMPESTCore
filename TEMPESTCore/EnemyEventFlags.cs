using System;
namespace TEMPESTCore
{
    [Flags]
    public enum EnemyEventFlags
    {
        None = 0,
        NotIfDead = 1 << 0,
        NotIfNoTarget = 1 << 1,
        NotIfSanded = 1 << 2,
        NotIfRadiant = 1 << 3,
        NotIfPuppeted = 1 << 4,
        NotIfBlind = 1 << 5,
        NotIfBlessed = 1 << 6,
        IfEnraged = 1 << 7,
        NotIfUnderwater = 1 << 8,
        NotIfHarpooned = 1 << 9,
        NotIfGasolined = 1 << 10,
        NotIfDrilled = 1 << 11
    }
}
