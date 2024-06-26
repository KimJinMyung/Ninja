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
        MonsterManager.instance.LockOnAbleMonsterListChanged(newColliders);
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
        //���� ������ Ÿ���� ������ �Ͱ� �����ϸ� return
        if (target == model.LockOnAbleTarget) return;

        //���� ������ Ÿ���� LockOnTarget�̸� model.LockOnAbleTarget ���θ� ����
        //�׷��� �ʴٸ� ������ Ÿ���� ���̾ LockOnAble�� ����.
        //������ Ÿ���� Monster�� ����
        if(target != null)
        {
            if (target.gameObject.layer != LayerMask.NameToLayer("LockOnTarget"))
            {
                target.gameObject.layer = LayerMask.NameToLayer("LockOnAble");
                if (model.LockOnAbleTarget != null)
                {
                    if (model.LockOnAbleTarget.gameObject.layer != LayerMask.NameToLayer("LockOnTarget"))
                    {
                        model.LockOnAbleTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
                    }                    
                }
            }
            else model.LockOnAbleTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
        }
        else
        {
            if (model.LockOnAbleTarget != null)
            {
                if(model.LockOnAbleTarget.gameObject.layer == LayerMask.NameToLayer("LockOnTarget")) return;
                model.LockOnAbleTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
            }                
        }        
        
        model.LockOnAbleTarget = target;
    }
    #endregion

    #region LockOnTarget
    public static void RegisterLockOnViewModel_TargetChanged(this LockOnViewModel model, bool isRegister)
    {
        ActorLogicManager._instance.RegisterLockOnViewModel_TargetChangedCallback(model.OnResponseLockOnTargetChangedEvent, isRegister);
    }

    public static void RequestLockOnViewModel_Target(this LockOnViewModel model, Transform target, Player player)
    {
        ActorLogicManager._instance.OnLockOnTarget_LockOnViewModel(target, player);
    }

    public static void OnResponseLockOnTargetChangedEvent(this LockOnViewModel model, Transform target, Player player)
    {
        if (target == model.LockOnTarget) return;

        if(model.LockOnTarget != null) 
        {
            if (model.LockOnTarget == model.LockOnAbleTarget) model.LockOnTarget.gameObject.layer = LayerMask.NameToLayer("LockOnAble");
            else model.LockOnTarget.gameObject.layer = LayerMask.NameToLayer("Monster");            
        }

        if (target != null)
            target.gameObject.layer = LayerMask.NameToLayer("LockOnTarget");

        Debug.Log(player.transform.name);
        player.ViewModel.RequestLockOnTarget(target);

        model.LockOnTarget = target;
    }
    #endregion
}
