using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

enum MonsterFileType
{
    Monster_Info,
    Monster_Attack
}

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance = null;

    private void Awake()
    {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }    

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

    public void LockOnAbleMonsterListChanged(List<Transform> lists)
    {
        if (_lockOnAbleMonsterList.Equals(lists)) return;

        _lockOnAbleMonsterList = lists;
    }
}
