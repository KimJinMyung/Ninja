public static class MonsterDataMappingExtension
{
    public static Monster_data GetMonsterData(this DataManager manager, int dataId)
    {
        var loadMonsterList = manager.LoadedMonsterDataList;
        int monsterId = dataId + 1;
        if(loadMonsterList.Count == 0 || loadMonsterList.ContainsKey(monsterId) == false)
        {
            return null;
        }

        return loadMonsterList[monsterId];
    }

    public static Monster_Attack GetAttackMethodName(this DataManager manager, string attackName)
    {
        var loadAttackMethodList = manager.LoadedMonsterAttackList;
        if (loadAttackMethodList.Count == 0 || loadAttackMethodList.ContainsKey(attackName) == false)
        {
            return null;
        }

        return loadAttackMethodList[attackName];
    }
}
