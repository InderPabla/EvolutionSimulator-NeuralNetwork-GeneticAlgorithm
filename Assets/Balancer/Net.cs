using UnityEngine;
using System.Collections;

public class Net  {

    private int numberOfInputPerceptrons;
    private int numberOfOutputPerceptrons;
    private int numberOfHiddenLayers;
    private int numberOfHiddenPerceptrons;

    public Net(int numberOfInputPerceptrons, int numberOfOutputPerceptrons, int numberOfHiddenLayers, int numberOfHiddenPerceptrons) {
        this.numberOfInputPerceptrons = numberOfInputPerceptrons;
        this.numberOfOutputPerceptrons = numberOfOutputPerceptrons;
        this.numberOfHiddenLayers = numberOfHiddenLayers;
        this.numberOfHiddenPerceptrons = numberOfHiddenPerceptrons;
    }
}
