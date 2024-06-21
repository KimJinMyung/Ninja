using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    #region LoadData
    public Dictionary<int, Monster_data> LoadedMonsterDataList { get; private set; }
    public Dictionary<string, Monster_Attack> LoadedMonsterAttackList { get; private set; }

    private Dictionary<MonsterFileType, TextAsset> textAssetDic = new Dictionary<MonsterFileType, TextAsset>();
    void LoadFile()
    {
        textAssetDic.Add(MonsterFileType.Monster_Info, Resources.Load("Monster_data") as TextAsset);
        textAssetDic.Add(MonsterFileType.Monster_Attack, Resources.Load("MonsterAttack") as TextAsset);
    }

    private void ReadDataOnAwake()
    {
        foreach (var item in textAssetDic)
        {
            ReadData(nameof(item.Value), item.Key);
        }
    }

    private void ReadData(string tableName, MonsterFileType fileType)
    {       
        var textAsset = textAssetDic[fileType];

        string dataString = textAsset.text;

        string[] lines = dataString.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        bool isFirstLineHeader = true;

        foreach (var line in lines)
        {
            if (isFirstLineHeader)
            {
                isFirstLineHeader = false;
                continue;
            }

            string[] fields = line.Split(',');

            Debug.Log(fields);

            switch (tableName)
            {
                case nameof(Monster_data):
                    if (!FileType_MonsterData(fields)) continue;
                    break;
                case nameof(Monster_Attack):
                    if (!FileType_MonsterAttack(fields)) continue;
                    break;
            };
        }


    }

    private bool FileType_MonsterData(string[] fields)
    {
        if (fields.Length < 10)
        {
            return false;
        }

        LoadedMonsterDataList = new Dictionary<int, Monster_data>();

        // 예시로 Monster_data 클래스는 다음과 같다고 가정합니다.
        Monster_data monster = new Monster_data();
        monster.DataId = int.Parse(fields[0]); 
        monster.Name = fields[1]; 
        monster.HP = float.Parse(fields[3]);
        monster.ATK = float.Parse(fields[4]);
        monster.WalkSpeed = float.Parse(fields[5]);
        monster.RunSpeed = float.Parse(fields[6]);
        monster.Strength = float.Parse(fields[7]);
        monster.Stamina = float.Parse(fields[8]);
        monster.Description = fields[2];

        string AttackMethodNameString = fields[9];
        if (!string.IsNullOrEmpty(AttackMethodNameString))
        {
            AttackMethodNameString = AttackMethodNameString.Replace("{", string.Empty);
            AttackMethodNameString = AttackMethodNameString.Replace("}", string.Empty);

            var AttackNames = AttackMethodNameString.Split(',');

            var list = new List<string>();
            if(AttackNames.Length > 0)
            {
                foreach(var name in AttackNames)
                {
                    list.Add(name);
                }
            }
            monster.AttackMethod_Name = list;
        }

        // LoadedMonsterList에 추가합니다.
        LoadedMonsterDataList.Add(monster.DataId, monster);

        return true;
    }

    private bool FileType_MonsterAttack(string[] fields)
    {
        if (fields.Length < 2)
        {
            return false;
        }

        LoadedMonsterAttackList = new Dictionary<string, Monster_Attack>();

        Monster_Attack monsterAttack = new Monster_Attack();
        monsterAttack.DataName = fields[0];

        string AttackNameString = fields[1];
        if (!string.IsNullOrEmpty(AttackNameString))
        {
            AttackNameString = AttackNameString.Replace("{", string.Empty);
            AttackNameString = AttackNameString.Replace("}", string.Empty);

            var scriptsName = AttackNameString.Split(',');

            var list = new List<string>();
            if(scriptsName.Length > 0)
            {
                foreach (var script in scriptsName)
                {
                    list.Add(script);
                }
            }
            monsterAttack.AttackScriptsName = list;
        }

        LoadedMonsterAttackList.Add(monsterAttack.DataName, monsterAttack);

        return true;
    }

        #endregion

    private void Awake()
    {
        LoadFile();
        ReadDataOnAwake();
    }
}
