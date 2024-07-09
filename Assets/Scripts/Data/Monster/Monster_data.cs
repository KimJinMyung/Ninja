using System.Collections.Generic;

public class Monster_data
{
    public int DataId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public float HP { get; set; }
    public float MaxHP { get; set; }
    public float ATK { get; set; }
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float Strength { get; set; }
    public float Stamina { get; set; }
    public float MaxStamina { get; set; }
    public float ViewRange { get; set; }
    public float ViewAngel { get; set; }
    public float DefencePer {  get; set; }
    public float Life { get; set; }
    public List<string> AttackMethodName { get; set; }
    public Monster_data MonsterDataClone() 
    {
        return new Monster_data
        {
            DataId = this.DataId,
            Name = this.Name,
            Description = this.Description,
            HP = this.HP,
            MaxHP = this.MaxHP,
            ATK = this.ATK,
            WalkSpeed = this.WalkSpeed,
            RunSpeed = this.RunSpeed,
            Strength = this.Strength,
            Stamina = this.Stamina,
            MaxStamina = this.MaxStamina,
            ViewRange = this.ViewRange,
            ViewAngel = this.ViewAngel,
            DefencePer = this.DefencePer,
            Life = this.Life,
            AttackMethodName = new List<string>(this.AttackMethodName)
        };
    }

}

public enum WeaponsType
{
    SwordAttack,
    SpearAttack,
    ShurikenAttack,
    DaggerAttack,
    GreateSwordAttack
}

public class Monster_Attack
{
    public string DataName { get; set; }
    public string AttackType {  get; set; }
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }

    public Monster_Attack Clone()
    {
        return new Monster_Attack
        {
            DataName = this.DataName,
            AttackType = this.AttackType,
            AttackSpeed = this.AttackSpeed,
            AttackRange = this.AttackRange
        };
    }
}