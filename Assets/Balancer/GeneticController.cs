using UnityEngine;
using System;
using System.Threading;

public class GeneticController : MonoBehaviour {

    public GameObject testPrefab;

    //input through unity editor
    public int numberOfInputPerceptrons = 0;
    public int numberOfOutputPerceptrons = 0;
    public int numberOfHiddenPerceptrons = 0;
    public int numberOfHiddenLayers = 0;
    public int populationSize = 0;

    private Net[] nets; //array of neural networks 
    private int testCounter; //counter for population testing
    private Semaphore finished; //mutex lock for when test if finished and updating test counter

    void Start () {
        testCounter = 0;
        finished = new Semaphore(1,1);
        nets = new Net[populationSize];

        //create population to test
        for (int i = 0; i < populationSize; i++) {
            nets[i] = new Net(numberOfInputPerceptrons, numberOfOutputPerceptrons, numberOfHiddenLayers, numberOfHiddenPerceptrons);
            nets[i].SetRandomWeights();

            GameObject testObject = (GameObject)Instantiate(testPrefab, new Vector3(0, i, 0), testPrefab.transform.rotation);
            testObject.SendMessage("Activate", nets[i]);
            testObject.GetComponent<Balancer>().TestFinished += OnFinished; //suscribe OnFinished to event in Balancer
        }

    }
	
	void Update () {
	
	}

   public void OnFinished(object source, EventArgs e) {
        finished.WaitOne();
        /*Net net = (Net)source;
        Debug.Log(net.GetNetFitness() + " Fits");*/
        testCounter++;
        if (testCounter == populationSize) {
            Debug.Log("Test finished");
        }
        finished.Release();
   }


}
