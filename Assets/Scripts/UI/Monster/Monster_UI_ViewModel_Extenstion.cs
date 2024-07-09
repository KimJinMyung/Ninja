public static class Monster_UI_ViewModel_Extenstion
{
    public static void RegisterMonsterHPChanged(this Monster_UI_ViewModel vm, bool isRegister, int id)
    {
        MonsterManager.instance.RegisterMonsterHPChangedCallback(id, vm.OnMonsterHPChanged, isRegister);
    }
    public static void OnMonsterHPChanged(this Monster_UI_ViewModel vm, float HP)
    {
        vm.HP = HP;
    }
    public static void RegisterMonsterMaxHPChanged(this Monster_UI_ViewModel vm, bool isRegister, int id)
    {
        MonsterManager.instance.RegisterMonsterMaxHPChangedCallback(id, vm.OnMonsterMaxHPChanged, isRegister);
    }
    public static void OnMonsterMaxHPChanged(this Monster_UI_ViewModel vm, float maxHP)
    {
        vm.MaxHP = maxHP;
    }
    public static void RegisterMonsterStaminaChanged(this Monster_UI_ViewModel vm, bool isRegister, int id)
    {
        MonsterManager.instance.RegisterMonsterStaminaChangedCallback(id, vm.OnMonsterStaminaChanged, isRegister);
    }
    public static void OnMonsterStaminaChanged(this Monster_UI_ViewModel vm, float stmina)
    {
        vm.Stamina = stmina;
    }
    public static void RegisterMonsterMaxStaminaChanged(this Monster_UI_ViewModel vm, bool isRegister, int id)
    {
        MonsterManager.instance.RegisterMonsterMaxStaminaChangedCallback(id, vm.OnMonsterMaxStaminaChanged, isRegister);
    }
    public static void OnMonsterMaxStaminaChanged(this Monster_UI_ViewModel vm, float maxStamina)
    {
        vm.MaxStamina = maxStamina;
    }
    public static void RegisterMonsterLifeCountChanged(this Monster_UI_ViewModel vm, bool isRegister, int id)
    {
        MonsterManager.instance.RegisterMonsterMaxStaminaChangedCallback(id, vm.OnMonsterLifeCountChanged, isRegister);
    }
    public static void OnMonsterLifeCountChanged(this Monster_UI_ViewModel vm, float LifeCount)
    {
        vm.LifeCount = LifeCount;
    }
}
