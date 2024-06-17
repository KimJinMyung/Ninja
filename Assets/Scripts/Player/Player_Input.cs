using System.ComponentModel;
using UnityEngine;

public class InputViewModel
{
    private Vector2 _move;
    public Vector2 Move
    {
        get { return _move; }
        set
        {
            _move = value;
            OnPropertyChanged(nameof(Move));
            OnPropertyChanged(nameof(TargetAngle));
        }
    }
    private float _targetAngle; 
    public float TargetAngle
    {
        get
        {
            return Mathf.Atan2(_move.x, _move.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
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
