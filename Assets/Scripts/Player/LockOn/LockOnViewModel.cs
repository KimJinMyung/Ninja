using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LockOnViewModel
{
    private List<Transform> _hitColliders = new List<Transform>();
    public List<Transform> HitColliders
    {
        get { return _hitColliders; }
        set
        {
            if (_hitColliders.Equals(value)) return;

            _hitColliders = value;
            OnPropertyChanged(nameof(HitColliders));
        }
    }

    private Transform _lockOnAbleTarget;
    public Transform LockOnAbleTarget
    {
        get { return _lockOnAbleTarget; }
        set 
        {
            if (_lockOnAbleTarget == value) return;
            _lockOnAbleTarget = value;
            OnPropertyChanged(nameof(LockOnAbleTarget));
        }
    }

    private Transform _lockOnTarget;
    public Transform LockOnTarget
    {
        get { return _lockOnTarget; }
        set
        {
            if (_lockOnTarget == value) return;
            _lockOnTarget = value;
            OnPropertyChanged(nameof(LockOnTarget));
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
