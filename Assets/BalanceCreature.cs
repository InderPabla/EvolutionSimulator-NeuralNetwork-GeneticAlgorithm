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
        polePhysics.transform.eulerAngles = new Vector3(0f,0f,-35);
        brain = new BloopBrain();
        brain.GenerationRandomBrain();

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
    void Update () {
        if (activate)
        {
            fitTick += Time.deltaTime;
            brain.inputLayers[0] = boardPhysics.velocity.x;
            brain.inputLayers[1] = Mathf.Deg2Rad * polePhysics.transform.eulerAngles.z;
            brain.inputLayers[2] = polePhysics.angularVelocity;
            brain.ping();


            float speed = brain.outputLayers[0];
            boardPhysics.velocity += new Vector2(speed * 0.01f, 0);

            float angle = polePhysics.transform.eulerAngles.z;

            if (Mathf.Abs(polePhysics.transform.eulerAngles.z) >= 45 && testMode == false)
            {
                Finish();
            }

            
        }


    }

    void Finish()
    {
        brain.fitness = /*Mathf.Abs(polePhysics.transform.eulerAngles.z) + (Mathf.Abs(polePhysics.transform.eulerAngles.z)*boardPhysics.velocity.x)*/fitTick ;
        transform.parent.SendMessage("UpdateCounter",brain);
        Destroy(gameObject);
    }
}
