using UnityEngine;
using System.Collections.Generic;
using System;

//Fast neural network based on matrix operations 
public class Brain_V2 : IEquatable<Brain_V2>
{

    private const float BIAS = 0.25f;

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

    private string name;
   
    private int ID;
    private int calculations; 

    public Brain_V2(int[] lay, int ID)
    {
        this.ID = ID;

        //deep copy layers array
        this.layers = new int[lay.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = lay[i];

            if (i > 0) {
                calculations += lay[i] * lay[i-1];
            }
        }

        //init neurons and weights matrix
        InitilizeNeurons();
        InitilizeWeights();
        GenerateRandomName();
    }

    // Deep copy constructor of a given Brain
    public Brain_V2(Brain_V2 parentBrain, int ID)
    {
        this.ID = ID;
        this.calculations = parentBrain.calculations;

        //deep copy layers array
        this.layers = new int[parentBrain.layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = parentBrain.layers[i];
        }

        //deep copy neurons
        this.neurons = new float[parentBrain.neurons.Length][];
        for (int i = 0; i < this.neurons.Length; i++)
        {
            this.neurons[i] = (float[])parentBrain.neurons[i].Clone();
        }

        //deep copy weights
        this.weights = new float[parentBrain.weights.Length][][];
        for (int i = 0; i < this.weights.Length; i++)
        {
            float[][] parentNeuronWeightsOfLayer = parentBrain.weights[i];
            float[][] weightsOfLayer = new float[parentNeuronWeightsOfLayer.Length][];

            for (int j = 0; j < weightsOfLayer.Length; j++)
            {
                weightsOfLayer[j] = (float[])parentNeuronWeightsOfLayer[j].Clone();
            }

            this.weights[i] = weightsOfLayer;
        }

        this.name = parentBrain.name;
    }

    //create a static neuron matrix
    private void InitilizeNeurons()
    {
        //Neuron Initilization
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++) //run through all layers
        {
            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
        }
        neurons = neuronsList.ToArray(); //convert list to array
    }

    //create a static weights matrix
    private void InitilizeWeights()
    {
        //Weights Initilization
        List<float[][]> weightsList = new List<float[][]>();

        for (int i = 1; i < neurons.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>(); //layer weights list

            int neuronsInPreviousLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                //set the weights randomly between 1 and -1
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray(); //convert list to array*/
    }

    //neural network feedword by matrix operation
    public float[] Feedforward(float[] inputs)
    {
        //add inputs to the neurons matrix
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }


        int weightLayerIndex = 0;

        // run through all neurons starting from the second layer
        for (int i = 1; i < neurons.Length; i++) //layers
        {
            for (int j = 0; j < neurons[i].Length; j++) //nerons
            {
                float value = BIAS ;

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[weightLayerIndex][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = Tanh(value);
            }
            weightLayerIndex++;
        }

        return neurons[layers.Length - 1]; //return output field
    }

    public float[][] GetNeurons()
    {
        return neurons;
    }

    //hyperbolic tangent activation
    private float Tanh(float value)
    {
        return (float)Math.Tanh(value);

        /*if (value < -3)
            return -1;
        else if (value > 3)
            return 1;
        else
            return value * (27f + value * value) / (27f + 9f * value * value);*/
    }

    //re return last float array in neurons matrix
    public float[] GetOutput()
    {
        return neurons[layers.Length - 1]; //return output field
    }

    //random name generation, with atlest 1 vowel per 3 letters 
    public void GenerateRandomName()
    {
        int nameSize = UnityEngine.Random.Range(3, 11);
        char[] name = new char[nameSize];

        int[] vowels = new int[] { 97, 101, 105, 111, 117 };
        int vowelCounter = 1;
        for (int i = 0; i < name.Length; i++)
        {
            int charNum = UnityEngine.Random.Range(97, 123);

            bool isVowel = false;
            for (int j = 0; j < vowels.Length; j++)
            {
                if (charNum == vowels[j])
                {
                    isVowel = true;
                    break;
                }
            }

            if (isVowel)
            {
                vowelCounter = 1;
            }
            else if (vowelCounter == 3)
            {
                vowelCounter = 1;
                charNum = vowels[UnityEngine.Random.Range(0, vowels.Length)];
            }

            vowelCounter++;
            name[i] = (char)charNum;
        }
        this.name = new string(name);
    }

    public string GetName()
    {
        return name;
    }

    public bool Equals(Brain_V2 other)
    {
        if (other == null)
            return false;

        return (other.ID == this.ID);
    }

    public void SetID(int ID)
    {
        this.ID = ID;
    }

    public void Mutate()
    {
        //Mutate weight, each weight has a 4%
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    int randomNumber1 = UnityEngine.Random.Range(1, /*250*/1000); //random number between 1 and 100
                    if (randomNumber1 <= 1)
                    { //if 1
                      //flip sign of weight
                        weight *= -1f;
                    }
                    else if (randomNumber1 <= 2)
                    { //if 2
                      //pick random weight between -1 and 1
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber1 <= 3)
                    { //if 3
                      //randomly increase by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if (randomNumber1 <= 4)
                    { //if 4
                      //randomly decrease by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    //if(weight>3f || weight <-3f)
                        //weight = UnityEngine.Random.Range(-0.5f, 0.5f);

                    weights[i][j][k] = weight;
                }
            }
        }

        //Mutate name
        int index = UnityEngine.Random.Range(0, name.Length);
        char[] nameChar = name.ToCharArray();
        List<char> nameCharList = new List<char>(nameChar);

        int randomNumber = UnityEngine.Random.Range(0, 3);
        if (randomNumber == 0)
        {
            nameChar[index] = (char)UnityEngine.Random.Range(97, 123);
            name = new string(nameChar);
        }
        else if (randomNumber == 1) {
            if (nameCharList.Count >= 4)
            {
                nameCharList.RemoveAt(UnityEngine.Random.Range(0,nameCharList.Count));
            
            }
            else
            {
                nameCharList.Add((char)UnityEngine.Random.Range(97, 123));
            }

            nameChar = nameCharList.ToArray();
            name = new string(nameChar);
        }
        else if (randomNumber == 2)
        {

            if (nameCharList.Count <10)
            {
                int locationToAdd = UnityEngine.Random.Range(0,nameCharList.Count);
                nameCharList.Insert(locationToAdd,(char)UnityEngine.Random.Range(97, 123));
            }
            else
            {
                nameCharList.RemoveAt(UnityEngine.Random.Range(0, nameCharList.Count));
            }

            nameChar = nameCharList.ToArray();
            name = new string(nameChar);

        }
    }

    public float[][][] GetWeights()
    {
        return weights;
    }

    public int GetCalculations()
    {
        return calculations;
    }
}
