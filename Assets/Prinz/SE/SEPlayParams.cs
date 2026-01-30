using UnityEngine;

public struct SEPlayParams
{
    public int? clipIndex;
    public bool loop;
    public bool ignoreCooldown;
    public bool ignoreFrameGuard;

    public static SEPlayParams Default => new SEPlayParams
    {
        clipIndex = null,
        loop = false, 
        ignoreCooldown = false, 
        ignoreFrameGuard = false
    };
}
