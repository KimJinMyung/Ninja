using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LockOnModel_Extension
{
    #region LockOnTargetList
    public static void RegisterLockOnTargetListChanged(this LockOnViewModel model, bool isRegister)
    {
        ActorLogicManager._instance.RegisterLockOnTargetListChangedCallback(model.OnResponseLockOnTargetListChangedEvent, isRegister);
    }

    public static void RequestLockOnTargetList(this LockOnViewModel model, List<Transform> tartgetlists)
    {
        ActorLogicManager._instance.OnLockOnTargetList(tartgetlists);
    }

    public static void OnResponseLockOnTargetListChangedEvent(this LockOnViewModel model, List<Transform> tartgetlists)
    {
        List<Transform> newColliders = new List<Transform>();

        foreach (var c in tartgetlists)
        {
            if (!model.HitColliders.Contains(c))
            {
                newColliders.Add(c);
            }
        }

        foreach (var c in model.HitColliders)
        {
            if (tartgetlists.Contains(c))
            {
                newColliders.Add(c);
            }
        }

        foreach (var c in model.HitColliders)
        {
            if (!newColliders.Contains(c))
            {
                c.gameObject.layer = LayerMask.NameToLayer("Monster");
            }
        }

        model.HitColliders = newColliders;
        //MonsterManager.instance.LockOnAbleMonsterListChanged(newColliders);
    }
    #endregion

    #region LockOnAbleTarget
    public static void RegisterLockOnAbleTargetChanged(this LockOnViewModel model, bool isRegister)
    {
        ActorLogicManager._instance.RegisterLockOnAbleTargetChangedCallback(model.OnResponseLockOnAbleTargetChangedEvent, isRegister);
    }

    public static void RequestLockOnAbleTarget(this LockOnViewModel model, Transform target)
    {
        ActorLogicManager._instance.OnLockOnAbleTarget(target);
    }

    public static void OnResponseLockOnAbleTargetChangedEvent(this LockOnViewModel model, Transform target)
    {
        model.RequestLockOnTargetList(model.HitColliders);

        if (target.gameObject.layer != LayerMask.NameToLayer("LockOnTarget"))
        {
            if (target != model.LockOnAbleTarget && model.LockOnAbleTarget != null && model.LockOnAbleTarget.gameObject.layer != LayerMask.NameToLayer("LockOnTarget"))
                model.LockOnAbleTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
            target.gameObject.layer = LayerMask.NameToLayer("LockOnAble");
        }

        model.LockOnAbleTarget = target;

    }
    #endregion

    #region LockOnTarget
    public static void RegisterLockOnTargetChanged(this LockOnViewModel model, bool isRegister)
    {
        ActorLogicManager._instance.RegisterLockOnTargetChangedCallback(model.OnResponseLockOnTargetChangedEvent, isRegister);
    }

    public static void RequestLockOnTarget(this LockOnViewModel model, Transform target, Player_ViewModel player)
    {
        ActorLogicManager._instance.OnLockOnTarget(target, player);
    }

    public static void OnResponseLockOnTargetChangedEvent(this LockOnViewModel model, Transform target, Player_ViewModel player)
    {
        if (target != model.LockOnTarget && model.LockOnTarget != null)
        {
            if (model.LockOnTarget == model.LockOnAbleTarget) model.LockOnTarget.gameObject.layer = LayerMask.NameToLayer("LockOnAble");
            else model.LockOnTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
        }


        if (target != null)
            target.gameObject.layer = LayerMask.NameToLayer("LockOnTarget");
        player.RequestLockOnTarget(target);
        model.LockOnTarget = target;
    }
    #endregion
}
