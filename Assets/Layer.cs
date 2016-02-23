using UnityEngine;
using System.Collections;

public class Layer
{
    public Neuron[] neurons;
     
    public Layer(int size)
    {
        neurons = new Neuron[size];
        neurons[size - 1].data = 1f;
    }

    public void fireNeurons(Neuron[] inputNeurons)
    {
        for(int i = 0; i < neurons.Length; i++)
        {
            neurons[i].fire(inputNeurons[i].weights);
        }
    }
	
}
