using UnityEngine;
using System;

public class Balancer : MonoBehaviour {

    public Rigidbody2D trackRigidbody;
    public Rigidbody2D pole1Rigidbody;
    public Rigidbody2D pole2Rigidbody;

    private Net net;
    private bool isActive = false;

    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    void Start () {

        /*net = new Net(6,1,10,10);
        net.SetRandomWeights();*/
	}

	void Update () {
        if (isActive == true) {
            UpdateNet(); //update neural net

            if (FailCheck() == true) {
                OnFinished();
            }
        }
    }

    public void Activate(Net net){
        this.net = net;
        isActive = true;
    }

    //Updates nerual net with new inputs from the balancer 
    private void UpdateNet() {
        float boardVelocity = trackRigidbody.velocity.x; //get current velocity of the board

        //both poles angles in radians
        float pole1AngleRadian = Mathf.Deg2Rad * pole1Rigidbody.transform.eulerAngles.z; 
        float pole2AngleRadian = Mathf.Deg2Rad * pole2Rigidbody.transform.eulerAngles.z; 

        //both poles angular velocities 
        float pole1AngularVelocity = pole1Rigidbody.angularVelocity;
        float pole2AngularVelocity = pole2Rigidbody.angularVelocity;

        float[] inputValues = {boardVelocity, pole1AngleRadian, pole2AngleRadian, pole1AngularVelocity, pole2AngularVelocity}; //gather pole and track data into an array 

        float[] output = net.FireNet(inputValues); //caluclate new neural net output with given input values

        trackRigidbody.velocity += new Vector2(output[0], 0); //update track velocity with neural net output

        net.AddNetFitness(Time.deltaTime);
    }

    //restrictions on the test to fail bad neural networks faster
    private bool FailCheck() {
        float pole1AngleDegree = pole1Rigidbody.transform.eulerAngles.z;
        float pole2AngleDegree = pole2Rigidbody.transform.eulerAngles.z;

        //if both poles are within 45 degrees on eaither side then fail check is false
        if (((pole1AngleDegree <= 45 && pole1AngleDegree >= 0) || (pole1AngleDegree <= 360 && pole1AngleDegree >= 315)) &&
            ((pole2AngleDegree <= 45 && pole2AngleDegree >= 0) || (pole2AngleDegree <= 360 && pole2AngleDegree >= 315))) {
            return false;
        }
        //if both poles are above 0 y then fail check is false
        else if(pole1Rigidbody.transform.localPosition.y>0 && pole2Rigidbody.transform.localPosition.y >0) {
            return false;
        }
        else {
            //one or both the pole(s) have fallen below 45 degrees or have fallen below 0 y, thus fail is true
            return true;
        }
    }

    //action based on neural net faling the test
    protected virtual void OnFinished() {
        if (TestFinished != null) {
            TestFinished(net, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
}
