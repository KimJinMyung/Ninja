
public static class Player_UI_Extension
{
    public static void RegisterPlayerHPChanged(this Player_UI_ViewModel vm, bool isRegister)
    {
        PlayerManager.instance.BindHPChanged(vm.OnPlayerHpChanged, isRegister);
    }
    
    public static void OnPlayerHpChanged(this Player_UI_ViewModel vm, float HP)
    {
        vm.HP = HP;
    }
    public static void RegisterPlayerMaxHPChanged(this Player_UI_ViewModel vm, bool isRegister)
    {
        PlayerManager.instance.BindMaxHPChanged(vm.OnPlayerMaxHpChanged, isRegister);
    }

    public static void OnPlayerMaxHpChanged(this Player_UI_ViewModel vm, float maxHp)
    {
        vm.MaxHP = maxHp;
    }

    public static void RegisterPlayerStaminaChanged(this Player_UI_ViewModel vm, bool isRegister)
    {
        PlayerManager.instance.BindStaminaChanged(vm.OnPlayerStaminaiChanged, isRegister);
    }

    public static void OnPlayerStaminaiChanged(this Player_UI_ViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
    }
    public static void RegisterPlayerMaxStaminaChanged(this Player_UI_ViewModel vm, bool isRegister)
    {
        PlayerManager.instance.BindMaxStaminaChanged(vm.OnPlayerMaxStaminaiChanged, isRegister);
    }

    public static void OnPlayerMaxStaminaiChanged(this Player_UI_ViewModel vm, float maxStamina)
    {
        vm.MaxStamina = maxStamina;
    }
    public static void RegisterPlayerLifeCountChanged(this Player_UI_ViewModel vm, bool isRegister)
    {
        PlayerManager.instance.BindLifeCountChanged(vm.OnPlayerLifeCountChanged, isRegister);
    }

    public static void OnPlayerLifeCountChanged(this Player_UI_ViewModel vm, float LifeCount)
    {
        vm.Stamina = LifeCount;
    }
}
