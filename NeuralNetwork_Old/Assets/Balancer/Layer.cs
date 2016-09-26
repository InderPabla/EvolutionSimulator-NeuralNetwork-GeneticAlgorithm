
using UnityEngine;

public class Layer {

    // constants for types of layers
    public const int INPUT_LAYER = 0; 
    public const int OUTPUT_LAYER = 1;
    public const int HIDDEN_LAYER = 2;

    public const int LAYER_FULL_CROSSOVER = 0;
    public const int LAYER_PARTIAL_CROSSOVER = 1;
    public const int LAYER_PARTIAL_PERCEPTRON_CROSSOVER = 2;

    private Layer previousLayer; //previous layer pointer which will be used to feedforward the neural network

    private int numberOfPerceptrons; //number of perceptrons on this layer

    private Perceptron[] perceptrons; //perceptrons array

    private int layerType; //this layers type

    //Deepy copy constructor  1
    public Layer(Layer copyLayer) {
        this.numberOfPerceptrons = copyLayer.numberOfPerceptrons;
        this.layerType = copyLayer.layerType;

        this.perceptrons = new Perceptron[this.numberOfPerceptrons];
        for (int i = 0; i < this.numberOfPerceptrons; i++) {
            this.perceptrons[i] = new  Perceptron(copyLayer.perceptrons[i]);
        }
    }

    //Deepy copy constructor  2
    public Layer(Layer copyLayer, Layer previousLayer) {
        this.numberOfPerceptrons = copyLayer.numberOfPerceptrons;
        this.previousLayer = previousLayer;
        this.layerType = copyLayer.layerType;

        this.perceptrons = new Perceptron[this.numberOfPerceptrons];
        for (int i = 0; i < this.numberOfPerceptrons; i++) {
            this.perceptrons[i] = new Perceptron(copyLayer.perceptrons[i]);
        }
    }

    //This is the constructor for input layer since it does not need a previous layer
    public Layer(int numberOfPerceptrons) {
        this.numberOfPerceptrons = numberOfPerceptrons;
        this.layerType = INPUT_LAYER;

        InitializePerceptrons(); //initialize perceptron for this layer 
    }

    //This is the constructor for output and hidden layers
    public Layer(int numberOfPerceptrons, Layer previousLayer, int type) {
        this.numberOfPerceptrons = numberOfPerceptrons; 
        this.previousLayer = previousLayer;
        this.layerType = type;

        if (this.layerType == OUTPUT_LAYER) {
            //if this is the output layer, then there is no bias perceptron on this layer
            previousLayer.SetWeightsConnection(this.numberOfPerceptrons); 
        }
        else {
            //else is this the hidden layer and thus subtract to remove bias connection
            previousLayer.SetWeightsConnection(this.numberOfPerceptrons - 1);
        }

        InitializePerceptrons(); //initialize perceptron for this layer      
    }

    //initialize perceptron types based on current layer type
    private void InitializePerceptrons() {
        perceptrons = new Perceptron[numberOfPerceptrons];

        //create all perceptrons with their respected type 
        for (int i = 0; i < perceptrons.Length; i++) {
            bool isLast = i == perceptrons.Length-1; //last perceptron for input and hidden will be bias perceptron
            int perceptronType = -1;

            //switch statement for the 3 layers
            switch (layerType) {
                case INPUT_LAYER:
                    //determine input perceptron type 
                    if (isLast)
                        perceptronType = Perceptron.INPUT_BIAS_PERCEPTRON; 
                    else
                        perceptronType = Perceptron.INPUT_PERCEPTRON;
                    break;
                case OUTPUT_LAYER:
                    perceptronType = Perceptron.OUTPUT_PERCEPTRON; //output perceptron only has one type since there is no bias output neuron
                    break;
                case HIDDEN_LAYER:
                    //determine hidden perceptron type 
                    if (isLast)
                        perceptronType = Perceptron.HIDDEN_BIAS_PERCEPTRON; 
                    else
                        perceptronType = Perceptron.HIDDEN_PERCEPTRON;
                    break;
            }
            perceptrons[i] = new Perceptron(perceptronType, i); //instantiate perceptron 
        }
    }

    public void SetPerceptronValues(float[] values) {
        //set values for all perceptrons EXCEPT the last perceptron which is the bias
        for (int i = 0; i < perceptrons.Length - 1; i++) {
            perceptrons[i].SetValue(values[i]);
        }
    }

