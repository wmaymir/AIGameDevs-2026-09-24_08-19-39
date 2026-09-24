using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseEntity))]
public class SteeringBehaviours : MonoBehaviour
{
    public ObstacleManager  obstacleManager = null;
    private BaseEntity      baseEntity      = null;

    //Toggle flags to enable / disable behaviours.
    public bool     bEnabled_Seek       = false;
    public float    fSeekWeight         = 0.5f;
    public bool     bEnabled_Arrive     = false;
    public float    fArriveWeight       = 0.5f;
    public bool     bEnabled_Flee       = false;
    public float    fFleeWeight         = 0.5f;
    public bool     bEnabled_Avoidance  = false;
    public float    fAvoidanceWeight    = 0.75f;
    public bool     bEnabled_Wander     = false;
    public float    fWanderWeight       = 0.25f;

    public bool     bEnabled_Flocking   = false;
    public float    fFlockingWeight     = 0.25f;

    //Target used for behaviours.
    public Transform targetTransform = null;
    public List<Transform> feelers = new List<Transform>();
    //public Vector2 targetPosition;

    public float fWander_Radius         = 5.0f;
    public float fWander_DistanceAhead  = 10.0f;

    public float fFlocking_Distance = 25.0f;
    public float fFlocking_FOV = -0.5f;

    public float fMaximumForce          = 10.0f;

    List<Vector2> whiskers = new List<Vector2>();
    public float fWhiskerMaxDistanceAhead   = 11.0f;
    public float fWhiskerAngle              = 45.0f;

    //--------------------------------------------------------------------------------------

    private void Start()
    {
        baseEntity = GetComponent<BaseEntity>();
    }

    //--------------------------------------------------------------------------------------

    public Vector2 GetCombinedForce()
    {
        //Delete me.
        return Vector2.zero;

        if (bEnabled_Avoidance)
        {
            //Add Avoidance code here.
        }

        if (bEnabled_Flocking)
        {
            //Add Flocking code here.
        }

        if (bEnabled_Seek)
        {
            //Add Seek code here.
        }

        if (bEnabled_Arrive)
        {
            //Add Arrive code here.
        }

        if (bEnabled_Flee)
        {
            //Add Flee code here.
        }

        if (bEnabled_Wander)
        {
            //Add Wander code here.
        }
    }

    //--------------------------------------------------------------------------------------

