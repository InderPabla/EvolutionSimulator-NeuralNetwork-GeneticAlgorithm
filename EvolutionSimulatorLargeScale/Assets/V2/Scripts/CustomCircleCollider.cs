using UnityEngine;
using System.Collections;

public class CustomCircleCollider
{
    private float initialRadius;

    private Vector3 position;
    private float angle;

    private float veloForward;
    private float veloAngular;

    private float mass;
    private float energyDensity;

    private float worldDeltaTime;

    public CustomCircleCollider(float initialRadius, Vector3 position, float angle, float veloForward, float veloAngular, float mass, float energyDensity, float worldDetlaTime)
    {
        this.position = position;
        this.initialRadius = initialRadius;
        this.position = position;
        this.angle = angle;
        this.veloAngular = veloAngular;
        this.veloForward = veloForward;
        this.mass = mass;
        this.energyDensity = energyDensity;
        this.worldDeltaTime = worldDetlaTime;
    }

    public void Update(float accelForward, float accelAngular)
    {
        veloForward += accelForward * worldDeltaTime;
        veloAngular += accelAngular * worldDeltaTime;

        float unitAngle = angle - 90f;
        if (unitAngle > 180)
            unitAngle = (360f - unitAngle) * -1f;
        Vector3 newUnit = new Vector3(Mathf.Cos(unitAngle * Mathf.Deg2Rad), Mathf.Sin(unitAngle * Mathf.Deg2Rad), 0f);
  
        position += newUnit * veloForward * worldDeltaTime * 10f;
        angle += veloAngular * worldDeltaTime;
    }


    
}
