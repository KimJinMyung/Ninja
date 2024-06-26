using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOn_UI : MonoBehaviour
{
    [SerializeField] private GameObject _lockOnIconPrefab;

    private Canvas _thisCanvas;

    private Dictionary<Transform, Image> monsterIcons = new Dictionary<Transform, Image>();

    private void Awake()
    {
        _thisCanvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        foreach (var monster in MonsterManager.instance.LockOnAbleMonsterList)
        {
            if(monster == null) continue;

            if (!monsterIcons.ContainsKey(monster))
            {
                GameObject newIcon;
                if (!ActivateInactiveChild(transform, out newIcon))
                {
                    newIcon = Instantiate(_lockOnIconPrefab);
                }
                newIcon.gameObject.SetActive(true);
                newIcon.transform.SetParent(_thisCanvas.transform);
                Image _lockOnIcon = newIcon.GetComponent<Image>();
                monsterIcons.Add(monster, _lockOnIcon);
            }

            MonsterLockOnUIPrint(monster, monsterIcons[monster]);
        }

        if (monsterIcons.Count <= 0) return;

        List<Transform> removeList = new List<Transform>();
        foreach (var monster in monsterIcons)
        {
            if (!MonsterManager.instance.LockOnAbleMonsterList.Contains(monster.Key))
            {
                removeList.Add(monster.Key);
                monster.Value.enabled = false;
            }
        }

        foreach (var monster in removeList)
        {
            if (monster.gameObject.layer == LayerMask.NameToLayer("LockOnAble")) return;
            monsterIcons[monster].gameObject.SetActive(false);
            monsterIcons.Remove(monster);
        }
    }

    bool ActivateInactiveChild(Transform parent, out GameObject newIcon)
    {
        foreach(Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
            {
                newIcon = child.gameObject;
                return true;
            }
        }
        newIcon = null;
        return false;
    }

    private void MonsterLockOnUIPrint(Transform _monster, Image icon)
    {
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(_monster.position + Vector3.up);

        if (ScreenPosition.z > 0)
        {
            Vector2 canvasPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _thisCanvas.transform as RectTransform,
                ScreenPosition,
                _thisCanvas.worldCamera,
                out canvasPosition);

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
        if (target.layer == LayerMask.NameToLayer("LockOnTarget"))
        {
            _lockOnIcon.color = Color.red;
        }
        else if (target.layer == LayerMask.NameToLayer("LockOnAble"))
        {
            _lockOnIcon.color = Color.blue;
        }
        else
        {
            _lockOnIcon.color = Color.white;
        }
    }
}
