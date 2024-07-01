using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 forwardRayoffset = new Vector3(0, 0.25f, 0);
    [SerializeField] float forwardRayLength = 1f;
    [SerializeField] float heightRayLength = 5f;
    [SerializeField] LayerMask obstacleLayer;

    public obstacleHitData ObstacleCheck()
    {
        var hitData = new obstacleHitData();

        var forwardDir = transform.position + forwardRayoffset;
        hitData.forwardHitFound = Physics.Raycast(forwardDir, transform.forward, out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardDir, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);

        if(hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }

        return hitData;
    }
}

public struct obstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}