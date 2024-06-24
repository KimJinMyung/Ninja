public static class PlayerData_Extension
{
    public static Player_data GetPlayerData(this DataManager manager, int playerId)
    {
        var loadPlayerData = manager.LoadPlayerData;
        int index = playerId + 1;
        if (loadPlayerData.Count == 0 || loadPlayerData.ContainsKey(index) == false)
        {
            return null;
        }

        return loadPlayerData[index];
    }
}
