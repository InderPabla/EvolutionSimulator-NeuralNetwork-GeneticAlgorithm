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

    private float worldDetlaTime;

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
        this.worldDetlaTime = worldDetlaTime;
    }

    public void Update(float accelForward, float accelAngular)
    {
        veloForward += accelForward * worldDetlaTime;
        veloAngular += accelAngular * worldDetlaTime;

    }


    
}