    public void FireLayer() {
        int size;

        if (layerType == OUTPUT_LAYER) {
            size = perceptrons.Length;
        }
        else {
            size = perceptrons.Length - 1;
        }

        for (int i = 0; i < size; i++) {
            perceptrons[i].FeedForward(previousLayer.perceptrons);
        }
    }

    public void BackwardPassLayer(float[] error) {
        float[] deltaOutput = new float[perceptrons.Length];

        if (layerType == OUTPUT_LAYER) {
            
            for (int i = 0; i < deltaOutput.Length; i++) {
                //deltaOutput[i] = error[i] * (1f - Mathf.Pow(perceptrons[i].GetValue(), 2f));
                deltaOutput[i] = error[i] * (perceptrons[i].GetValue() - Mathf.Pow(perceptrons[i].GetValue(), 2f));
            }

            previousLayer.UpdateWeights(deltaOutput);
        }
        else {

            for (int i = 0; i < deltaOutput.Length; i++) {
                float del = 0;
                for (int j = 0; j < error.Length; j++) {
                    del += error[j] * perceptrons[i].GetWeightAtIndex(j);
                }

                //deltaOutput[i] = del*(1f - Mathf.Pow(perceptrons[i].GetValue(), 2f));
                deltaOutput[i] = del * (perceptrons[i].GetValue() - Mathf.Pow(perceptrons[i].GetValue(), 2f));
            }
            previousLayer.UpdateWeights(deltaOutput);
        }

        if (previousLayer.layerType != INPUT_LAYER) {
            previousLayer.BackwardPassLayer(deltaOutput);
        }
    }

    public void UpdateWeights(float[] delta) {
        for (int i = 0; i < perceptrons.Length; i++) {
            perceptrons[i].BackwardPassPerceptron(delta);
        }
    }

    public void UpdateBackwardPass()
    {
        for (int i = 0; i < perceptrons.Length; i++)
        {
            perceptrons[i].UpdateBackwardPass();
        }
    }

    public float[] GetAllPerceptronValues() {
        float[] values = new float[perceptrons.Length];

        for (int i = 0; i < perceptrons.Length; i++) {
            values[i] = perceptrons[i].GetValue();
        }

        return values;
    }

    //set weight size for each perceptron with given number of connections
    private void SetWeightsConnection(int numberOFConnections) {
        for (int i = 0; i < perceptrons.Length; i++) {
            perceptrons[i].SetPerceptronWeights(numberOFConnections);
        }
    }

    public void SetRandomWeights() {
        for (int i = 0; i < perceptrons.Length; i++) {
            perceptrons[i].SetRandomWeights();
        }
    }

    public int GetNumberOfPerceptrons() {
        return numberOfPerceptrons;
    }

    public void LayerMutate() {
        for (int i = 0; i < perceptrons.Length; i++) {
            perceptrons[i].PerceptronMutate();
        }
    }

    public void ClearPerceptronValues(){
        int size;
        if (layerType == OUTPUT_LAYER)
            size = perceptrons.Length;
        else
            size = perceptrons.Length - 1;

        for (int i = 0; i < size; i++){
            perceptrons[i].SetValue(0);
        }
    }

    internal static void CrossOver(Layer layer1, Layer layer2, int type) {
        int numberOfPerceptrons = layer1.numberOfPerceptrons;

        if (type == LAYER_FULL_CROSSOVER) {

            for (int i = 0; i < numberOfPerceptrons; i++) {
                Perceptron.CrossOver(layer1.perceptrons[i], layer2.perceptrons[i],Perceptron.SWAP);
            }

        }
        else if (type == LAYER_PARTIAL_CROSSOVER) {
            int randomIndex = UnityEngine.Random.Range(0, numberOfPerceptrons);

            if (randomIndex == numberOfPerceptrons - 1) {
                for (int i = 0; i < numberOfPerceptrons; i++) {
                    Perceptron.CrossOver(layer1.perceptrons[i], layer2.perceptrons[i], Perceptron.SWAP);
                }
            }
            else{
                for (int i = 0; i <=randomIndex; i++){
                    Perceptron.CrossOver(layer1.perceptrons[i], layer2.perceptrons[i], Perceptron.ONE_WAY);
                }
                for (int i = randomIndex+1; i < numberOfPerceptrons; i++)
                {
                    Perceptron.CrossOver(layer2.perceptrons[i], layer1.perceptrons[i], Perceptron.ONE_WAY);
                }
            }

        }
        else if (type == LAYER_PARTIAL_PERCEPTRON_CROSSOVER) {

        }
    }

}
