using System;
using System.Collections.Generic;
using System.Security.Cryptography;

[Serializable]
public class CryptoRandom
{
    public double RandomValue { get; set; }
 
    public CryptoRandom()
    {
		RNGCryptoServiceProvider p = new RNGCryptoServiceProvider();
		Random r = new Random(p.GetHashCode());
        this.RandomValue = r.NextDouble();
    }
}
