using UnityEngine;
using System.Collections;

public class BloopCreature : MonoBehaviour
{
    BloopBrain brain;
    public Transform food;
    public Transform pos1, pos2;
    bool activate = false;
    void Start ()
    {
        //brain.GenerationRandomBrain();
        //ping();

    }
	

    void ActivateWithBrain(BloopBrain newBrain)
    {
        brain = newBrain;
        activate = true;
        Invoke("Finish",10f);
    }

	void Update ()
    {
        if(activate)
        {
            transform.position += transform.up * Time.deltaTime * 0.5f;

            brain.inputLayers[0] = Vector3.Distance(pos1.transform.position, food.position);
            brain.inputLayers[1] = Vector3.Distance(pos2.transform.position, food.position);
            brain.ping();


            float out1 = brain.outputLayers[0];
            float out2 = brain.outputLayers[1];
            //Debug.Log(out1+" "+out2);
            if (out1 > out2)
                transform.eulerAngles += new Vector3(0, 0, -1f);
            else if (out2 > out1)
                transform.eulerAngles += new Vector3(0, 0, 1f);
        }
    }

    public void Finish()
    {
        brain.fitness = Vector3.Distance(transform.position,food.position);
        transform.parent.SendMessage("UpdateCounter");
    }

    public void ping()
    {
        

        //Invoke("ping",0.1f);
    }
}
