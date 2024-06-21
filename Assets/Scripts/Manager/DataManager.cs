using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance {  get; private set; }

    #region LoadData
    public Dictionary<int, Monster_data> LoadedMonsterDataList { get; private set; }
    public Dictionary<string, Monster_Attack> LoadedMonsterAttackList { get; private set; }

    private Dictionary<MonsterFileType, TextAsset> textAssetDic = new Dictionary<MonsterFileType, TextAsset>();
    void LoadFile()
    {
        textAssetDic.Add(MonsterFileType.Monster_Info, Resources.Load(nameof(Monster_data)) as TextAsset);
        textAssetDic.Add(MonsterFileType.Monster_Attack, Resources.Load(nameof(Monster_Attack)) as TextAsset);
    }

    private void ReadDataOnAwake()
    {
        ReadData(nameof(Monster_data), MonsterFileType.Monster_Info);
        ReadData(nameof(Monster_Attack), MonsterFileType.Monster_Attack);
    }

    private void ReadData(string tableName, MonsterFileType fileType)
    {       
        var textAsset = textAssetDic[fileType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if (xmlAsset == null) return;
        //string dataString = textAsset.text;

        switch (fileType)
        {
            case MonsterFileType.Monster_Info:
                FileType_MonsterData(xmlAsset);
                break;
            case MonsterFileType.Monster_Attack:
                FileType_MonsterAttack(xmlAsset);
                break;
        }      
    }

    private void FileType_MonsterData(XDocument xmlAsset)
    {
        LoadedMonsterDataList = new Dictionary<int, Monster_data>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            Monster_data monster = new Monster_data();
            monster.DataId = int.Parse(data.Attribute(nameof(monster.DataId)).Value);
            monster.Name = data.Attribute(nameof(monster.Name)).Value;
            monster.HP = float.Parse(data.Attribute(nameof(monster.HP)).Value);
            monster.ATK = float.Parse(data.Attribute(nameof(monster.ATK)).Value);
            monster.WalkSpeed = float.Parse(data.Attribute(nameof(monster.WalkSpeed)).Value);
            monster.RunSpeed = float.Parse(data.Attribute(nameof(monster.RunSpeed)).Value);
            monster.Strength = float.Parse(data.Attribute(nameof(monster.Strength)).Value);
            monster.Stamina = float.Parse(data.Attribute(nameof(monster.Stamina)).Value);
            monster.Description = data.Attribute(nameof(monster.Description)).Value;

            string AttackMethodNameString = data.Attribute(nameof(monster.AttackMethodName)).Value;
            if (!string.IsNullOrEmpty(AttackMethodNameString))
            {
                AttackMethodNameString = AttackMethodNameString.Replace("{", string.Empty);
                AttackMethodNameString = AttackMethodNameString.Replace("}", string.Empty);

                var AttackNames = AttackMethodNameString.Split(',');

                var list = new List<string>();
                if (AttackNames.Length > 0)
                {
                    foreach (var name in AttackNames)
                    {
                        list.Add(name);
                    }
                }
                monster.AttackMethodName = list;
            }
            LoadedMonsterDataList.Add(monster.DataId, monster);
        }       
    }

    private void FileType_MonsterAttack(XDocument xmlAsset)
    {
        LoadedMonsterAttackList = new Dictionary<string, Monster_Attack>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            Monster_Attack attack = new Monster_Attack();
            attack.DataName = data.Attribute(nameof(attack.DataName)).Value;
            attack.AttackScriptName = data.Attribute(nameof(attack.AttackScriptName)).Value;

            LoadedMonsterAttackList.Add(attack.DataName, attack);
        }
    }


    #endregion

    private void Awake()
    {
        SetSingleton();

        LoadFile();
        ReadDataOnAwake();
    }

    private void SetSingleton()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
}
