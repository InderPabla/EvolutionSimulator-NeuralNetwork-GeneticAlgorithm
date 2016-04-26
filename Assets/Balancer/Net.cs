using UnityEngine;
using System.Collections;

public class Net  {

    private int numberOfInputPerceptrons;
    private int numberOfOutputPerceptrons;
    private int numberOfHiddenLayers;
    private int numberOfHiddenPerceptrons;

    private Layer[] layers;

    public Net(int numberOfInputPerceptrons, int numberOfOutputPerceptrons, int numberOfHiddenLayers, int numberOfHiddenPerceptrons) {
        this.numberOfInputPerceptrons = numberOfInputPerceptrons;
        this.numberOfOutputPerceptrons = numberOfOutputPerceptrons;
        this.numberOfHiddenLayers = numberOfHiddenLayers;
        this.numberOfHiddenPerceptrons = numberOfHiddenPerceptrons;

        layers = new Layer[2+numberOfHiddenLayers];

        layers[0] = new Layer(numberOfInputPerceptrons);
        for (int i = 1; i < layers.Length; i++) {

            if (i == layers.Length - 1){
                layers[layers.Length - 1] = new Layer(numberOfOutputPerceptrons, numberOfHiddenPerceptrons);
            }
            else if (i == 1){
                layers[i] = new Layer(numberOfHiddenPerceptrons, numberOfInputPerceptrons);
            }
            else {
                layers[i] = new Layer(numberOfHiddenPerceptrons, numberOfHiddenPerceptrons);
            }
            
        }

        
        
    }
}
