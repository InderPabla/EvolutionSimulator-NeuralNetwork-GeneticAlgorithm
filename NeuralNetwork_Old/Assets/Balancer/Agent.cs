using UnityEngine;
using System;
using System.Collections.Generic;

public class Agent : MonoBehaviour {

    public List<Transform> bodies;
    

    private Net net;
    private bool isActive = false;
    private const string ON_FINISHED = "OnFinished";

    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    private Rigidbody2D body;
    float up = 0, right = 0, left = 0;

    public Vector3 locationTrack;
    void Start () {
        body = bodies[0].GetComponent<Rigidbody2D>();
        //body.velocity = new Vector2(0,0.1f);
        //body.angularVelocity = 1f;
       
    }

	void FixedUpdate () {
        if (isActive == true) {
            UpdateNet(); //update neural net

            if (FailCheck() == true) {
                OnFinished();
            }
        }
    }

    public void Activate(Net net){
        this.net = net;
        Invoke(ON_FINISHED,(float)net.GetNetTestTime());
        isActive = true;
    }

    //action based on neural net faling the test
    protected virtual void OnFinished() {
        if (TestFinished != null) {
            TestFinished(net.GetNetID(), EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    //--Add your own neural net update code here--//
    //Updates nerual net with new inputs from the agent
    private void UpdateNet() {
        /*float boardVelocity = bodies[0].velocity.x; //get current velocity of the board
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z;
        float pole1AngularVelocity = bodies[1].angularVelocity;
        float[] inputValues = { boardVelocity, pole1AngleRadian, pole1AngularVelocity };
        float[] output = net.FireNet(inputValues);
        bodies[0].velocity += new Vector2(output[0]/10f, 0);
        net.AddNetFitness(Time.deltaTime);*/


        RaycastHit2D hitUp = Physics2D.Raycast(bodies[0].position, bodies[0].transform.up);
        RaycastHit2D hitRight = Physics2D.Raycast(bodies[0].position, bodies[0].transform.right);
        RaycastHit2D hitLeft = Physics2D.Raycast(bodies[0].position, -bodies[0].transform.right) ;
        
        float distUp = 0, distRight = 0, distLeft = 0;

        if (hitUp.collider != null)
        {
            distUp = Vector2.Distance(bodies[0].position, hitUp.point);

            if (distUp <= 4)
            {
                up = distUp;
                distUp = 1;
                
            }
            else
            {
                distUp = 0;
                up = 100;
            }
        }
        else {
            distUp = 0;
            up = 100;
        }

        if (hitRight.collider != null)
        {
            distRight = Vector2.Distance(bodies[0].position, hitRight.point);

            if (distRight <= 4)
            {
                right = distRight;
                distRight = 1;
            }
            else
            {
                distRight = 0;
                right = 100;
            }
        }
        else
        {
            distRight = 0;
            right = 100;
        }

        if (hitLeft.collider != null)
        {
            distLeft = Vector2.Distance(bodies[0].position, hitLeft.point);

            if (distLeft <= 4)
            {
                left = distRight;
                distLeft = 1;
            }
            else
            {
                distLeft = 0;
                left = 100;
            }
        }
        else
        {
            distLeft = 0;
            left = 100;
        }

        //Debug.Log(distUp+" "+distRight+" "+distLeft);
        float distance = Vector3.Distance(bodies[0].position, locationTrack);
        float angle = bodies[0].transform.eulerAngles.z;

        //float[] inputValues = {distance, angle, distRight,distUp,distLeft};
        float[] inputValues = {distance,angle,bodies[0].position.x,bodies[0].position.y};
        float[] output = net.FireNet(inputValues);

        /*if (output[0] > output[1])
        {
            body.angularVelocity = 100f;
        }
        else {
            body.angularVelocity = -100f;
        }*/

        body.angularVelocity = output[0] * 100f;

        body.velocity = bodies[0].up * 2f;

        net.SetNetFitness(distance);
        //net.AddNetFitness(Time.deltaTime);

    }

    //--Add your own neural net fail code here--//
    //restrictions on the test to fail bad neural networks faster
    private bool FailCheck() {
        /*int failDegree = 50;
        bool inControl = false;
        bool onTrack = false;
        float pole1AngleDegree = bodies[1].transform.eulerAngles.z;

        if (((pole1AngleDegree <= failDegree && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= (360 - failDegree))))
        {
            inControl = true;
        }

        if (bodies[1].transform.localPosition.y > 0)
        {
            onTrack = true;
        }


        if (inControl == true && onTrack == true)
            return false;
        else
            return true;*/

        /*if (up <= 1 || right <= 1 || left <= 1)
            return true;*/

        return false;

    }

    
}
