using UnityEngine;
using System.Collections;

public class BalanceCreature : MonoBehaviour {
    public Rigidbody2D polePhysics;
    Rigidbody2D boardPhysics;
    BloopBrain brain;
    // Use this for initialization
    bool activate = false;
    float fitTick = 0;

    bool testMode = false;
	void Start () {
        boardPhysics = GetComponent<Rigidbody2D>();
        if(Random.Range(0,2) == 0)
            polePhysics.transform.eulerAngles = new Vector3(0f,0f,Random.Range(-25,-19));
        else 
            polePhysics.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(20, 26));
        /*brain = new BloopBrain();
        brain.GenerationRandomBrain();*/

        //velocity of board
        //angle of pole
        //angular vel of pole


    }
	

    public void ActivateWithBrain(BloopBrain brain)
    {
        this.brain = brain;
        activate = true;
        
        Invoke("Finish",10f);
    }

    public void ActivateWithBrainTestMode(BloopBrain brain)
    {
        this.brain = brain;
        activate = true;
        testMode = true;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (activate)
        {
            fitTick += Time.deltaTime;
            brain.inputLayers[0] = boardPhysics.velocity.x;
            brain.inputLayers[1] = Mathf.Deg2Rad * polePhysics.transform.eulerAngles.z;
            brain.inputLayers[2] = polePhysics.angularVelocity;
            brain.ping();


            float speed = brain.outputLayers[0];
            boardPhysics.velocity += new Vector2(speed, 0);
            float angle = polePhysics.transform.eulerAngles.z;
            bool failed = false;

            if ((angle <= 45 && angle >=0) || (angle<=360 && angle>=315))
            {
                failed = false;
            }
            else
                failed = true;
            

            if ((failed || polePhysics.transform.position.y <= 0) && testMode == false)
            {
                Finish();  
            }

            
        }


    }

    void Finish()
    {
        //brain.fitness = /*Mathf.Abs(polePhysics.transform.eulerAngles.z) + (Mathf.Abs(polePhysics.transform.eulerAngles.z)*boardPhysics.velocity.x)*/(fitTick - polePhysics.angularVelocity) - (Mathf.Abs(polePhysics.transform.eulerAngles.z)*Mathf.Deg2Rad);
        brain.fitness = fitTick;
        transform.parent.SendMessage("UpdateCounter",brain);
        Destroy(gameObject);
    }
}
