using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot_MonsterHud : MonoBehaviour
{
    [SerializeField] Text Text_MonsterName;
    [SerializeField] Image Image_MonsterHP;
    [SerializeField] StaminaBar MonsterStamina;

    public Monster _monster { get; private set; } = null;
    private Monster_data monster_data;

    private Image Image_HPBackground;
    private Image Image_Stamina;
    private Image Image_Stamina_left;
    private Image Image_Stamina_Right;

    private RectTransform MonsterHud_Panel;
    private Canvas _thisCanvas;

    private void Awake()
    {
        Image_Stamina = MonsterStamina.GetComponent<Image>();
        Image_Stamina_left = MonsterStamina.transform.GetChild(0).GetComponent<Image>();
        Image_Stamina_Right = MonsterStamina.transform.GetChild(1).GetComponent<Image>();
        Image_HPBackground = Image_MonsterHP.transform.parent.GetComponent<Image>();
    }

    private void OnEnable()
    {
        MonsterHud_Panel = Image_MonsterHP.transform.parent.parent.GetComponent<RectTransform>();
    }

    public void BindMonster(Monster monster, Canvas canvas)
    {
        _monster = monster;
        _thisCanvas = canvas;

        monster_data = _monster.MonsterViewModel.MonsterInfo;

        MonsterStamina.SetMaxStamina(monster_data.MaxStamina);
        MonsterStamina.SetCurrentStamina(monster_data.Stamina);
    }

    public void OnOffHud(bool onAlbe)
    {
        Image_Stamina.enabled = onAlbe;
        Image_Stamina_left.enabled = onAlbe;
        Image_Stamina_Right.enabled = onAlbe;
        MonsterStamina.enabled = onAlbe;
        Text_MonsterName.enabled = onAlbe;
        Image_MonsterHP.enabled = onAlbe;
        Image_HPBackground.enabled = onAlbe;
    }

    private void Update()
    {
        if(_monster == null || _monster.MonsterViewModel == null)
        {
            OnOffHud(false);
            return;
        }

        if (DetectMonster())
        {
            Vector3 ScreenPos = Camera.main.WorldToScreenPoint(_monster.transform.position + Vector3.up * _monster.monsterHeight);

            if (ScreenPos.z > 0)
            {
                Vector2 canvasPosition;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _thisCanvas.transform as RectTransform,
                    ScreenPos,
                    _thisCanvas.worldCamera,
                    out canvasPosition);

                MonsterHud_Panel.anchoredPosition = canvasPosition;
                OnOffHud(true);
            }
            else
            {
                OnOffHud(false);
            }
        }
        else
        {
            OnOffHud(false);
        }
        

        Image_MonsterHP.fillAmount = (_monster.MonsterViewModel.MonsterInfo.HP / _monster.MonsterViewModel.MonsterInfo.MaxHP);
        MonsterStamina.SetCurrentStamina(monster_data.Stamina);
        // _monster.gameObject;
        // Slo 위치 갱신
    }

    private bool DetectMonster()
    {
        if (_monster == null) return false;
        float distance = Vector3.Distance(_monster.transform.position, Camera.main.transform.position);
        if (distance <= 15f)
        {
            return true;
        }
        return false;
    }
}
