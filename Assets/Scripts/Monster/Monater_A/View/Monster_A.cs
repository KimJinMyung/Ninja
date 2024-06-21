using System.Collections;
using System.ComponentModel;

public class Monster_A : Monster
{
    protected override void Awake()
    {
        base.Awake();
        type = monsterType.monster_A;
    }

    protected override void OnEnable()
    {
        base.OnEnable();        
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    IEnumerator StartAI()
    {
        while (true)
        {

        }
        yield break;
    }

    #region PropertyChangedMethod
    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(sender, e);
    }
    #endregion
}
