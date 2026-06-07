namespace Combat.Core
{
    public enum DamageType
    {
        Slash,
        Pierce,
        Blunt
    }

    public enum GuardResult
    {
        None,
        Blocked,
        GuardBroken
    }

    public enum HitQuality
    {
        Miss,
        Glancing,
        Clean,
        Critical,
        Fatal
    }

    public enum CombatSide
    {
        Player,
        Enemy
    }
    
    public enum CombatState
    {
        Idle,
        Moving,
        Attacking,
        Blocking,
        Stepping,
        Recovering,
        Staggered,
        Dead
    }
}