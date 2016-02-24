using UnityEngine;
using System.Collections;

public class BloopBrain
{

    public float[] inputLayers, hiddenLayers, outputLayers;
    public float[,] inputWeights, hiddenWeights;
    /*int numberOfInputs = 3;
    int numberOfHidden = 3;
    int numberOfOutputs = 2;*/

    int numberOfInputs = 4;
    int numberOfHidden = 4;
    int numberOfOutputs = 1;
    public float fitness = 0;
    public BloopBrain()
    { 
        inputLayers = new float[numberOfInputs];
        hiddenLayers = new float[numberOfHidden];
        outputLayers = new float[numberOfOutputs];
        inputWeights = new float[numberOfHidden,numberOfInputs];
        hiddenWeights = new float[numberOfOutputs,numberOfHidden];

        inputLayers[numberOfInputs - 1] = 1f;
        hiddenLayers[numberOfHidden - 1] = 1f;
    }

    public void GenerationRandomBrain()
    {
        for (int i = 0; i < numberOfHidden; i++)
        {
            for (int j = 0; j < numberOfInputs; j++)
            {
                inputWeights[i,j] = Random.Range(-10f, 10f); ;
            }
            
        }

        for (int i = 0; i < numberOfOutputs; i++)
        {
            for (int j = 0; j < numberOfHidden; j++)
            {
                hiddenWeights[i, j] = Random.Range(-10f, 10f); ;
            }

        }
    }

    public void ping()
    {
        for (int i = 0; i < numberOfHidden; i++)
        {
            for (int j = 0; j < numberOfInputs; j++)
            {
                hiddenLayers[i] += (inputWeights[i,j] * inputLayers[j]);
            }
            hiddenLayers[i] = tanh(hiddenLayers[i]);
        }

        for (int i = 0; i < numberOfOutputs; i++)
        {
            for (int j = 0; j < numberOfHidden; j++)
            {
                outputLayers[i] += (hiddenWeights[i,j] * hiddenLayers[j]);
            }
            outputLayers[i] = tanh(outputLayers[i]);
        }

    }

    public float tanh(float x)
    {
        if (x > 20f)
            return 1;
        else if (x < -20f)
            return -1f;
        else
        {
            float a = Mathf.Exp(x);
            float b = Mathf.Exp(-x);
            return (a - b) / (a + b);
        }
    }
}
