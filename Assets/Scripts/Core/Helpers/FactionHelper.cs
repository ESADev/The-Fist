public static class FactionHelper
{
    public static FactionType GetEnemyOf(FactionType faction)
    {
        FactionType result;

        switch (faction)
        {
            case FactionType.Player:
                result = FactionType.Enemy;
                break;
            case FactionType.Enemy:
                result = FactionType.Player;
                break;
            default:
                result = FactionType.Enemy;
                break;
        }

        return result;
    }

    public static bool AreEnemies(FactionType a, FactionType b)
    {
        return a == GetEnemyOf(b);
    }
}