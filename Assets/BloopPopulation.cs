using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloopPopulation : MonoBehaviour
{
    private object mutex = new object();
    public GameObject creaturePrefab;
    GameObject[] balancers;
    List<BloopBrain> brainList = new List<BloopBrain>();
    List<BloopBrain> brainListReturned = new List<BloopBrain>();
    int size = 100;
    int sizeCounter = 0;
    float[,] rankedFitness;
    int generationNumber = 1;
    void Start ()
    {
        Application.runInBackground = true;
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
        lock (mutex)
        {
            brainListReturned.Add(brain);
            if (sizeCounter == size)
            {
                //Debug.Log("Done");
                brainList.Clear();
                generationNumber++;
                for (int i = 0; i < size; i++)
                {
                    rankedFitness[i, 0] = i;
                    rankedFitness[i, 1] = brainListReturned[i].fitness;
                    Destroy(balancers[i]);
                }

                BubbleSortBrains();
                Debug.Log("BEST: " + rankedFitness[size - 1, 1]);
                if (rankedFitness[size - 1, 1] > 20f && generationNumber > 10)
                {
                    Time.timeScale = 1f;
                    balancers[0] = (GameObject)Instantiate(creaturePrefab);
                    balancers[0].transform.parent = transform;
                    balancers[0].SendMessage("ActivateWithBrainTestMode", brainListReturned[(int)rankedFitness[size - 1, 0]]);
                }
                else
                {
                    int[,] pairedIndcies = new int[size, 2];
                    for (int i = 0; i < (size / 2); i++)
                    {

                        pairedIndcies[i, 0] = Random.Range((size / 2), size);
                        pairedIndcies[i, 1] = Random.Range((size / 2), size);

                    }


                    for (int i = 0; i < (size / 2); i++)
                    {

                        int index1 = (int)rankedFitness[pairedIndcies[i, 0], 0];
                        int index2 = (int)rankedFitness[pairedIndcies[i, 1], 0];
                        BloopBrain[] cross = brainListReturned[index1].Crossover(brainListReturned[index2]);
                        brainList.Add(cross[0]);
                        brainList.Add(cross[1]);
                    }
                    brainListReturned.Clear();
                    sizeCounter = 0;
                    Next();
                    /*Debug.Log("BEST: " + rankedFitness[size - 1, 1]);
                    Time.timeScale = 1f;
                    balancers[0] = (GameObject)Instantiate(creaturePrefab);
                    balancers[0].transform.parent = transform;
                    balancers[0].SendMessage("ActivateWithBrainTestMode", brainListReturned[(int)rankedFitness[size - 1, 0]]);*/
                }
            }
            else
            {
                //Debug.Log(brain.fitness);
                Next();
            }
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
