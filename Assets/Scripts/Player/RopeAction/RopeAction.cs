using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    private Player owner;
    private Player_LockOn ownerViewZone;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float trajectoryHeight = 2f;
    [SerializeField] private float grappleSpeed = 5f;

    [SerializeField] private float grapplingAbleAngle = 60f;

    [Header("Grappling StartPos")]
    [SerializeField] private Transform LeftHand;

    [Header("Cooldown")]
    [SerializeField]public float grapplingCd;
    [SerializeField]private float grapplingCdTimer;

    private LineRenderer lr;
    private Vector3 GrapplingPoint;
    private float grappleStartTime;

    private Vector3 jumpVelocity;
    private State currentState;

    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    public bool IsGrappling {  get; private set; }

    private void Awake()
    {
        owner = GetComponent<Player>();
        ownerViewZone = GetComponent<Player_LockOn>();
        lr = GetComponentInChildren<LineRenderer>();
    }

    private void OnEnable()
    {
        lr.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;

        if (IsGrappling)
        {
            GrapplingMove();
        }
    }

    private void LateUpdate()
    {
        Debug.Log(owner.ViewModel.playerState);
        if (IsGrappling) 
        { 
            lr.SetPosition(0, LeftHand.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        GrapplingPoint = GetRopePoint();
        if (GrapplingPoint != Vector3.zero)
        {
            if (IsGrapplingAblePoint(GrapplingPoint, grapplingAbleAngle)) return;
            //if (CheckIsUnderObject(owner.transform.position, GrapplingPoint))
            //{
            //    return;
            //}

            IsGrappling = true;
            owner.Animator.SetBool(hashIsMoveAble, false);
            grappleStartTime = Time.time;
            jumpVelocity = CalculateJumpVelocity(owner.transform.position, GrapplingPoint, trajectoryHeight);

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    private bool IsGrapplingAblePoint(Vector3 grapplingPoint, float checkAngle)
    {
        Vector3 direcion = grapplingPoint - owner.transform.position;

        direcion.Normalize();

        float dot = Vector3.Dot(direcion, Vector3.up);

        float angleToGrapplingPoint = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angleToGrapplingPoint <= checkAngle) return true;
        else return false;  
    }

    void CheckIsClose(Vector3 originPos, Vector3 targetPos, float minDis = 1.0f, float minHeight = 0.3f)
    {
        //bool isClose = false;

        var vecOrigin = new Vector2(originPos.x, originPos.z);
        var vecTarget = new Vector2(targetPos.x, targetPos.z);


        float target = Vector2.Distance(vecOrigin, vecTarget);
        Debug.Log($"거리 차이값 : {target}");

        if (target < minDis)
        {
            float heightDis = (originPos.y - targetPos.y);
            Debug.LogWarning($"높이 차이값 : {heightDis}");

            if (heightDis > minHeight)
            {
                StopGrapple();
            }
        }
    }


    void GrapplingMove()
    {

         // 목표 지점과의 거리 체크
        float distanceToTarget = Vector3.Distance(owner.transform.position, GrapplingPoint);

        // 도착 거리 설정 (예시로 0.5f로 설정)
        float arrivalDistance = 0.5f;

        Vector3 velocity = CalculateJumpVelocity(transform.position, GrapplingPoint, trajectoryHeight);

        if (distanceToTarget < arrivalDistance)
        {
            // 목표 지점에 거의 도달한 경우 속도를 줄여 정확히 목표 지점에 위치하도록 보정
            float decelerationFactor = distanceToTarget / arrivalDistance;
            velocity *= Mathf.Max(0.1f, decelerationFactor);            
        }

        // 플레이어 이동
        owner.playerController.Move(grappleSpeed * velocity * Time.deltaTime);

        // 도착 조건
        CheckIsClose(owner.transform.position, GrapplingPoint);
    }

    private void ExecuteGrapple()
    {
        currentState = owner.ViewModel.playerState;
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Grappling);

        //애니메이션 실행
        lr.enabled = true;
        lr.SetPosition(1, GrapplingPoint);
    }

    private void StopGrapple()
    {
        owner.Animator.SetBool(hashIsMoveAble, true);
        IsGrappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;
        owner.ViewModel.RequestStateChanged(owner.player_id, currentState);
    }

    private Vector3 GetRopePoint()
    {
        Vector3 targetPos = Vector3.zero;

        if (ownerViewZone.ViewModel.LockOnAbleTarget.CompareTag("RopePoint"))
        {
            targetPos = ownerViewZone.ViewModel.LockOnAbleTarget.position;
        }

        return targetPos;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = owner.GravityValue;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        float sqrTerm1 = Mathf.Sqrt(-2 * trajectoryHeight / gravity);
        float sqrTerm2 = Mathf.Sqrt(Mathf.Max(0, 2 * (displacementY - trajectoryHeight) / gravity));

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (sqrTerm1 + sqrTerm2);

        return velocityXZ + velocityY;
    }
}
