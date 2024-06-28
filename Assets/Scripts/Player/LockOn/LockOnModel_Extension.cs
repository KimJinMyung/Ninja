using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LockOnModel_Extension
{
    private static int MonsterLayer = LayerMask.NameToLayer("Monster");
    private static int LockOnTargetLayer = LayerMask.NameToLayer("LockOnTarget");
    private static int LockOnAbleLayer = LayerMask.NameToLayer("LockOnAble");
    private static int DeadLayer = LayerMask.NameToLayer("Dead");

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
                c.gameObject.layer = MonsterLayer;
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
            if (target.gameObject.layer != LockOnTargetLayer)
            {
                target.gameObject.layer = LockOnAbleLayer;

                if (model.LockOnAbleTarget != null)
                {
                    if (model.LockOnAbleTarget.gameObject.layer != LockOnTargetLayer)
                    {
                        model.LockOnAbleTarget.gameObject.layer = MonsterLayer;
                    }                    
                }
            }
            else model.LockOnAbleTarget.gameObject.layer = MonsterLayer;
        }
        else
        {
            if (model.LockOnAbleTarget != null)
            {
                if(model.LockOnAbleTarget.gameObject.layer == LockOnTargetLayer) return;
                model.LockOnAbleTarget.gameObject.layer = MonsterLayer;
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
            if (model.LockOnTarget == model.LockOnAbleTarget) model.LockOnTarget.gameObject.layer = LockOnAbleLayer;
            else model.LockOnTarget.gameObject.layer = MonsterLayer;            
        }

        if (target != null)
            target.gameObject.layer = LockOnTargetLayer;

        player.ViewModel.RequestLockOnTarget(target);

        model.LockOnTarget = target;
    }
    #endregion
}
