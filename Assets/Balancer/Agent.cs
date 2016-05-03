using UnityEngine;
using System;
using System.Collections.Generic;

public class Agent : MonoBehaviour {

    public List<Rigidbody2D> bodies;
    

    private Net net;
    private bool isActive = false;
    private const string ON_FINISHED = "OnFinished";

    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    void Start () {

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
        Invoke(ON_FINISHED,net.GetNetTestTime());
        isActive = true;
    }

    //action based on neural net faling the test
    protected virtual void OnFinished() {
        if (TestFinished != null) {
            TestFinished(net.GetNetID(), EventArgs.Empty);
            //TestFinished(null, EventArgs.Empty);
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
        bodies[0].velocity += new Vector2(output[0], 0);
        net.AddNetFitness(Time.deltaTime);*/

        float boardVelocity = bodies[0].velocity.x; //get current velocity of the board

        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * bodies[1].transform.eulerAngles.z; 
        float pole2AngleRadian = Mathf.Deg2Rad * bodies[2].transform.eulerAngles.z; 

        //both poles angular velocities 
        float pole1AngularVelocity = bodies[1].angularVelocity;
        float pole2AngularVelocity = bodies[2].angularVelocity;

        float[] inputValues = {boardVelocity, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity}; //gather pole and track data into an array 

        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values

        bodies[0].velocity += new Vector2(output[0], 0); //update track velocity with neural net output

        net.AddNetFitness(Time.deltaTime);
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
            return true; */

        int failDegree = 85;

        float pole1AngleDegree = bodies[1].transform.eulerAngles.z;
        float pole2AngleDegree = bodies[2].transform.eulerAngles.z;

        bool inControl = false;
        bool onTrack = false;

        //if both poles are within 45 degrees on eaither side then fail check is false
        if (((pole1AngleDegree <= failDegree && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= (360- failDegree))) &&
            ((pole2AngleDegree <= failDegree && pole2AngleDegree >= 0) || (pole2AngleDegree <= 360 && pole2AngleDegree >= (360 - failDegree)))) {
            inControl = true;
        }
        //if both poles are above 0 y then fail check is false
        if(bodies[1].transform.localPosition.y>0 && bodies[2].transform.localPosition.y >0) {
            onTrack = true;
        }

        
        if (inControl == true && onTrack == true)
            return false;
        else
            //one or both the pole(s) have fallen below 45 degrees or have fallen below 0 y, thus fail is true
            return true;

    }

    
}
