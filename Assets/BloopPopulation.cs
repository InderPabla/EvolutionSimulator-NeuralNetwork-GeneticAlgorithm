using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloopPopulation : MonoBehaviour
{
    public GameObject creaturePrefab;
    GameObject[] balancers;
    List<BloopBrain> brainList = new List<BloopBrain>();
    List<BloopBrain> brainListReturned = new List<BloopBrain>();
    int size = 100;
    int sizeCounter = 0;
    float[,] rankedFitness;
    void Start ()
    {
        balancers = new GameObject[size];
        rankedFitness = new float[size, 2];
        for (int i = 0; i< size; i++)
        {
            BloopBrain brain = new BloopBrain();
            brain.GenerationRandomBrain();
            brainList.Add(brain);
        }
        Next();
    }
	
	void Update ()
    {
	    
	}

    public void UpdateCounter(BloopBrain brain)
    {
        brainListReturned.Add(brain);
        if (sizeCounter == size)
        {
            //Debug.Log("Done");
            brainList.Clear();
            
            for (int i = 0; i < size; i++)
            {
                rankedFitness[i, 0] = i;
                rankedFitness[i, 1] = brainListReturned[i].fitness;
                Destroy(balancers[i]);
            }

            BubbleSortBrains();
            Debug.Log(rankedFitness[size-1,1]);
            Time.timeScale = 1f;
            balancers[0] = (GameObject)Instantiate(creaturePrefab);
            balancers[0].transform.parent = transform;
            balancers[0].SendMessage("ActivateWithBrainTestMode", brainListReturned[(int)rankedFitness[size - 1, 0]]);
        }
        else
        {
            //Debug.Log(brain.fitness);
            Next();
        }
    }

    public void Next()
    {
        balancers[sizeCounter] = (GameObject)Instantiate(creaturePrefab);
        balancers[sizeCounter].transform.parent = transform;
        balancers[sizeCounter].SendMessage("ActivateWithBrain", brainList[sizeCounter]);
        sizeCounter++;
    }

    public void BubbleSortBrains()
    {
        float[] tempFitness = new float[2];
        bool swapped = true;
        int j = 0;
        while (swapped)
        {
            swapped = false;
            j++;
            for (int i = 0; i < size - j; i++)
            {
                if (rankedFitness[i, 1] > rankedFitness[i + 1, 1])
                {
                    tempFitness[0] = rankedFitness[i, 0];
                    tempFitness[1] = rankedFitness[i, 1];

                    rankedFitness[i, 0] = rankedFitness[i + 1, 0];
                    rankedFitness[i, 1] = rankedFitness[i + 1, 1];

                    rankedFitness[i + 1, 0] = tempFitness[0];
                    rankedFitness[i + 1, 1] = tempFitness[1];
                    swapped = true;
                }
            }
        }
    }
}
