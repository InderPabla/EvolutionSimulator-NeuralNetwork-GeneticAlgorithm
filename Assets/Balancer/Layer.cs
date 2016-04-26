using UnityEngine;
using System.Collections;

public class Layer {
    private int numberOfPerceptrons;
    private int previousLayerNumberOfPerceptrons;

    private Perceptron[] perceptrons;

    public Layer(int numberOfPerceptrons, int previousLayerNumberOfPerceptrons){
        this.numberOfPerceptrons = numberOfPerceptrons;
        this.previousLayerNumberOfPerceptrons = previousLayerNumberOfPerceptrons;

        perceptrons = new Perceptron[numberOfPerceptrons];
    }

    public Layer(int numberOfPerceptrons) {
        this.numberOfPerceptrons = numberOfPerceptrons;
        this.previousLayerNumberOfPerceptrons = -1;

        perceptrons = new Perceptron[numberOfPerceptrons];
    }

}
