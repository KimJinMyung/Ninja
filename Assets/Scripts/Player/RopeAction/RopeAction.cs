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

            // �߰��� ���� (�÷��̾�� ��ǥ�� ������ �߰� ������ �ణ ���� �÷��� ����)
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
        //�ִϸ��̼� ����
        lr.enabled = true;
        lr.SetPosition(1, GrapplingPoint);
    }

    private void GrappleMove()
    {
        float timeSinceStart = (Time.time - grappleStartTime) * grapplingSpeed;
        float journeyLength = Vector3.Distance(transform.position, GrapplingPoint);
        float fracJourney = timeSinceStart / journeyLength;

        // ������ ��� ���� �̵�
        Vector3 currentPoint = Bezier.GetPoint(transform.position, controlPoint, GrapplingPoint, fracJourney);
        owner.playerController.Move(currentPoint - transform.position);

        // ��ǥ �������� �Ÿ� üũ
        float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

        // ���� �Ÿ� ���� (���÷� 0.5f�� ����)
        float arrivalDistance = 0.5f;

        if (distanceToTarget < arrivalDistance)
        {
            // ��ǥ ������ ���� ������ ��� �ӵ��� �ٿ� ��Ȯ�� ��ǥ ������ ��ġ�ϵ��� ����
            float decelerationFactor = distanceToTarget / arrivalDistance;
            grapplingSpeed *= decelerationFactor;
        }

        // �ӵ� ���� (FixedUpdate������ Time.deltaTime�� ������� �ʰ� ���� ���� �ð� ������ ���)
        grapplingSpeed = Mathf.Min(grapplingSpeed + grappleAcceleration * Time.fixedDeltaTime, grapplingSpeed);

        // ���� ����
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
