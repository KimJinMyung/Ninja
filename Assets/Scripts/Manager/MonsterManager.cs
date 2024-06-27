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

                if (attackMonster.MonsterViewModel.MonsterState != State.Battle && attackMonster.MonsterViewModel.MonsterState != State.Circling) return;

                //var AttackRange = attackMonster.GetComponentsInChildren<IArk>();

                float range = 0;    //공격 사거리

                //foreach (var Rangelist in AttackRange)
                //{
                //    if (range < Rangelist.attackRange)
                //    {
                //        range = Rangelist.attackRange;
                //    }
                //}


                if (Vector3.Distance(attackMonster.transform.position, attackMonster.MonsterViewModel.TraceTarget.position) <= range + 1.03f)
                {
                    attackMonster.Agent.speed = 0f;

                    //_attackingTimer = Random.Range(attackMonster.AttackDelay - 1.5f, attackMonster.AttackDelay + 1.5f);
                    attackMonster.MonsterViewModel.RequestStateChanged(attackMonster.monsterId, State.Attack);
                }
            }
        }
    }

    private bool IsAttackAble()
    {
        if (_monsterLists.Count <= 0) return false;

        bool hasTarget = false;

        foreach (var monsterTransform in _monsterLists.Values)
        {
            if (monsterTransform != null && monsterTransform.MonsterViewModel.TraceTarget != null)
            {
                hasTarget = true;

                if (monsterTransform.IsCurrentState(State.Attack)) return false;
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
            if (newMonster.MonsterViewModel.TraceTarget != null)
            {
                monsterList.Add(newMonster);
            }
        }

        return monsterList.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
    }
}
