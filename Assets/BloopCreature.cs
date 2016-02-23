using UnityEngine;
using System.Collections;

public class BloopCreature : MonoBehaviour
{
    BloopBrain brain = new BloopBrain();
    public Transform food;
    public Transform pos1, pos2;
    void Start ()
    {
        brain.GenerationRandomBrain();
        ping();

    }
	
	void Update ()
    {
        
    }

    public void ping()
    {
        brain.inputLayers[0] = Vector3.Distance(pos1.transform.position, food.position);
        brain.inputLayers[1] = Vector3.Distance(pos2.transform.position, food.position);
        brain.ping();
        

        float out1 = brain.outputLayers[0];
        float out2 = brain.outputLayers[1];
        //Debug.Log(out1+" "+out2);
        if (out1 > out2)
            transform.eulerAngles += new Vector3(0,0,-2f);
        else if (out2>out1)
            transform.eulerAngles += new Vector3(0, 0, 2f);

        Invoke("ping",0.1f);
    }
}
