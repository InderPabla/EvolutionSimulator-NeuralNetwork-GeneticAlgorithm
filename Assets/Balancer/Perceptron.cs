using UnityEngine;

public class Perceptron {

    //constant perceptron types
    public const int INPUT_PERCEPTRON = 0;
    public const int INPUT_BIAS_PERCEPTRON = 1;
    public const int HIDDEN_PERCEPTRON = 2;
    public const int HIDDEN_BIAS_PERCEPTRON = 3;
    public const int OUTPUT_PERCEPTRON = 4;

    private const float HYPERBOLIC_TANGENT_UPPER_INPUT = 20f;
    private const float HYPERBOLIC_TANGENT_LOWER_INPUT = -20f;

    private float value; //value of perceptron 
    private float[] weights; //weights of perceptron 
    private int perceptronType; //perceptron type
    private int perceptronIndex; //perceptron's index within its layer

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
        value = tanh(value);
    }

    public void SetRandomWeights() {
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = Random.Range(HYPERBOLIC_TANGENT_LOWER_INPUT, HYPERBOLIC_TANGENT_UPPER_INPUT);
        }
    }

    //hyperbolic tangent approximation
    private float tanh(float x){
        if (x > HYPERBOLIC_TANGENT_UPPER_INPUT) {
            return 1;
        }
        else if (x < HYPERBOLIC_TANGENT_LOWER_INPUT) {
            return -1f;
        }
        else {
            float a = Mathf.Exp(x);
            float b = Mathf.Exp(-x);
            return (a - b) / (a + b);
        }
    }
}
 