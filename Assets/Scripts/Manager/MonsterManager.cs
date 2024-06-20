using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance = null;

    private void Awake()
    {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    //Dictionary<monsterType, Dictionary<int, Transform>> monsterDics = new Dictionary<monsterType, Dictionary<int, Transform>>();
    private Dictionary<int, Transform> _monsterLists = new Dictionary<int, Transform>();

    private List<Transform> _lockOnAbleMonsterList = new List<Transform>();
    public List<Transform> LockOnAbleMonsterList { get { return _lockOnAbleMonsterList; } }

    public void AddMonsters(int actorId, Transform monster)
    {
        if (!_monsterLists.ContainsKey(actorId)) _monsterLists.Add(actorId, monster); 
        else _monsterLists[actorId] = monster;
    }

    public void RemoveMonsters(int actorId)
    {
        if(_monsterLists.ContainsKey(actorId)) _monsterLists.Remove(actorId);
    }

    public Transform LockOnAbleMonsterCallback(int actorId)
    {
        if(!_monsterLists.ContainsKey(actorId)) return null;
        return _monsterLists[actorId];
    }

    public void LockOnAbleListAdd(Transform monster)
    {
        if (_lockOnAbleMonsterList.Contains(monster)) return;
        _lockOnAbleMonsterList.Add(monster);
    }

    public void LockOnAbleListRemove(Transform monster)
    {
        if (!_lockOnAbleMonsterList.Contains(monster)) return;
        _lockOnAbleMonsterList.Remove(monster);
    }
}
