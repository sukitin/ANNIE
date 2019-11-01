using System;
using System.Collections.Generic;

[Serializable]
public class Dendrite
{
    public double Weight { get; set; }
 
    public Dendrite()
    {
        CryptoRandom n = new CryptoRandom();
        this.Weight = n.RandomValue;
    }
	
}