using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private float _attackingTimer;

    public void SpawnMonster(Monster monster)
    {
        if (_monsterLists.ContainsKey(monster.monsterId)) return;

        _monsterLists.Add(monster.monsterId, monster);
    }

    public void DieMonster(Monster monster)
    {
        if(_monsterLists.ContainsKey(monster.monsterId))
        {
            _monsterLists.Remove(monster.monsterId);
        }
    }

    public void LockOnAbleMonsterListChanged(List<Transform> monster)
    {
        if(_lockOnAbleMonsterList.Equals(monster)) return;

        _lockOnAbleMonsterList = monster;
    }

    public void DeadMonster_Update(int monster_id)
    {
        if (_monsterLists.ContainsKey(monster_id))
        {
            _monsterLists.Remove(monster_id);
        }
    }

    private void OnEnable()
    {
        _attackingTimer = Random.Range(2f, 4f);
    }

    private void Update()
    {
        if (_monsterLists.Count <= 0) return;

        if (IsAttackAble())
        {
            if (_attackingTimer > 0) _attackingTimer -= Time.deltaTime;
            else
            {
                //한마리만 Attack
                var attackMonster = SelectMonsterForAttack();

                if (attackMonster == null) return;
                if (attackMonster.MonsterViewModel.MonsterState != State.Battle && attackMonster.MonsterViewModel.MonsterState != State.Circling) return;

                _attackingTimer = Random.Range(2f, 4f);
                attackMonster.MonsterViewModel.RequestStateChanged(attackMonster.monsterId, State.Attack);                          
            }
        }
    }

    private bool IsAttackAble()
    {
        if (_monsterLists.Count <= 0) return false;

        bool hasTarget = false;

        foreach (var monster in _monsterLists.Values)
        {
            if (monster.Type == monsterType.Boss) continue;
            if (monster != null && monster.MonsterViewModel.TraceTarget != null)
            {
                hasTarget = true;

                if (monster.IsCurrentState(State.Attack)) return false;
            }

        }

        if (!hasTarget) return false;

        return true;
    }

    Monster SelectMonsterForAttack()
    {
        List<Monster> monsterList = new List<Monster>();

        foreach (var monster in _monsterLists.Values)
        {
            Monster newMonster = monster.GetComponent<Monster>();
            if (newMonster.Type == monsterType.Boss) continue;

            if (newMonster.MonsterViewModel.MonsterState != State.Battle) continue;

            Transform target = newMonster.MonsterViewModel.TraceTarget;
            if (target != null)
            {
                Vector3 targetDir = (target.position - newMonster.transform.position).normalized;
                float Angle = Vector3.Angle(newMonster.transform.forward, targetDir);
                if(Angle < 10f) monsterList.Add(newMonster);
            }
        }

        return monsterList.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
    }
}