    Vector2 Seek(Vector2 targetPosition)
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Arrive(Vector2 targetPosition)
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Flee(Vector2 targetPosition)
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Avoidance()
    {
        //Early out if obstacle manager is not set up - We need this to be able to avoid obstacles.
        if (obstacleManager == null) { return Vector2.zero; }

        SetUpWhiskers();

        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    void SetUpWhiskers()
    {
        whiskers.Clear();

        //Speed modifier is a 0-1 value: If moving at maximum speed it wil be a 1.
        float fSpeedModifier = VectorMath.Magnitude(baseEntity.GetCurrentVelocity()) / baseEntity.GetMaxMoveSpeed();
        float fWhiskerDistanceAhead = fWhiskerMaxDistanceAhead * fSpeedModifier;

        //Whisker ahead.
        Vector2 vWhiskerPos = (Vector2)baseEntity.transform.position + (baseEntity.GetFacing() * fWhiskerDistanceAhead);
        whiskers.Add(vWhiskerPos);
        Debug.DrawRay(baseEntity.transform.position, (baseEntity.GetFacing() * fWhiskerDistanceAhead), Color.black, 0.0f, true);

        float rad = Mathf.Deg2Rad * fWhiskerAngle;
        //Whisker at angle forward and to the left.
        vWhiskerPos = (Vector2)baseEntity.transform.position + baseEntity.GetFacing();
        Vector2 vWhiskerDirection = VectorMath.RotateAroundAPoint((Vector2)baseEntity.transform.position, vWhiskerPos, rad);
        vWhiskerDirection -= (Vector2)baseEntity.transform.position;
        vWhiskerDirection = VectorMath.Normalize(vWhiskerDirection);
        vWhiskerPos = (Vector2)baseEntity.transform.position + (vWhiskerDirection * fWhiskerDistanceAhead);
        whiskers.Add(vWhiskerPos);
        Debug.DrawRay(baseEntity.transform.position, (vWhiskerDirection * fWhiskerDistanceAhead), Color.black, 0.0f, true);

        //Whisker at angle forward and to the right.
        vWhiskerPos = (Vector2)baseEntity.transform.position + baseEntity.GetFacing();
        vWhiskerDirection = VectorMath.RotateAroundAPoint((Vector2)baseEntity.transform.position, vWhiskerPos, -rad);
        vWhiskerDirection -= (Vector2)baseEntity.transform.position;
        vWhiskerDirection = VectorMath.Normalize(vWhiskerDirection);
        vWhiskerPos = (Vector2)baseEntity.transform.position + (vWhiskerDirection * fWhiskerDistanceAhead);
        whiskers.Add(vWhiskerPos);
        Debug.DrawRay(baseEntity.transform.position, (vWhiskerDirection * fWhiskerDistanceAhead), Color.black, 0.0f, true);

        //Whisker to the left.
        vWhiskerPos = (Vector2)baseEntity.transform.position + (-baseEntity.GetRight() * fWhiskerDistanceAhead);
        whiskers.Add(vWhiskerPos);
        Debug.DrawRay(baseEntity.transform.position, (-baseEntity.GetRight() * fWhiskerDistanceAhead), Color.black, 0.0f, true);

        //Whisker to the right.
        vWhiskerPos = (Vector2)baseEntity.transform.position + (baseEntity.GetRight() * fWhiskerDistanceAhead);
        whiskers.Add(vWhiskerPos);
        Debug.DrawRay(baseEntity.transform.position, (baseEntity.GetRight() * fWhiskerDistanceAhead), Color.black, 0.0f, true);
    }

    //--------------------------------------------------------------------------------------

    Vector2 Wander()
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Flocking()
    {
        List<BaseEntity> neighbourhood = new List<BaseEntity>();

        //------------------------------------------------------------------------
        //Find all base entities within within distance and field of view.
        for (int entityIndex = 0; entityIndex < transform.parent.childCount; entityIndex++)
        {
            Transform otherTransform = transform.parent.GetChild(entityIndex);

            //Ensure the transform is available and is not our own transform.
            if (otherTransform != null && otherTransform != transform)
            {
                BaseEntity otherBaseEntity = otherTransform.GetComponent<BaseEntity>();

                if (otherBaseEntity != null)
                {
                    Vector2 vecToOther = otherTransform.position - transform.position;

                    //Is this transform close enough to be considered in range?
                    if (VectorMath.Magnitude(vecToOther) < fFlocking_Distance)
                    {
                        Vector2 unitVecToOther = VectorMath.Normalize(vecToOther);

                        //Is this transform within the designated fov range?
                        if (VectorMath.Dot(baseEntity.GetFacing(), unitVecToOther) > fFlocking_FOV)
                        {
                            neighbourhood.Add(otherBaseEntity);
                        }
                    }
                }
            }
        }

        //If we have no entities in our neighbourhood, return a zero velocity.
        if(neighbourhood.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 desiredVelocity = Vector2.zero;
        desiredVelocity += Separation(neighbourhood);
        desiredVelocity += Alignment(neighbourhood);
        desiredVelocity += Cohesion(neighbourhood);

        return desiredVelocity;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Separation(List<BaseEntity> neighbourhood)
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Alignment(List<BaseEntity> neighbourhood)
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------

    Vector2 Cohesion(List<BaseEntity> neighbourhood)
    {
        //Delete me.
        return Vector2.zero;
    }

    //--------------------------------------------------------------------------------------
}