using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOn_UI : MonoBehaviour
{
    //[SerializeField] private Transform _monter;
    [SerializeField] private GameObject _lockOnIconPrefab;

    //�ӽ� ����Ʈ [player�� ������ ���͵��� ����Ʈ]
    //MonsterManager���� ����

    //private Image _lockOnIcon;

    //private RectTransform uiElement;
    private Canvas _thisCanvas;

    private Dictionary<Transform, Image> monsterIcons = new Dictionary<Transform, Image>(); 

    private void Awake()
    {
        _thisCanvas = _lockOnIconPrefab.GetComponent<Canvas>();
        //_lockOnIcon = _lockOnIconPrefab.transform.GetComponentInChildren<Image>(); ;
    }

    private void FixedUpdate()
    {
        foreach (var monster in MonsterManager.instance.LockOnAbleMonsterList)
        {
            if (!monsterIcons.ContainsKey(monster))
            {
                GameObject newIcon = Instantiate(_lockOnIconPrefab);
                Image _lockOnIcon = newIcon.GetComponentInChildren<Image>();
                monsterIcons.Add(monster, _lockOnIcon);
            }            

            MonsterLockOnUIPrint(monster, monsterIcons[monster]);
        }

        if(monsterIcons.Count <= 0) return;

        List<Transform> removeList = new List<Transform>();
        foreach(var monster in monsterIcons)
        {
            if (!MonsterManager.instance.LockOnAbleMonsterList.Contains(monster.Key))
            {
                removeList.Add(monster.Key); 
                monster.Value.enabled = false;
            }            
        }

        foreach (var monster in removeList) monsterIcons.Remove(monster);
    }

    private void MonsterLockOnUIPrint(Transform _monster, Image icon)
    {
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(_monster.position + Vector3.up);

        if (ScreenPosition.z > 0)
        {
            Vector2 canvasPosition;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _thisCanvas.transform as RectTransform,
                ScreenPosition,
                _thisCanvas.worldCamera,
                out canvasPosition))
            {
                Debug.Log(canvasPosition);
            }
            else Debug.Log("����");

            icon.rectTransform.anchoredPosition = canvasPosition;

            icon.enabled = true;

            IconColorChanged(_monster.gameObject, icon);
        }
        else
        {
            icon.enabled = false;
        }
    }

    private void IconColorChanged(GameObject target, Image _lockOnIcon)
    {
        if(target.layer == LayerMask.NameToLayer("LockOn"))
        {
            _lockOnIcon.color = Color.red;
        }
        else
        {
            _lockOnIcon.color = Color.white;
        }
    }
}