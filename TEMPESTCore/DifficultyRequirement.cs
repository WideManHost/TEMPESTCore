namespace TEMPESTCore
{
    [System.Flags]
    public enum DifficultyRequirement
    {
        None = 0,          
        Harmless = 1 << 0,     
        Lenient = 1 << 1,       
        Standard = 1 << 2,
        Violent = 1 << 3,      
        Brutal = 1 << 4,       
        UKMD = 1 << 5           
    }
}

