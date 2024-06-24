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

    private Dictionary<int, Monster> _monsterLists = new Dictionary<int, Monster>();

    private List<Transform> _lockOnAbleMonsterList = new List<Transform>();
    public List<Transform> LockOnAbleMonsterList { get { return _lockOnAbleMonsterList; } }

    private float _attackingTimer;

    public void AddMonsters(int actorId, Transform monster)
    {
        var newMonster = monster.GetComponent<Monster>();

        if(newMonster == null) return;

        if (!_monsterLists.ContainsKey(actorId)) _monsterLists.Add(actorId, newMonster); 
        else _monsterLists[actorId] = newMonster;
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

        if (IsAttackAble())
        {
            if (_attackingTimer > 0) _attackingTimer -= Time.deltaTime;
            else
            {
                //�Ѹ����� Attack
                var attackMonster = SelectMonsterForAttack();

                if (attackMonster.MonsterViewModel.MonsterState != State.Battle && attackMonster.MonsterViewModel.MonsterState != State.Circling) return;

                var AttackRange = attackMonster.GetComponentsInChildren<IArk>();

                float range = 0;
                foreach(var Rangelist in AttackRange)
                {
                    if(range < Rangelist.attackRange)
                    {
                        range = Rangelist.attackRange;
                    }
                }

                Debug.Log($"���� ��Ÿ� : {range}");
                if (Vector3.Distance(attackMonster.transform.position, attackMonster.MonsterViewModel.TraceTarget.position) <= range + 1.03f)
                {
                    attackMonster.Agent.speed = 0f;

                    _attackingTimer = Random.Range(attackMonster.AttackDelayRange.x, attackMonster.AttackDelayRange.y);
                    attackMonster.MonsterViewModel.RequestStateChanged(attackMonster.monsterId, State.Attack);
                }                
            }
        }
    }

    //������ ������ ����
    //1. _monsterList.Count �� 0���� Ŀ���Ѵ�. (���Ͱ� �Ѹ����� �����ؾ� �Ѵ�.)
    //2. _monster�� target�� �߰��ؾ��Ѵ�.
    //3. target�� ���� ��� ���͵��� ���°� �ϳ��� State.Attack�̸� �ȵȴ�.

    private bool IsAttackAble()
    {
        if(_monsterLists.Count <=0) return false;

        bool hasTarget = false;

        foreach (var monsterTransform in _monsterLists.Values)
        {
            if (monsterTransform != null && monsterTransform.MonsterViewModel.TraceTarget != null)
            {
                hasTarget = true;

                if (monsterTransform.IsCurrentState(State.Attack)) return false;                
            } 
            
        }

        if(!hasTarget) return false;

        return true;

        //if (_monsterLists.Values.Any(e => e.GetComponent<Monster>().MonsterViewModel.TraceTarget == null)) return true;

        //return _monsterLists.Values.Any(e => e.GetComponent<Monster>().IsCurrentState(State.Attack));
    }

    Monster SelectMonsterForAttack()
    {
        List<Monster> monsterList = new List<Monster>();

        foreach(var monster in _monsterLists.Values)
        {
            if(monster.MonsterViewModel.TraceTarget != null)
            {
                monsterList.Add(monster);
            }
        }

        return monsterList.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();

        //return _monsterLists.Values.OrderByDescending(e => e.GetComponent<Monster>().CombatMovementTimer).FirstOrDefault().GetComponent<Monster>();       
    }
}
