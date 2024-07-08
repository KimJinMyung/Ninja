
public class Player_data
{
    public int PlayerId { get; set; }
    public float HP { get; set; }
    public float ATK { get; set; }
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float Strength { get; set; }
    public float Stamina { get; set; }
    public float HP_Plus { get; set; }
    public float ATK_Plus { get; set; }
    public float Strength_Plus { get; set; }
    public float Stamina_Plus { get; set; }
    public float Life {  get; set; }
    public float Exp {  get; set; }
    public float PlusExp {  get; set; }

    public Player_data Clone()
    {
        return new Player_data
        { PlayerId = this.PlayerId, 
            HP = this.HP, 
            ATK = this.ATK, 
            WalkSpeed = this.WalkSpeed, 
            RunSpeed = this.RunSpeed, 
            Strength = this.Strength, 
            Stamina = this.Stamina,
            HP_Plus = this.HP_Plus,
            ATK_Plus = this.ATK_Plus,
            Strength_Plus = this.Strength_Plus,
            Stamina_Plus = this.Stamina_Plus,
            Life = this.Life,
            Exp = this.Exp,
            PlusExp = this.PlusExp,
        };
    }
}
