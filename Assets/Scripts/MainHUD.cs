using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour
{
    [SerializeField] GameObject Prefab_MonsterHud;

    [Header("BossMonster")]
    [SerializeField] Image BossMonster_HPBar;
    [SerializeField] StaminaBar BossMonster_Stamina;
    [SerializeField] Text MonsterName;
    [SerializeField] Transform LifeCountIcon;

    List<Slot_MonsterHud> _monsterHUDSlotList = new List<Slot_MonsterHud>();

    private Canvas _thisCanvase;

    Image HP_BackGround;
    Image Stamina_Background, Stamina_Left, Stamina_Right;

    private void Awake()
    {
        _thisCanvase = GetComponent<Canvas>();
        HP_BackGround = BossMonster_HPBar.transform.parent.GetComponent<Image>();
        Stamina_Background = BossMonster_Stamina.GetComponent<Image>();
        Stamina_Left = BossMonster_Stamina.StaminaBarLeft;
        Stamina_Right = BossMonster_Stamina.StaminaBarRight;
    }

    private void OnEnable()
    {
        ViewBossHud(false);
    }

    private void OnDisable()
    {
        _monsterHUDSlotList.ForEach(e => DestroyImmediate(e.gameObject));
        _monsterHUDSlotList.Clear();
    }

    public void CreateMonsterHUD(Monster monster)
    {
        var gObj = Instantiate(Prefab_MonsterHud, this.transform);
        var hud = gObj.GetComponent<Slot_MonsterHud>();
        if (hud == null)
            return;

        hud.BindMonster(monster, _thisCanvase);
        _monsterHUDSlotList.Add(hud);
    }

    Monster BossMonster;
    Monster_data Boss_data;

    private bool isViewBossMonsterHud;

    public void BindBossMonster(Monster monster)
    {
        BossMonster = monster;
        Boss_data = BossMonster.MonsterViewModel.MonsterInfo;

        BossMonster_Stamina.SetMaxStamina(Boss_data.MaxStamina);
        BossMonster_Stamina.SetCurrentStamina(Boss_data.Stamina);

        MonsterName.text = Boss_data.Name;
    }

    public void OffMonsterHUD(Monster monster)
    {
        foreach (var slot in _monsterHUDSlotList)
        {
            if(slot._monster.monsterId == monster.monsterId)
            {
                slot.OnOffHud(false);       
                slot.gameObject.SetActive(false);
            }
        }
    }

    public void BossMonsterHud_OnOff(bool onOff)
    {
        isViewBossMonsterHud = onOff;
    }

    private void Update()
    {
        if(BossMonster == null)
        {
            return;
        }

        ViewBossHud(isViewBossMonsterHud);

        BossMonster_HPBar.fillAmount = (Boss_data.HP / Boss_data.MaxHP);
        BossMonster_Stamina.SetCurrentStamina(Boss_data.Stamina);
        
    }

    private void ViewBossHud(bool onOff)
    {
        Stamina_Background.enabled = onOff;
        Stamina_Left.enabled = onOff;
        Stamina_Right.enabled = onOff;
        BossMonster_Stamina.enabled = onOff;
        MonsterName.enabled = onOff;
        HP_BackGround.enabled = onOff;
        BossMonster_HPBar.enabled = onOff;

        if(!onOff) UpdateLifeCount(0);
        else UpdateLifeCount((int)Boss_data.Life);
    }

    private void UpdateLifeCount(int LifeCount)
    {
        int index = 0;
        foreach (Transform child in LifeCountIcon)
        {
            Image LifeCountIcon = child.GetComponent<Image>();

            if (LifeCountIcon == null) continue;

            if (index < LifeCount) LifeCountIcon.enabled = true;
            else LifeCountIcon.enabled = false;

            index++;
        }
    }
}
