using UnityEngine;
using System.Collections.Generic;
using System;

//Fast neural network based on matrix operations 
public class Brain {

    private const float BIAS = 0.25f;

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    
    private string name;
    private float energy = 100f;
    private float size = 0.5f;

    public Brain(int[] lay)
    {
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
            float[][] layerWeights; //layer weights array

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

    public void Eat(float energy)
    {
        this.energy += energy;
        //this.energy -= 5f;
    }

    public string GetName()
    {
        return name;
    }

    public float GetEnergy()
    {
        return energy;
    }

    public void NaturalEnergyLoss()
    {
        //energy -= 1f;
    }

    public void Move(float speed)
    {
        //energy -= Mathf.Abs(speed);
    }

}
