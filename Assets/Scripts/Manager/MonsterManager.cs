using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    private Dictionary<int, Monster> _monsterLists = new Dictionary<int, Monster>();
    private List<Transform> _lockOnAbleMonsterList = new List<Transform>();
    public List<Transform> LockOnAbleMonsterList { get { return _lockOnAbleMonsterList; } }

    private float _attackingTimer;

    Dictionary<int, Action<float>> _hpChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _maxHpChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _staminaChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _maxStaminaChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _LifeCountChangedCallback = new Dictionary<int, Action<float>>();

    [SerializeField] MainHUD _mainHud;

    private void Awake()
    {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
        FindMainHud();
    }

    private void FindMainHud()
    {
        var gObj = GameObject.Find("Player_UI");
        var mainHud = gObj.GetComponent<MainHUD>();
        if(mainHud != null)
        {
            _mainHud = mainHud;
        }
    }


    #region MonsterList
    public void SpawnMonster(Monster monster)
    {
        if (_monsterLists.ContainsKey(monster.monsterId)) return;

        _monsterLists.Add(monster.monsterId, monster);

        if (monster.Type != MonsterType.Boss)
        {
            CreateMonsterHUD(monster);
        }
        else CreateBossMonsterHUD(monster);
    }

    public void ShowBossMonsterHUD_OnOff(bool onoff)
    {
        if (_mainHud == null) return;

        _mainHud.BossMonsterHud_OnOff(onoff);
    }

    private void CreateBossMonsterHUD(Monster boss)
    {
        if (_mainHud == null) return;

        _mainHud.BindBossMonster(boss);
        ShowBossMonsterHUD_OnOff(true);
    }

    private void CreateMonsterHUD(Monster mob)
    {
        if (_mainHud == null)
            return;

        _mainHud.CreateMonsterHUD(mob);
    }

    public void DieMonster(Monster monster)
    {
        if(_monsterLists.ContainsKey(monster.monsterId))
        {
            if (monster.Type == MonsterType.Boss) ShowBossMonsterHUD_OnOff(false);
            else _mainHud.OffMonsterHUD(monster);
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
        _attackingTimer = UnityEngine.Random.Range(2f, 4f);
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

                _attackingTimer = UnityEngine.Random.Range(2f, 4f);
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
            if (monster.Type == MonsterType.Boss) continue;
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
            if (newMonster.Type == MonsterType.Boss) continue;

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
    #endregion

    public void RegisterMonsterHPChangedCallback(int monsterId, Action<float> monsterHpCallback, bool isRegister)
    {
        if(isRegister)
        {
            if (!_hpChangedCallback.ContainsKey(monsterId)) _hpChangedCallback[monsterId] = monsterHpCallback;
            else _hpChangedCallback.Add(monsterId, monsterHpCallback);
        }
        else
        {
            if (_hpChangedCallback.ContainsKey(monsterId))
            {
                _hpChangedCallback[monsterId] -= monsterHpCallback;
                if (_hpChangedCallback[monsterId] == null) _hpChangedCallback.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterMaxHPChangedCallback(int monsterId, Action<float> monstermaxHpCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_maxHpChangedCallback.ContainsKey(monsterId)) _maxHpChangedCallback[monsterId] = monstermaxHpCallback;
            else _maxHpChangedCallback.Add(monsterId, monstermaxHpCallback);
        }
        else
        {
            if (_maxHpChangedCallback.ContainsKey(monsterId))
            {
                _maxHpChangedCallback[monsterId] -= monstermaxHpCallback;
                if (_maxHpChangedCallback[monsterId] == null) _maxHpChangedCallback.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterStaminaChangedCallback(int monsterId, Action<float> monsterStaminaCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_staminaChangedCallback.ContainsKey(monsterId)) _staminaChangedCallback[monsterId] = monsterStaminaCallback;
            else _staminaChangedCallback.Add(monsterId, monsterStaminaCallback);
        }
        else
        {
            if (_staminaChangedCallback.ContainsKey(monsterId))
            {
                _staminaChangedCallback[monsterId] -= monsterStaminaCallback;
                if (_staminaChangedCallback[monsterId] == null) _staminaChangedCallback.Remove(monsterId);
            }
        }
    }
    public void RegisterMonsterMaxStaminaChangedCallback(int monsterId, Action<float> monsterMaxStaminaCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_maxStaminaChangedCallback.ContainsKey(monsterId)) _maxStaminaChangedCallback[monsterId] = monsterMaxStaminaCallback;
            else _maxStaminaChangedCallback.Add(monsterId, monsterMaxStaminaCallback);
        }
        else
        {
            if (_maxStaminaChangedCallback.ContainsKey(monsterId))
            {
                _maxStaminaChangedCallback[monsterId] -= monsterMaxStaminaCallback;
                if (_maxStaminaChangedCallback[monsterId] == null) _maxStaminaChangedCallback.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterLifeCountChangedCallback(int monsterId, Action<float> monsterMaxStaminaCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_LifeCountChangedCallback.ContainsKey(monsterId)) _LifeCountChangedCallback[monsterId] = monsterMaxStaminaCallback;
            else _LifeCountChangedCallback.Add(monsterId, monsterMaxStaminaCallback);
        }
        else
        {
            if (_LifeCountChangedCallback.ContainsKey(monsterId))
            {
                _LifeCountChangedCallback[monsterId] -= monsterMaxStaminaCallback;
                if (_LifeCountChangedCallback[monsterId] == null) _LifeCountChangedCallback.Remove(monsterId);
            }
        }
    }

    public void SetMonster_data(int monsterId,Monster_data data)
    {
        _hpChangedCallback[monsterId]?.Invoke(data.HP);
        _maxHpChangedCallback[monsterId]?.Invoke(data.MaxHP);
        _staminaChangedCallback[monsterId]?.Invoke(data.Stamina);
        _maxStaminaChangedCallback[monsterId]?.Invoke(data.MaxStamina);
        _LifeCountChangedCallback[monsterId]?.Invoke(data.Life);
    }
}
