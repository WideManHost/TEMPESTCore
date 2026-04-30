namespace TEMPESTCore
{
    [System.Flags]
    public enum DifficultyRequirement
    {
        None = 0,               // 000000 (No difficulties selected)
        Harmless = 1 << 0,      // 000001 (Value: 1)
        Lenient = 1 << 1,       // 000010 (Value: 2)
        Standard = 1 << 2,      // 000100 (Value: 4)
        Violent = 1 << 3,       // 001000 (Value: 8)
        Brutal = 1 << 4,        // 010000 (Value: 16)
        UKMD = 1 << 5           // 100000 (Value: 32)
    }
}

