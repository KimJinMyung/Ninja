using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New Parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;
    public string AnimName { get { return animName; } }

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [SerializeField] float climb_Value;
    public float Climb_Value { get { return climb_Value; } }
    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;

    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;

    public Vector3 MatchPos { get; set; }

    public bool CheckIfPossible(obstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y;

        if(height < minHeight || height > maxHeight) return false;

        if (enableTargetMatching)
        {
            MatchPos = hitData.heightHit.point;
        }

        return true;
    }
}
