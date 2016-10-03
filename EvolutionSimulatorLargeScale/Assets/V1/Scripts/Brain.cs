using UnityEngine;
using System.Collections.Generic;
using System;

//Fast neural network based on matrix operations 
public class Brain : IEquatable<Brain>{

    private const float BIAS = 0.25f;

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    
    private string name;

    private float currentEnergy = 100f;
    private float initialEnergy = 100f;
    private float deltaEnergy = 0f;

    private float size = 0.25f;
    private float initialSize = 0.25f;

    private float worldDeltaTime = 1f;
    private int ID;

    public Brain(int[] lay, float delta, int ID)
    {
        this.worldDeltaTime = delta;
        this.ID = ID;

        //deep copy layers array
        this.layers = new int[lay.Length];
        for (int i = 0; i < layers.Length; i++) {
            this.layers[i] = lay[i];
        }

        //init neurons and weights matrix
        InitilizeNeurons(); 
        InitilizeWeights();
        GenerateRandomName();
    }

    // Deep copy constructor of a given Brain
    public Brain(Brain parentBrain, int ID)
    {
        this.worldDeltaTime = parentBrain.worldDeltaTime;
        this.ID = ID;

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

            int neuronsInPreviousLayer = layers[i-1];
            
            for (int j = 0; j < neurons[i].Length; j++) 
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                //set the weights randomly between 1 and -1
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-1f, 1f);
                }

                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray(); //convert list to array*/
    }

    //neural network feedword by matrix operation
    public float[] feedforward(float[] inputs)
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
                float value = BIAS;

                for (int k = 0; k < neurons[i-1].Length; k++) 
                {
                    value += weights[weightLayerIndex][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = tanh(value);
            }
            weightLayerIndex++;
        }

        return neurons[layers.Length - 1]; //return output field
    }

    //hyperbolic tangent activation
    private float tanh(float value)
    {
        return (float)Math.Tanh(value);
    }

    //re return last float array in neurons matrix
    public float[] GetOutput()
    {
        return neurons[layers.Length - 1]; //return output field
    }

    //return creature size
    public float GetSize()
    {
        return size;
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

    public void SetWorldDeltaTime(float delta)
    {
        worldDeltaTime = delta;
    }

    public string GetName()
    {
        return name;
    }

    public float GetEnergy()
    {
        return currentEnergy;
    }

    public void NaturalEnergyLoss(float factor)
    {
        deltaEnergy -= (((Time.fixedDeltaTime * worldDeltaTime * 100f) *(size/initialSize))* factor); 
    }

    public void ResetDeltaEnergy()
    {
        deltaEnergy = 0f;
    }


    public void Eat(float energy)
    {
        //this.deltaEnergy += (energy - ((Time.deltaTime * 100f)/2f));
        this.deltaEnergy += (energy/2f);
    }

    public void Move(float accel)
    {
        //this.deltaEnergy -= (Mathf.Abs(accel));
        
    }

    public float CalculateSize() {
        if (currentEnergy < initialEnergy)
        {
            size = initialSize;
        }
        else
        {
            size = initialSize + (currentEnergy / initialEnergy)*0.014f;
        }
       
        return size;
    }

    
    public void ApplyDeltaEnergy() {
        currentEnergy += deltaEnergy;
    }

    public bool Equals(Brain other)
    {
        if (other == null)
            return false;

        return (other.ID == this.ID);
    }

    public void BirthEnergyLoss()
    {
        deltaEnergy -= 150f;
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

                    int randomNumber = UnityEngine.Random.Range(1, 101); //random number between 1 and 100
                    if (randomNumber <= 1)
                    { //if 1
                      //flip sign of weight
                        weight *= -1f;
                    }
                    else if (randomNumber <= 2)
                    { //if 2
                      //pick random weight between -1 and 1
                        weight = UnityEngine.Random.Range(-1f, 1f);
                    }
                    else if (randomNumber <= 3)
                    { //if 3
                      //randomly increase by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if (randomNumber <= 4)
                    { //if 4
                      //randomly decrease by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    weights[i][j][k] = weight;
                }
            }
        } 

    }
}
