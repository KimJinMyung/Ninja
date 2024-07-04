using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    private Player owner;
    private Player_LockOn ownerViewZone;

    [Header("Grappling")]
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float trajectoryHeight = 2f;
    [SerializeField] private float grappleSpeed = 5f;
    [SerializeField] private float waveFrequency = 2f; // 웨이브 빈도
    [SerializeField] private float waveAmplitude = 0.5f; // 웨이브 진폭

    [Header("Grappling Stop Check")]
    [SerializeField] private float _minDistance = 1f;
    [SerializeField] private float _minHeight = 0.3f;

    [Header("Grappling StartPos")]
    [SerializeField] private Transform LeftHand;

    [Header("Cooldown")]
    [SerializeField]public float grapplingCd;
    [SerializeField]private float grapplingCdTimer;

    private LineRenderer lr;
    private Vector3 GrapplingPoint;
    private float grappleStartTime;

    private State currentState;

    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    protected readonly int hashIsGrappling = Animator.StringToHash("Grappling");
    protected readonly int hashPullGrappling = Animator.StringToHash("Pull");

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
        if(grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;

        if (IsGrappling)
        {
            if (owner.transform.position.y < GrapplingPoint.y - 3f) GrapplingMove();
            else HandleHookShotMovement();
        }
    }

    public void OnRopeMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartGrapple();
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
            grappleStartTime = Time.time;

            Vector3 directionToGrapplingPoint = GrapplingPoint - transform.position;
            directionToGrapplingPoint.y = 0; 
            Quaternion targetRotation = Quaternion.LookRotation(directionToGrapplingPoint);
            owner.transform.rotation = targetRotation;

            int layerIndex = owner.Animator.GetLayerIndex("Grappling");
            owner.Animator.SetLayerWeight(layerIndex, 1);
            owner.Animator.SetBool(hashIsGrappling, true);

            owner.isGravityAble = false;
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

        if (angleToGrapplingPoint <= checkAngle) return false;
        else return true;  
    }

    void CheckIsClose(Vector3 originPos, Vector3 targetPos, float minDis, float minHeight)
    {
        var vecOrigin = new Vector2(originPos.x, originPos.z);
        var vecTarget = new Vector2(targetPos.x, targetPos.z);

        float target = Vector2.Distance(vecOrigin, vecTarget);

        if (target < minDis)
        {
            StopGrapple();            
        }
    }


    void GrapplingMove()
    {
        owner.isGravityAble = true;

        // 목표 지점과의 거리 체크
        float distanceToTarget = Vector3.Distance(owner.transform.position, GrapplingPoint);

        Vector3 velocity = CalculateJumpVelocity(transform.position, GrapplingPoint, trajectoryHeight);

        if (distanceToTarget < _minDistance)
        {
            // 목표 지점에 거의 도달한 경우 속도를 줄여 정확히 목표 지점에 위치하도록 보정
            float decelerationFactor = distanceToTarget / _minDistance;
            velocity *= Mathf.Max(0.1f, decelerationFactor);
        }

        // 플레이어 이동
        owner.playerController.Move(grappleSpeed * velocity * Time.deltaTime);

        // 도착 조건
        CheckIsClose(owner.transform.position, GrapplingPoint, _minDistance, _minHeight);
    }
    private void HandleHookShotMovement()
    {
        owner.isGravityAble = false;

        Vector3 hookShotDir = (GrapplingPoint - owner.transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotDistance = Mathf.Clamp(Vector3.Distance(owner.transform.position, GrapplingPoint), hookshotSpeedMin, hookshotSpeedMax);

        owner.playerController.Move(grappleSpeed * hookshotDistance * hookShotDir * Time.deltaTime);

        if (Vector3.Distance(transform.position, GrapplingPoint) < _minDistance)
        {
            //owner.ViewModel.RequestStateChanged(owner.player_id, currentState);
            StopGrapple();
        }
    }

    public void ExecuteGrapple()
    {
        currentState = owner.ViewModel.playerState;
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Grappling);

        owner.isGravityAble = true;

        //그래플링 애니메이션
        if(!isShootHook)
            StartCoroutine(GrapplingHookAnimation());
    }
    private bool isShootHook;
    IEnumerator GrapplingHookAnimation()
    {
        float distance = Vector3.Distance(LeftHand.position, GrapplingPoint);
        float startTime = Time.time;

        isShootHook = true;
        lr.enabled = true;

        while (true)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed * 20f / distance;

            if (t >= 1f)
            {
                t = 1f;
                break;
            }

            Vector3 currentPoint = Vector3.Lerp(LeftHand.position, GrapplingPoint, t);
            float wave = Mathf.Sin(t * waveFrequency * Mathf.PI) * waveAmplitude;
            currentPoint.y += wave;

            lr.SetPosition(0, LeftHand.position);
            lr.SetPosition(1, currentPoint);

            yield return null;
        }

        lr.SetPosition(1, GrapplingPoint);

        //플레이어가 매달리는 애니메이션
        owner.Animator.SetTrigger(hashPullGrappling);
        IsGrappling = true;
        yield break;
    }

    public void StopGrapple()
    {
        owner.Animator.SetBool(hashIsGrappling, false);

        owner.Animator.applyRootMotion = true;
        owner.Animator.SetBool(hashIsMoveAble, true);

        isShootHook = false;
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
