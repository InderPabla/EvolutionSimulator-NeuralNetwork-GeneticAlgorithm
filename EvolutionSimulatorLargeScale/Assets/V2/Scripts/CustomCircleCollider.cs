using UnityEngine;
using System.Collections;
using System;

public class CustomCircleCollider
{
    public float radius;

    public Vector3 position;
    public float rotation;
    
    public float veloForward;
    public float veloAngular;

    public float mass;

    private float worldDeltaTime;

    public CustomCircleCollider(float radius, Vector3 position, float angle, float veloForward, float veloAngular, float mass, float worldDetlaTime)
    {
        this.position = position;
        this.radius = radius;
        this.position = position;
        this.rotation = angle;
        this.veloAngular = veloAngular;
        this.veloForward = veloForward;
        this.mass = mass;
        this.worldDeltaTime = worldDetlaTime;
    }

    public void UpdateColliderPhysics(float accelForward, float accelAngular)
    {
        float unitAngle = rotation - 90f;
        Vector3 newUnit = new Vector3(Mathf.Cos(unitAngle * Mathf.Deg2Rad), Mathf.Sin(unitAngle * Mathf.Deg2Rad), 0f);

        /*veloForward += accelForward * worldDeltaTime /2f;
        veloAngular += accelAngular * worldDeltaTime * 10f;
        veloForward *= (1f-0.04f);
        veloAngular *= (1f-0.004f);*/
        
        veloForward = accelForward * worldDeltaTime * 10f;
        veloAngular = accelAngular * worldDeltaTime * 2000f;

        position += newUnit * veloForward /** worldDeltaTime * 10f*/;
        rotation += veloAngular /** worldDeltaTime *100f*/;
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

    public bool CollisionCheckWithCircle(float circleRadius, Vector2 circlePoint)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(circlePoint.x - position.x, 2f) + Mathf.Pow(circlePoint.y - position.y, 2f));


        if (distance <= (radius + circleRadius))
        {
            return true;
        }

        return false;
    }


    public float IsLineIntersectingWithCircle(Vector2 point1, Vector2 point2)
    {
        float dx, dy, A, B, C, det, t, indx, indy, cx, cy, inDistance, distance;
        Vector2 intersection1, intersection2;

        cx = position.x;
        cy = position.y;
        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
        C = (point1.x - cx) * (point1.x - cx) +
            (point1.y - cy) * (point1.y - cy) -
            radius * radius;

        det = B * B - 4 * A * C;
        if ((A <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);

            return -1f;
        }
        else if (det == 0)
        {
            // One solution.
            t = -B / (2 * A);
            intersection1 =
                new Vector2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = new Vector2(float.NaN, float.NaN);

            indx = intersection1.x - point1.x;
            indy = intersection1.y - point1.y;
            inDistance = Vector2.Distance(intersection1, point1);
            distance = Vector2.Distance(point2, point1);

            if (Mathf.Sign(dx) == Mathf.Sign(indx) && Mathf.Sign(dy) == Mathf.Sign(indy) && inDistance<=distance)
                return inDistance;
            else
                return -1f;
        }
        else
        {
            // Two solutions.
            t = (float)((-B + Math.Sqrt(det)) / (2 * A));
            intersection1 =
                new Vector2(point1.x + t * dx, point1.y + t * dy);
            t = (float)((-B - Math.Sqrt(det)) / (2 * A));
            intersection2 =
                new Vector2(point1.x + t * dx, point1.y + t * dy);

            indx = intersection1.x - point1.x;
            indy = intersection1.y - point1.y;
            inDistance = Vector2.Distance(intersection1, point1);
            distance = Vector2.Distance(point2, point1);

            if (Mathf.Sign(dx) == Mathf.Sign(indx) && Mathf.Sign(dy) == Mathf.Sign(indy) && inDistance <= distance)
                return inDistance;
            else
                return -1f;
        }
    }





    // Find the points of intersection.
    //public bool IsLineIntersectingWithCircle( Vector2 p1, Vector2 p2)
    //{
    /*float dx, dy, A, B, C, det;

    dx = point2.x - point1.x;
    dy = point2.y - point1.y;

    A = dx * dx + dy * dy;
    B = 2 * (dx * (point1.x - position.x) + dy * (point1.y - position.y));
    C = (point1.x - position.x) * (point1.x - position.x) +
        (point1.y - position.y) * (point1.y - position.y) -
        radius * radius;

    det = B * B - 4 * A * C;
    if ((A <= 0.0000001) || (det < 0))
    {
        return false;
    }

    return true;*/

    /*Vector2 p3 = new Vector2();
    p3.x = position.x + radius;
    p3.y = position.y + radius;

    float m = ((p2.y - p1.y) / (p2.x - p1.x));
    float Constant = (m * p1.x) - p1.y;

    float b = -(2 * ((m * Constant) + p3.x + (m * p3.y)));
    float a = (1 + (m * m));
    float c = ((p3.x * p3.x) + (p3.y * p3.y) - (radius * radius) + (2 * Constant * p3.y) + (Constant * Constant));
    float D = ((b * b) - (4 * a * c));

    if (D > 0)
    {

        float distance = Vector2.Distance(p1, p2);
        if (Vector2.Distance(p1,p3)<= distance && Vector2.Distance(p2, p3) <= distance)
            return true;
    }
    return false;
}*/

    public float GetRadius()
    {
        return radius;
    }
}
