using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    private Player owner;
    private Player_LockOn ownerViewZone;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float grapplingSpeed;
    [SerializeField] private float grappleAcceleration;

    [Header("Grappling StartPos")]
    [SerializeField] private Transform LeftHand;

    [Header("Cooldown")]
    [SerializeField]public float grapplingCd;
    [SerializeField]private float grapplingCdTimer;

    private LineRenderer lr;
    private Vector3 GrapplingPoint;
    private float currentGrappleSpeed;
    private Vector3 controlPoint;
    private float grappleStartTime;

    public bool grappling {  get; private set; }

    private void Start()
    {
        owner = GetComponent<Player>();
        ownerViewZone = GetComponent<Player_LockOn>();
        lr = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling) 
        { 
            lr.SetPosition(0, LeftHand.position);
            GrappleMove(); 
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;       

        GrapplingPoint = GetRopePoint();
        if(GrapplingPoint != Vector3.zero)
        {
            grappling = true;
            owner.Animator.SetBool("IsMoveAble", false);
            grappleStartTime = Time.time;

            // 중간점 설정 (플레이어와 목표점 사이의 중간 지점을 약간 위로 올려서 설정)
            controlPoint = (transform.position + GrapplingPoint) / 2 + Vector3.up * 5f;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }        
    }

    private void ExecuteGrapple()
    {
        //애니메이션 실행
        lr.enabled = true;
        lr.SetPosition(1, GrapplingPoint);
    }

    private void GrappleMove()
    {
        float timeSinceStart = (Time.time - grappleStartTime) * grapplingSpeed;
        float journeyLength = Vector3.Distance(transform.position, GrapplingPoint);
        float fracJourney = timeSinceStart / journeyLength;

        // 베지어 곡선을 따라 이동
        Vector3 currentPoint = Bezier.GetPoint(transform.position, controlPoint, GrapplingPoint, fracJourney);
        owner.playerController.Move(currentPoint - transform.position);

        // 목표 지점과의 거리 체크
        float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

        // 도착 거리 설정 (예시로 0.5f로 설정)
        float arrivalDistance = 0.5f;

        if (distanceToTarget < arrivalDistance)
        {
            // 목표 지점에 거의 도달한 경우 속도를 줄여 정확히 목표 지점에 위치하도록 보정
            float decelerationFactor = distanceToTarget / arrivalDistance;
            grapplingSpeed *= decelerationFactor;
        }

        // 속도 증가 (FixedUpdate에서는 Time.deltaTime을 사용하지 않고 직접 물리 시간 간격을 계산)
        grapplingSpeed = Mathf.Min(grapplingSpeed + grappleAcceleration * Time.fixedDeltaTime, grapplingSpeed);

        // 도착 조건
        if (fracJourney >= 1f || distanceToTarget < arrivalDistance)
        {
            StopGrapple();
        }
    }
    private void StopGrapple()
    {
        owner.Animator.SetBool("IsMoveAble", true);
        grappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;
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
}
