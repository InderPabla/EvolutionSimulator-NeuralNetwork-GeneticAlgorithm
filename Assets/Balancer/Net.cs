using UnityEngine;
using System.Collections;

public class Net  {

    private int inputLayerIndex = 0;
    private int outputLayerIndex;
    private int numberOfInputPerceptrons;
    private int numberOfOutputPerceptrons;
    private int numberOfHiddenLayers;
    private int numberOfHiddenPerceptrons;
    
    private Layer[] layers;

    public Net(int numberOfInputPerceptrons, int numberOfOutputPerceptrons, int numberOfHiddenLayers, int numberOfHiddenPerceptrons) {
        this.numberOfInputPerceptrons = numberOfInputPerceptrons;
        this.numberOfOutputPerceptrons = numberOfOutputPerceptrons;
        this.numberOfHiddenLayers = numberOfHiddenLayers;
        this.numberOfHiddenPerceptrons = numberOfHiddenPerceptrons;

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
}
 