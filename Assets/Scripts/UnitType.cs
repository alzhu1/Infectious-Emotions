public enum UnitType {
    PLAYER_MAIN = 0,
    PLAYER_LOVE = 1,
    PLAYER_HATE = 2,
    PLAYER_ENVY = 3,
    NPC = 4
}

static class UnitTypeHelpers {
    public static bool IsPlayerControlled(this UnitType unitType) {
        return (int)unitType < 4;
    } 
}