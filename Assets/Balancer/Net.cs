using System;
using UnityEngine;

public class Net  {

    private int inputLayerIndex = 0;
    private int outputLayerIndex;

    private int numberOfInputPerceptrons;
    private int numberOfOutputPerceptrons;
    private int numberOfHiddenLayers;
    private int numberOfHiddenPerceptrons;

    private float netFitness;
    private float netTestTime;
    private int netID;

    private Layer[] layers;

    

    //Deep copy constructor 
    public Net(Net copyNet) {
        this.inputLayerIndex = copyNet.inputLayerIndex;
        this.outputLayerIndex = copyNet.outputLayerIndex;
        
        this.numberOfInputPerceptrons = copyNet.numberOfInputPerceptrons;
        this.numberOfOutputPerceptrons = copyNet.numberOfOutputPerceptrons;
        this.numberOfHiddenLayers = copyNet.numberOfHiddenLayers;
        this.numberOfHiddenPerceptrons = copyNet.numberOfHiddenPerceptrons;

        this.netFitness = copyNet.netFitness;
        this.netTestTime = copyNet.netTestTime;
        this.netID = copyNet.netID;

        this.layers = new Layer[2 + this.numberOfHiddenLayers];
        this.layers[this.inputLayerIndex] = new Layer(copyNet.layers[this.inputLayerIndex]);
        for (int i = 1; i < this.layers.Length; i++) {
            this.layers[i] = new Layer(copyNet.layers[i], this.layers[i - 1]);
        }
    }

    public Net(int netID, int numberOfInputPerceptrons, int numberOfOutputPerceptrons, int numberOfHiddenLayers, int numberOfHiddenPerceptrons, float netTestTime) {
        this.netID = netID;
        this.numberOfInputPerceptrons = numberOfInputPerceptrons;
        this.numberOfOutputPerceptrons = numberOfOutputPerceptrons;
        this.numberOfHiddenLayers = numberOfHiddenLayers;
        this.numberOfHiddenPerceptrons = numberOfHiddenPerceptrons;
        this.netTestTime = netTestTime;
        
        layers = new Layer[2+numberOfHiddenLayers];
        this.outputLayerIndex = layers.Length - 1;

        layers[inputLayerIndex] = new Layer(numberOfInputPerceptrons);
         
        for (int i = 1; i < layers.Length; i++) {
            if (IsOutputLayer(i)) {
                //if this layer is the output layer 
                layers[i] = new Layer(numberOfOutputPerceptrons, layers[i - 1], Layer.OUTPUT_LAYER);
            }
            else {
                //if this layer is the input layer
                layers[i] = new Layer(numberOfHiddenPerceptrons, layers[i - 1], Layer.HIDDEN_LAYER);
            }
        }

        this.netFitness = 0;      
    }

    //given index return true or false whether given index corresponds to the hidden layer
    public bool IsInputLayer(int index) {
        //it's the input layer if the index is the first index - 0
        if (index == 0)
            return true;
        else
            return false;
    }

    //given index return true or false whether given index corresponds to the output layer
    public bool IsOutputLayer(int index) {
        //it's the output layer if the index is the last index - size - 1
        if (index == layers.Length-1)
            return true;
        else
            return false;
    }

    

    //given index return true or false whether given index corresponds to the hidden layer
    public bool IsHiddenLayer(int index) {
        //it's the hidden layer if the index is between the last and first index
        if (index > 0 && index < layers.Length - 1)
            return true;
        else
            return false;
    }

    //calculate net results 
    public float[] FireNet(float[] inputValues) {
        layers[inputLayerIndex].SetPerceptronValues(inputValues);

        for (int i = 1; i < layers.Length; i++) {
            layers[i].FireLayer();
        }

        return layers[outputLayerIndex].GetAllPerceptronValues();
    }

    public void SetRandomWeights() {
        for (int i = 0; i < outputLayerIndex; i++) {
            layers[i].SetRandomWeights();
        }
    }

    public void AddNetFitness(float fitness){
        netFitness += fitness;
    }

    public void SetNetFitness(float fitness){
        netFitness = fitness;
    }

    public float GetNetFitness() {
        return netFitness;
    }

    public float GetNetTestTime() {
        return netTestTime;
    }

    public void SetNetID(int id) {
        this.netID = id;
    }

    public int GetNetID(){
        return netID;
    }

    public int GetNumberOfLayers() {
        return layers.Length;
    }

    public void NetMutate() {
        for (int i = 0; i < layers.Length-1; i++) {
            layers[i].LayerMutate();  
        }
    }

    internal static Net[] CrossOver(Net net1, Net net2) {
        Net[] children = new Net[2];
        children[0] = new Net(net1);
        children[1] = new Net(net2);

        children[0].NetMutate();
        children[1].NetMutate();
        /*int numberOfLayers = net1.numberOfHiddenLayers + 2;

        for (int i = 0; i < numberOfLayers-1; i++) {
            int numberOfPerceptrons = net1.layers[i].GetNumberOfPerceptrons();
            
        }*/

        return children;
    }

    public void ClearPerceptronValues() {
        for (int i = 0; i < layers.Length; i++){
            layers[i].ClearPerceptronValues();
        }
    }
}
 