using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    private void Awake()
    {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private Dictionary<int, Monster> _monsterLists = new Dictionary<int, Monster>();

    private List<Transform> _lockOnAbleMonsterList = new List<Transform>();
    public List<Transform> LockOnAbleMonsterList { get { return _lockOnAbleMonsterList; } }

    public void LockOnAbleMonsterListChanged(List<Transform> monster)
    {
        if(_lockOnAbleMonsterList.Equals(monster)) return;

        _lockOnAbleMonsterList = monster;
    }


}
