using System.Collections;
using System.ComponentModel;

public class Monster_A : Monster
{
    protected override void Awake()
    {
        base.Awake();
        type = monsterType.monster_A;
        _viewAngle = 90f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();        
        
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #region PropertyChangedMethod
    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(sender, e);
    }
    #endregion
}
