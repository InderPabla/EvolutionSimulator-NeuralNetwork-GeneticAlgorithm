using UnityEngine;
using System.Collections;

public class Balancer : MonoBehaviour {

    public Rigidbody2D trackRigidbody;
    public Rigidbody2D pole1Rigidbody;
    public Rigidbody2D pole2Rigidbody;

    private Net net;
	
	void Start () {
        net = new Net(6,1);
        
	}
	
	
	void Update () {
	
	}
}
