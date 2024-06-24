using System.Collections.Generic;
using System.Linq;
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

    private float _attackingTimer;

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

    private void Update()
    {
        if (_monsterLists.Count <= 0) return;

        if (!IsNotAttackAble())
        {
            if (_attackingTimer > 0) _attackingTimer -= Time.deltaTime;
            else
            {
                //한마리만 Attack
                var attackMonster = SelectMonsterForAttack();
                if(Vector3.Distance(attackMonster.transform.position, attackMonster.MonsterViewModel.TraceTarget.position) > attackMonster.AttackRange)
                {
                    attackMonster.Agent.speed = attackMonster.MonsterViewModel.MonsterInfo.RunSpeed;
                    attackMonster.Agent.Move(attackMonster.transform.position);
                }
                else
                {
                    attackMonster.Agent.speed = 0;
                    attackMonster.Agent.Move(attackMonster.transform.position);

                    _attackingTimer = Random.Range(attackMonster.AttackDelayRange.x, attackMonster.AttackDelayRange.y);
                    attackMonster.MonsterViewModel.RequestStateChanged(attackMonster.monsterId, State.Attack);                    
                }              
            }
        }
    }

    private bool IsNotAttackAble()
    {
        foreach (var monsterTransform in _monsterLists.Values)
        {
            var monsterComponent = monsterTransform.GetComponent<Monster>();
            if (monsterComponent == null || monsterComponent.MonsterViewModel.TraceTarget == null)
            {
                return true;
            }

            if (!monsterComponent.IsCurrentState(State.Attack)) return true;
        }

        return false;

        //if (_monsterLists.Values.Any(e => e.GetComponent<Monster>().MonsterViewModel.TraceTarget == null)) return true;

        //return _monsterLists.Values.Any(e => e.GetComponent<Monster>().IsCurrentState(State.Attack));
    }

    Monster SelectMonsterForAttack()
    {
        return _monsterLists.Values.OrderByDescending(e => e.GetComponent<Monster>().CombatMovementTimer).FirstOrDefault().GetComponent<Monster>();       
    }
}
