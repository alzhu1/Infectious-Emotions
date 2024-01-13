public enum UnitType {
    PLAYER_MAIN = 0,
    PLAYER_GROUP = 1,
    NPC = 2
}

static class UnitTypeHelpers {
    public static bool IsPlayerControlled(this UnitType unitType) {
        return (int)unitType < 2;
    } 
}