
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

    public static void RegisterPlayerStaminaChanged(this Player_UI_ViewModel vm, bool isRegister)
    {
        PlayerManager.instance.BindStaminaChanged(vm.OnPlayerStaminaiChanged, isRegister);
    }

    public static void OnPlayerStaminaiChanged(this Player_UI_ViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
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
