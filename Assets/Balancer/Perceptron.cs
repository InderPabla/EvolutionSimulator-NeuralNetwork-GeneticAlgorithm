using UnityEngine;
using System.Collections;

public class Perceptron {

    //constant perceptron types
    public const int INPUT_PERCEPTRON = 0;
    public const int INPUT_BIAS_PERCEPTRON = 1;
    public const int HIDDEN_PERCEPTRON = 2;
    public const int HIDDEN_BIAS_PERCEPTRON = 3;
    public const int OUTPUT_PERCEPTRON = 4;

    private float value; //value of perceptron 
    private float[] weights; //weights of perceptron 
    private int perceptronType; //perceptron type

    public Perceptron(int type) {
        this.perceptronType = type;

        //if this perceptron's type is a bias type then give this perceptron a constant value of 1f
        if (this.perceptronType == INPUT_BIAS_PERCEPTRON || this.perceptronType == HIDDEN_BIAS_PERCEPTRON) {
            this.value = 1f;
        }
    }

    //set the number of connections for this perceptron which will be the weight of each connection 
    public void SetPerceptronWeights(int connectionSize) {
        weights = new float[connectionSize];
    }
}
 