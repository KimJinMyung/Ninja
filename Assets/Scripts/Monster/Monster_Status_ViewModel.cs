using System.ComponentModel;

public class Monster_Status_ViewModel
{
    private State _monsterState;
    public State MonsterState
    {
        get { return _monsterState; }
        set
        {
            if(_monsterState == value) return;
            _monsterState = value;
            OnPropertyChanged(nameof(MonsterState));
        }
    }

    private float _hp;
    public float HP
    {
        get { return _hp; }
        set
        {
            if(_hp == value) return;

            _hp = value;
            OnPropertyChanged(nameof(HP));
        }
    }

    #region propertyEvent

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
