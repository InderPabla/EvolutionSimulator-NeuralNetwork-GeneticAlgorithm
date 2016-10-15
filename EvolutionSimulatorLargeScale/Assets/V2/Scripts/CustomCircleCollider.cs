using UnityEngine;
using System.Collections;
using System;

public class CustomCircleCollider
{
    public float radius;

    public Vector3 position;
    public float angle;

    public float veloForward;
    public float veloAngular;

    public float mass;

    private float worldDeltaTime;

    public CustomCircleCollider(float radius, Vector3 position, float angle, float veloForward, float veloAngular, float mass, float worldDetlaTime)
    {
        this.position = position;
        this.radius = radius;
        this.position = position;
        this.angle = angle;
        this.veloAngular = veloAngular;
        this.veloForward = veloForward;
        this.mass = mass;
        this.worldDeltaTime = worldDetlaTime;
    }

    public void UpdatePhysics(float accelForward, float accelAngular)
    {
        float unitAngle = angle - 90f;
        /*if (unitAngle > 180)
            unitAngle = (360f - unitAngle) * -1f;*/
        Vector3 newUnit = new Vector3(Mathf.Cos(unitAngle * Mathf.Deg2Rad), Mathf.Sin(unitAngle * Mathf.Deg2Rad), 0f);
        
        veloForward += accelForward * worldDeltaTime;
        veloAngular += accelAngular * worldDeltaTime;
        veloForward *= (1f-0.004f);
        veloAngular *= (1f-0.004f);

        
  
        position += newUnit * veloForward /** worldDeltaTime * 10f*/;
        angle += veloAngular /** worldDeltaTime *100f*/;
    }

    public bool CollisionCheckWithPoint(Vector2 point)
    {
        float radiusSqr = Mathf.Pow(radius, 2f);
        float distanceBetweenPointAndCircle = Mathf.Pow((position.x - point.x), 2f) + Mathf.Pow((position.y - point.y), 2f);

        if (distanceBetweenPointAndCircle <= radiusSqr)
        {
            return true;
        }

        return false;
    }
}
