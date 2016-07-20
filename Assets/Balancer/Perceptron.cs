using UnityEngine;
using System;

public class Perceptron {

    public const int SWAP = 0;
    public const int ONE_WAY = 1;

    //constant perceptron types
    public const int INPUT_PERCEPTRON = 0;
    public const int INPUT_BIAS_PERCEPTRON = 1;
    public const int HIDDEN_PERCEPTRON = 2;
    public const int HIDDEN_BIAS_PERCEPTRON = 3;
    public const int OUTPUT_PERCEPTRON = 4;

    private const float HYPERBOLIC_TANGENT_UPPER_INPUT = 1f;
    private const float HYPERBOLIC_TANGENT_LOWER_INPUT = -1f;

    private float value; //value of perceptron 
    private float[] weights; //weights of perceptron
    private float[] deltaWeight; //weights delta to be change by 
    private int perceptronType; //perceptron type
    private int perceptronIndex; //perceptron's index within its layer

    public Perceptron(Perceptron copyPerceptron) {
        this.value = copyPerceptron.value;  
        this.perceptronType = copyPerceptron.perceptronType; 
        this.perceptronIndex = copyPerceptron.perceptronIndex;
        
        if (copyPerceptron.weights != null) {
            this.weights = new float[copyPerceptron.weights.Length];
            for (int i = 0; i < this.weights.Length; i++){
                this.weights[i] = copyPerceptron.weights[i];
            }
        }
   
    }

    public Perceptron(int type, int index) {
        this.perceptronType = type;
        this.perceptronIndex = index;

        //if this perceptron's type is a bias type then give this perceptron a constant value of 1f
        if (this.perceptronType == INPUT_BIAS_PERCEPTRON || this.perceptronType == HIDDEN_BIAS_PERCEPTRON) {
            this.value = 1f;
        }
    }

    //set the number of connections for this perceptron which will be the weight of each connection 
    public void SetPerceptronWeights(int connectionSize) {
        weights = new float[connectionSize];
        deltaWeight = new float[connectionSize];
    }

    public void SetValue(float value) {
        this.value = value;
    }

    public float GetValue() {
        return value;
    }

    //Calculate new value for this perceptron based on previous layer's connection weights
    public void FeedForward(Perceptron[] previousLayerPerceptrons) {
        value = 0;
        for (int i = 0; i < previousLayerPerceptrons.Length; i++) {
            value += previousLayerPerceptrons[i].weights[perceptronIndex] * previousLayerPerceptrons[i].value;
        }
        //value = (float)Math.Tanh(value);
        value = (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
    }

    public void BackwardPassPerceptron(float[] delta) {
        for (int i = 0; i < weights.Length; i++) {
            /*float w = weights[i];
            weights[i] -= (0.5f * delta[i] * value);
            Debug.Log("Weight Change: "+i+" "+w+" "+weights[i]);*/
            deltaWeight[i] = (0.5f * delta[i] * value);
            //weights[i] -= deltaWeight[i];
        }
    }

    public void UpdateBackwardPass()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] -= deltaWeight[i];
        }
    }

    public void SetRandomWeights() {
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = UnityEngine.Random.Range(HYPERBOLIC_TANGENT_LOWER_INPUT, HYPERBOLIC_TANGENT_UPPER_INPUT);
        }
    }

    public void PerceptronMutate(){
        for (int i = 0; i <weights.Length; i++){
            int randomRate = UnityEngine.Random.Range(0,100);
            if (randomRate == 0) {
                weights[i] = UnityEngine.Random.Range(HYPERBOLIC_TANGENT_LOWER_INPUT, HYPERBOLIC_TANGENT_UPPER_INPUT);
            }
        }
    }

    public float GetWeightAtIndex(int index) {
        return weights[index];
    }

    /*public float GetRandomNumber(float minimum, float maximum)
    {
        System.Random random = new System.Random();
        return random.Nextfloat() * (maximum - minimum) + minimum;
    }*/

    internal static void CrossOver(Perceptron perceptron1, Perceptron perceptron2, int type)
    {
        int numberOfWeights = perceptron1.weights.Length;
        if (type == SWAP)
        {
            for (int i = 0; i < numberOfWeights; i++)
            {
                float tempWeight = perceptron1.weights[i];
                perceptron1.weights[i] = perceptron2.weights[i];
                perceptron2.weights[i] = tempWeight;
            }
        }
        else if (type == ONE_WAY) {
            for (int i = 0; i < numberOfWeights; i++)
            {
                perceptron1.weights[i] = perceptron2.weights[i];
            }
        }

    }
}
 