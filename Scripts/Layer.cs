using System;
using System.Collections.Generic;

[Serializable]
public class Layer
{
    public List<Neuron> Neurons { get; set; }
    public int NeuronCount
    {
        get
        {
            return Neurons.Count;
        }
    }
 
    public Layer(int numNeurons)
    {
        Neurons = new List<Neuron>(numNeurons);
    }
}