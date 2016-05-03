using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class GeneticController : MonoBehaviour {

    public GameObject testPrefab;

    //input through unity editor
    public int numberOfInputPerceptrons = 0;
    public int numberOfOutputPerceptrons = 0;
    public int numberOfHiddenPerceptrons = 0;
    public int numberOfHiddenLayers = 0;
    public int populationSize = 0;
    public float testTime = 0;

    private Net[] nets; //array of neural networks 
    private int[] finishedNetID;
    private int testCounter; //counter for population testing
    private Semaphore finished; //mutex lock for when test if finished and updating test counter
    private const string ACTIVATE = "Activate";
    private int generationNumber = 0;

    void Start () {
        Application.runInBackground = true;
        if (ErrorCheck() == false) {
            testCounter = 0;
            finished = new Semaphore(1, 1);
            nets = new Net[populationSize];
            finishedNetID = new int[populationSize];

            GenerateInitialNets();
            GeneratePopulation();
        }
    }
	
	void Update () {
	
	}

    public void GenerateInitialNets(){
        for (int i = 0; i < populationSize; i++){
            nets[i] = new Net(i, numberOfInputPerceptrons, numberOfOutputPerceptrons, numberOfHiddenLayers, numberOfHiddenPerceptrons, testTime);
            nets[i].SetRandomWeights();
        }
    }

    public void GeneratePopulation() {
        for (int i = 0; i < populationSize; i++) {
            GameObject agent = (GameObject)Instantiate(testPrefab, new Vector3(0, /*(-populationSize/2)+i*/0, 0), testPrefab.transform.rotation);
            agent.SendMessage(ACTIVATE, nets[i]);
            agent.GetComponent<Agent>().TestFinished += OnFinished; //suscribe OnFinished to event in Balancer
        }
    }

    public void OnFinished(object source, EventArgs e) {
        finished.WaitOne();

        finishedNetID[testCounter] = (int)source;
        testCounter++;
        if (testCounter == populationSize) {
            TestFinished();
        }

        finished.Release();
    }

    public void TestFinished() {
        testCounter = 0;
        generationNumber++;

        Net[] tempNet = new Net[populationSize];
        List<int> unusedIndicies = GenerateListNumbers(populationSize/2, populationSize-1);
        int[,] paires = GenerateRandomUniquePaires(unusedIndicies);
        Net[] newNets = new Net[populationSize];
        int pairIndex = 0;

        Debug.Log("Generation Number: "+generationNumber+", Best Fitness: "+ nets[finishedNetID[populationSize-1]].GetNetFitness());

        for (int i = 0; i < newNets.Length; i+=4) {
            int index1 = finishedNetID[paires[pairIndex, 0]];
            int index2 = finishedNetID[paires[pairIndex, 1]];

            Net[] corssover = Net.CrossOver(nets[index1], nets[index2]);

            newNets[i] = nets[index1];
            newNets[i+1] = nets[index2];
            newNets[i+2] = corssover[0];
            newNets[i+3] = corssover[1];

            pairIndex++;
        }

        for (int i = 0; i < newNets.Length; i++){
            nets[i] = newNets[i];
            nets[i].SetNetFitness(0);
            nets[i].SetNetID(i);
            nets[i].ClearPerceptronValues();
        }
        Invoke("GeneratePopulation",1);
        //GeneratePopulation();
    }

    public List<int> GenerateListNumbers(int min, int max) {
        List<int> unusedIndicies = new List<int>();
        for (int i = min; i <= max; i++) {
            unusedIndicies.Add(i);
        }
        return unusedIndicies;
    }

    public int[,] GenerateRandomUniquePaires(List<int> indicies) {
        int[,] paires = new int[indicies.Count/2,2];
        int count = indicies.Count / 2;

        for (int i = 0; i < count; i++) {
            int index1, index2, item1, item2;

            index1 = UnityEngine.Random.Range(0, indicies.Count);
            item1 = indicies[index1];
            indicies.RemoveAt(index1);

            index2 = UnityEngine.Random.Range(0, indicies.Count);
            item2 = indicies[index2];
            indicies.RemoveAt(index2);

            paires[i, 0] = item1;
            paires[i, 1] = item2;
        }

        return paires;
    }

    public bool ErrorCheck() {
        bool error = false;

        if (numberOfInputPerceptrons <= 0) {
            Debug.LogError("Need one or more input perceptrons.");
            error = true;
        }

        if (numberOfOutputPerceptrons <= 0) {
            Debug.LogError("Need one or more output perceptrons.");
            error = true;
        }

        if (numberOfHiddenPerceptrons <= 0) {
            Debug.LogError("Need one or more hidden perceptrons.");
            error = true;
        }

        if (numberOfHiddenLayers <= 0) {
            Debug.LogError("Need one or hidden layers.");
            error = true;
        }

        if (populationSize <= 0) {
            Debug.LogError("Population size must be greater than 0.");
            error = true;
        }

        if (populationSize % 2 != 0 || populationSize % 4 != 0) {
            Debug.LogError("Population size must be divisible by 2 and 4.");
            error = true;
        }

        if (testTime <= 0) {
            Debug.LogError("Time to test must be greater than 0 seconds.");
            error = true;
        }

        if (error){
            Debug.LogError("One or more issues found. Exiting.");
            return true;
        }
        else {
            return false;
        }
    }
}
