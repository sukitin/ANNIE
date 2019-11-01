using System;
using System.Collections.Generic;

[Serializable]
public class Neuron
{
    public List<Dendrite> Dendrites { get; set; }
    public double Bias { get; set; }
    public double Delta { get; set; }
    public double Value { get; set; }
 
    public int DendriteCount
    {
        get
        {
            return Dendrites.Count;
        }
    }
 
    public void resetDelta()
    {
        Delta = 0;
    }
    public Neuron()
    {
        Random n = new Random(Environment.TickCount);
        this.Bias = n.NextDouble();
        //this.Bias = 1;

        this.Dendrites = new List<Dendrite>();
    }
}
