using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawnManager : MonoBehaviour
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

    [SerializeField] private Collider BattleZone;

    private Collider SpawnerCollider;
    List<Transform> spawnPosition;

    private void Awake()
    {
        SpawnerCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        foreach (var spawnSetting in spawnSettings)
        {
            if(spawnSetting.monsterType == MonsterType.Boss) continue;

            spawnPosition = new List<Transform>();

            foreach(Transform spawn in spawnSetting.spawnPoint.transform)
            {
                spawnPosition.Add(spawn);
            }

            for (int i = 0; i< spawnSetting.spawnCount; i++)
            {                
                GameObject NewMonster = Instantiate(monsterPrefab, GetRandomPos(spawnPosition[UnityEngine.Random.Range(0, spawnPosition.Count)].position, 5), Quaternion.identity);
                Monster spawnMonster = NewMonster.GetComponent<Monster>();
                spawnMonster.SetStateOnCreate(spawnSetting.monsterType);
                NewMonster.transform.parent = transform;
            }
        }
    }

    private Vector3 GetRandomPos(Vector3 origin, float Range)
    {
        int maxAttempts = 30;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector3 randomPoint = origin + UnityEngine.Random.insideUnitSphere * Range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, (1 << NavMesh.GetAreaFromName("Walkable"))))
            {
                return hit.position;
            }
            attempts++;
        }

        return origin;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(var spawnSetting in spawnSettings)
            {
                if(spawnSetting.monsterType == MonsterType.Boss)
                {
                    GameObject NewMonster = Instantiate(monsterPrefab, spawnSetting.spawnPoint.transform.position, Quaternion.identity);
                    Monster spawnMonster = NewMonster.GetComponent<Monster>();
                    spawnMonster.SetStateOnCreate(spawnSetting.monsterType);
                    NewMonster.transform.parent = transform;

                    BattleZone.enabled = true;
                    SpawnerCollider.enabled = false;
                }
            }
        }
    }
}
