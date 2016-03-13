using UnityEngine;
using System.Collections;

public class NEAT : MonoBehaviour
{
    Genome genome;
    public Rigidbody2D polePhysics1,polePhysics2;
    Rigidbody2D boardPhysics;
    // Use this for initialization
    void Start ()
    {
        boardPhysics = GetComponent<Rigidbody2D>();
        polePhysics1.transform.eulerAngles = new Vector3(0f, 0f, -20);
        polePhysics2.transform.eulerAngles = new Vector3(0f, 0f, -20);
        /*if (Random.Range(0, 2) == 0)
            polePhysics1.transform.eulerAngles = new Vector3(0f, 0f, -20);
        else
            polePhysics1.transform.eulerAngles = new Vector3(0f, 0f, 20);

        if (Random.Range(0, 2) == 0)
            polePhysics2.transform.eulerAngles = new Vector3(0f, 0f, -20);
        else
            polePhysics2.transform.eulerAngles = new Vector3(0f, 0f, 20);*/

        genome = new Genome(6, 1);
        genome.MakeRandom();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        genome.nodeList[0].nodeValue = boardPhysics.velocity.x;
        genome.nodeList[1].nodeValue = Mathf.Deg2Rad * polePhysics1.transform.eulerAngles.z;
        genome.nodeList[2].nodeValue = polePhysics1.angularVelocity;

        genome.nodeList[3].nodeValue = Mathf.Deg2Rad * polePhysics2.transform.eulerAngles.z;
        genome.nodeList[4].nodeValue = polePhysics2.angularVelocity;


        genome.feedforward();


        float speed = genome.nodeList[6].nodeValue;
        boardPhysics.velocity += new Vector2(speed, 0);

        float angle1 = polePhysics1.transform.eulerAngles.z;
        float angle2 = polePhysics2.transform.eulerAngles.z;

        bool failed = false;

        if (((angle1 <= 45 && angle1 >= 0) || (angle1 <= 360 && angle1 >= 315)) && ((angle2 <= 45 && angle2 >= 0) || (angle2 <= 360 && angle2 >= 315)))
        {
            failed = false;
        }
        else
            failed = true;

        if (failed)
            Destroy(gameObject);

        if (Input.GetMouseButtonDown(0))
        {
            genome.print();
        }

    }
}
