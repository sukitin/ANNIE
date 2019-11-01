using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NeuralNetwork
{
    public List<Layer> Layers { get; set; }
    public double LearningRate { get; set; }
    public int LayerCount
    {
        get
        {
            return Layers.Count;
        }
    }
 
    public NeuralNetwork(double learningRate, int[] layers)
    {
        if (layers.Length < 2) return;
 
        this.LearningRate = learningRate;
        this.Layers = new List<Layer>();
 
        for(int l = 0; l < layers.Length; l++)
        {
            Layer layer = new Layer(layers[l]);
            this.Layers.Add(layer);
 
            for (int n = 0; n < layers[l]; n++)
                layer.Neurons.Add(new Neuron());

            layer.Neurons.ForEach((nn) =>
            {
                if (l == 0)
                    nn.Bias = 0;
                else
                    for (int d = 0; d < layers[l - 1]; d++)
                        nn.Dendrites.Add(new Dendrite());
            });
        }
    }
 
    private double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }
 
	public double[] Run(double[] input)
    {
        if (input.Length != this.Layers[0].NeuronCount) return null;
 
        for (int l = 0; l < Layers.Count; l++)
        {
            Layer layer = Layers[l];
 
            for (int n = 0; n < layer.Neurons.Count; n++)
             {
                Neuron neuron = layer.Neurons[n];
 
                if (l == 0)
                     neuron.Value = input[n];
                 else
                {
                    neuron.Value = 0;
                    for (int np = 0; np < this.Layers[l - 1].Neurons.Count; np++)
                        neuron.Value = neuron.Value + this.Layers[l - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;
 
                    neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                }
            }
        }
 
        Layer last = this.Layers[this.Layers.Count - 1];
        int numOutput = last.Neurons.Count ;
        double[] output = new double[numOutput];
        for (int i = 0; i < last.Neurons.Count; i++)
            output[i] = last.Neurons[i].Value;
 
        return output;
    }
 
	public double[][] Train(double[] input, double[] output)
    {
        // error = 1, ans = 2
        double[][] info = new double[2][];
        info[0] = new double[1];
        info[0][0] = 0;

        if ((input.Length != this.Layers[0].Neurons.Count) || (output.Length != this.Layers[this.Layers.Count - 1].Neurons.Count)) return info;

        info[1] = Run(input);
        //Run(input);

            //Reset all Detla
            for(int j = this.Layers.Count - 1; j > 0; j--)
            {
                for(int k = 0; k < this.Layers[j].Neurons.Count; k++)
                {
                    this.Layers[j].Neurons[k].resetDelta();
                }
            }
 
        for(int i = 0; i < this.Layers[this.Layers.Count - 1].Neurons.Count; i++)
        {
            Neuron neuron = this.Layers[this.Layers.Count - 1].Neurons[i];
            neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[i] - neuron.Value);
            for (int j=0; j < neuron.Dendrites.Count; j++)
            {
                neuron.Dendrites[j].Weight = neuron.Dendrites[j].Weight + (this.LearningRate * this.Layers[this.Layers.Count - 2].Neurons[j].Value * neuron.Delta);
            }
        }
        for(int i =this.Layers.Count - 2; i >0; i--)
        {
            for(int j = 0; j < this.Layers[i].Neurons.Count; j++)
            {
                Neuron n = this.Layers[i].Neurons[j];
                for(int k = 0; k < this.Layers[i+1].Neurons.Count; k++)
                {
                    n.Delta += n.Value * (1 - n.Value) * this.Layers[i + 1].Neurons[k].Dendrites[j].Weight * this.Layers[i + 1].Neurons[k].Delta;
                }
                n.Bias = n.Bias + (this.LearningRate * n.Delta);
                
                for (int k=0; k < n.Dendrites.Count; k++)
                {
                    n.Dendrites[k].Weight = n.Dendrites[k].Weight + (this.LearningRate * this.Layers[i - 1].Neurons[k].Value * n.Delta);
                }
            }
        }
 
        /*for(int i = this.Layers.Count - 1; i > 1; i--)
        {
            for(int j=0; j < this.Layers[i].Neurons.Count; j++)
            {
                Neuron n = this.Layers[i].Neurons[j];
                n.Bias = n.Bias + (this.LearningRate * n.Delta);
 
                for (int k = 0; k < n.Dendrites.Count; k++)
                    n.Dendrites[k].Weight = n.Dendrites[k].Weight + (this.LearningRate * this.Layers[i - 1].Neurons[k].Value * n.Delta);
            }
        }*/

            // error
            for(int j=0; j < this.Layers[this.Layers.Count - 1].Neurons.Count; j++)
            {
                Neuron n = this.Layers[this.Layers.Count - 1].Neurons[j];
                info[0][0] = info[0][0] + n.Delta*n.Delta;
            }
        
 
        return info;
    }

    public static void Save(string filePath, NeuralNetwork objectToWrite)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, objectToWrite);
        file.Close();
    }

    public static NeuralNetwork Read(string filePath)
    {
        NeuralNetwork data = null;
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            data = (NeuralNetwork)bf.Deserialize(file);
            file.Close();
        }
        return data;
    }
}
