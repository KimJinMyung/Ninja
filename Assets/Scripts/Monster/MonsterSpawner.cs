using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 포인트")]
    [SerializeField] private GameObject SpawnPoint;

    [Header("스폰할 몬스터 리스트")]
    [SerializeField] private List<GameObject> _monsterList;

    [Header("소환할 몬스터 수")]
    [SerializeField] private float count;

    private void Start()
    {
        List<Transform> _spawnPosList = new List<Transform>();
        
        foreach(Transform childTrans in SpawnPoint.transform)
        {
            _spawnPosList.Add(childTrans);
        }

        for(int i = 0; i< count; i++)
        {
            GameObject newMonster = Instantiate(_monsterList[Random.Range(0, _monsterList.Count)], _spawnPosList[Random.Range(0, _spawnPosList.Count)]);
            Monster monster = newMonster.GetComponent<Monster>();

            MonsterManager.instance.AddMonsters(monster.monsterId, monster.transform);
        }
    }
}
