using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Monster Prefab")]
    [SerializeField] private GameObject monsterPrefab;

    [Serializable]
    public class spawnMonsterSetting
    {
        public MonsterType monsterType;
        public GameObject spawnPoint;
        public int spawnCount;
    }

    [Header("Spawn Monster Type")]
    [SerializeField] private spawnMonsterSetting[] spawnSettings;

    private void Start()
    {
        foreach (var spawnSetting in spawnSettings)
        {
            List<Transform> spawnPosition = new List<Transform>();

            foreach(Transform spawn in spawnSetting.spawnPoint.transform)
            {
                spawnPosition.Add(spawn);
            }

            for (int i = 0; i< spawnSetting.spawnCount; i++)
            {                
                GameObject NewMonster = Instantiate(monsterPrefab, spawnPosition[UnityEngine.Random.Range(0, spawnPosition.Count)].position, Quaternion.identity);
                Monster spawnMonster = NewMonster.GetComponent<Monster>();
                spawnMonster.SetStateOnCreate(spawnSetting.monsterType);
                NewMonster.transform.parent = transform;
            }
        }
    }
}
