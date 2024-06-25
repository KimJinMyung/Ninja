using System.Collections.Generic;

public class Monster_data
{
    public int DataId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public float HP { get; set; }
    public float ATK { get; set; }
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float Strength { get; set; }
    public float Stamina { get; set; }
    public List<string> AttackMethodName { get; set; }
}

public class Monster_Attack
{
    public string DataName { get; set; }
    public string AttackScriptName { get; set; }

    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }
}