using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    #region LoadData
    public Dictionary<int, Monster_data> LoadedMonsterDataList { get; private set; }
    public Dictionary<string, Monster_Attack> LoadedMonsterAttackList { get; private set; }

    private Dictionary<MonsterFileType, TextAsset> textAssetDic = new Dictionary<MonsterFileType, TextAsset>();

    public Dictionary<int, Player_data> LoadPlayerData { get; private set; }

    private TextAsset playerTextAsset;

    void LoadFile()
    {
        textAssetDic.Add(MonsterFileType.Monster_Info, Resources.Load(nameof(Monster_data)) as TextAsset);
        textAssetDic.Add(MonsterFileType.Monster_Attack, Resources.Load(nameof(Monster_Attack)) as TextAsset);


        playerTextAsset = Resources.Load(nameof(Player_data)) as TextAsset;
    }

    private void ReadDataOnAwake()
    {
        ReadMonsterData(nameof(Monster_data), MonsterFileType.Monster_Info);
        ReadMonsterData(nameof(Monster_Attack), MonsterFileType.Monster_Attack);
        ReadPlayerData();
    }

    private void ReadMonsterData(string tableName, MonsterFileType fileType)
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

    private void ReadPlayerData()
    {
        XDocument xmlAsset = XDocument.Parse(playerTextAsset.text);
        if (xmlAsset == null) return;

        FileType_PlayerData(xmlAsset);
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
            monster.MaxHP = float.Parse(data.Attribute(nameof(monster.MaxHP)).Value);
            monster.ATK = float.Parse(data.Attribute(nameof(monster.ATK)).Value);
            monster.WalkSpeed = float.Parse(data.Attribute(nameof(monster.WalkSpeed)).Value);
            monster.RunSpeed = float.Parse(data.Attribute(nameof(monster.RunSpeed)).Value);
            monster.Strength = float.Parse(data.Attribute(nameof(monster.Strength)).Value);
            monster.Stamina = float.Parse(data.Attribute(nameof(monster.Stamina)).Value);
            monster.MaxStamina = float.Parse(data.Attribute(nameof(monster.MaxStamina)).Value);
            monster.Description = data.Attribute(nameof(monster.Description)).Value;
            monster.ViewRange = float.Parse(data.Attribute(nameof(monster.ViewRange)).Value);
            monster.ViewAngel = float.Parse(data.Attribute(nameof(monster.ViewAngel)).Value);
            monster.DefencePer = float.Parse(data.Attribute(nameof(monster.DefencePer)).Value);
            monster.Life = float.Parse(data.Attribute(nameof(monster.Life)).Value);

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
            attack.AttackType = data.Attribute(nameof(attack.AttackType)).Value;
            attack.AttackRange = float.Parse(data.Attribute(nameof(attack.AttackRange)).Value);
            attack.AttackSpeed = float.Parse(data.Attribute(nameof(attack.AttackSpeed)).Value);

            LoadedMonsterAttackList.Add(attack.DataName, attack);
        }
    }

    private void FileType_PlayerData(XDocument xmlAsset)
    {
        LoadPlayerData = new Dictionary<int, Player_data>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            Player_data player_Data = new Player_data();
            player_Data.PlayerId = int.Parse(data.Attribute(nameof(player_Data.PlayerId)).Value);
            player_Data.HP = float.Parse(data.Attribute(nameof(player_Data.HP)).Value);
            player_Data.MaxHP = float.Parse(data.Attribute(nameof(player_Data.MaxHP)).Value);
            player_Data.ATK = float.Parse(data.Attribute(nameof(player_Data.ATK)).Value);
            player_Data.WalkSpeed = float.Parse(data.Attribute(nameof(player_Data.WalkSpeed)).Value);
            player_Data.RunSpeed = float.Parse(data.Attribute(nameof(player_Data.RunSpeed)).Value);
            player_Data.Strength = float.Parse(data.Attribute(nameof(player_Data.Strength)).Value);
            player_Data.Stamina = float.Parse(data.Attribute(nameof(player_Data.Stamina)).Value);
            player_Data.MaxStamina = float.Parse(data.Attribute(nameof(player_Data.MaxStamina)).Value);
            player_Data.HP_Plus = float.Parse(data.Attribute(nameof(player_Data.HP_Plus)).Value);
            player_Data.ATK_Plus = float.Parse(data.Attribute(nameof(player_Data.ATK_Plus)).Value);
            player_Data.Strength_Plus = float.Parse(data.Attribute(nameof(player_Data.Strength_Plus)).Value);
            player_Data.Stamina_Plus = float.Parse(data.Attribute(nameof(player_Data.Stamina_Plus)).Value);
            player_Data.Life = float.Parse(data.Attribute(nameof(player_Data.Life)).Value);
            player_Data.Exp = float.Parse(data.Attribute(nameof(player_Data.Exp)).Value);
            player_Data.PlusExp = float.Parse(data.Attribute(nameof(player_Data.PlusExp)).Value);

            LoadPlayerData.Add(player_Data.PlayerId, player_Data);
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
