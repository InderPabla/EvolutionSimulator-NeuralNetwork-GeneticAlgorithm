using UnityEngine;
using System.Collections;

public class Layer {

    private Layer previousLayer;

    private int numberOfPerceptrons;

    private Perceptron[] perceptrons;

    private bool isInputLayer = false;

    public int x = 23;

    public Layer(int numberOfPerceptrons)
    {
        this.numberOfPerceptrons = numberOfPerceptrons;
        this.isInputLayer = true;

        perceptrons = new Perceptron[numberOfPerceptrons];

        for (int i = 0; i < perceptrons.Length; i++){
            perceptrons[i] = new Perceptron();
        }
    }

    public Layer(int numberOfPerceptrons, Layer previousLayer) {
        this.numberOfPerceptrons = numberOfPerceptrons;
        this.previousLayer = previousLayer;
        this.isInputLayer = false;

        perceptrons = new Perceptron[numberOfPerceptrons];

        for (int i = 0; i < perceptrons.Length; i++){
            perceptrons[i] = new Perceptron();
        }
    }

}
