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
        textAssetDic.Add(MonsterFileType.Monster_Info, Resources.Load("Monster_data") as TextAsset);
        textAssetDic.Add(MonsterFileType.Monster_Attack, Resources.Load("MonsterAttack") as TextAsset);
    }

    private void ReadDataOnAwake()
    {
        ReadData("Monster_data", MonsterFileType.Monster_Info);
        ReadData("Monster_Attack", MonsterFileType.Monster_Attack);
    }

    private void ReadData(string tableName, MonsterFileType fileType)
    {       
        var textAsset = textAssetDic[fileType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if(xmlAsset == null) return;
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

        foreach (var dataCategory in xmlAsset.Descendants("dataCategory"))
        {
            Monster_data monster = new Monster_data();
            monster.DataId = int.Parse(dataCategory.Element(nameof(monster.DataId)).Value);
            monster.Name = dataCategory.Element(nameof(monster.Name)).Value;
            monster.HP = float.Parse(dataCategory.Element(nameof(monster.HP)).Value);
            monster.ATK = float.Parse(dataCategory.Element(nameof(monster.ATK)).Value);
            monster.WalkSpeed = float.Parse(dataCategory.Element(nameof(monster.WalkSpeed)).Value);
            monster.RunSpeed = float.Parse(dataCategory.Element(nameof(monster.RunSpeed)).Value);
            monster.Strength = float.Parse(dataCategory.Element(nameof(monster.Strength)).Value);
            monster.Stamina = float.Parse(dataCategory.Element(nameof(monster.Stamina)).Value);
            monster.Description = dataCategory.Element(nameof(monster.Description)).Value;

            string AttackMethodNameString = dataCategory.Element("AttackMethodName").Value;
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
                monster.AttackMethod_Name = list;
            }
            LoadedMonsterDataList.Add(monster.DataId, monster);
        }       
    }

    private void FileType_MonsterAttack(XDocument xmlAsset)
    {
        LoadedMonsterAttackList = new Dictionary<string, Monster_Attack>();

        foreach (var dataCategory in xmlAsset.Descendants("dataCategory"))
        {
            Monster_Attack attack = new Monster_Attack();
            attack.DataName = dataCategory.Element(nameof(attack.DataName)).Value;
            attack.AttackScriptsName = dataCategory.Element(nameof(attack.AttackScriptsName)).Value;

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
