using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
		private NeuralNetwork nn;
		private double[] input, output;
        private bool nnSwitch, trainSwitch, collectData;
        int trainsetCount = 0;
        int testcase = 0;
        List<double[][]> trainset = new List<double[][]>();

        public void chSwitch(int num){
            switch(num){
                case 0:
                    nnSwitch=!nnSwitch;
                    break;
                case 1:
                    trainSwitch=!trainSwitch;
                    break;
                case 2:
                    collectData=!collectData;
                    break;
                case 3:
                    NeuralNetwork.Save("save", nn);
                    Debug.Log("Saved");
                    break;
                case 4:
                    nn = NeuralNetwork.Read("save");
                    Debug.Log("Loaded");
                    break;

                    
            }
        }

        private void Update()
        {
            // Switch from AI control or manuel control
            if (Input.GetKeyDown(KeyCode.Space))
                nnSwitch = !nnSwitch;

            // AI Training
            if (Input.GetKeyDown(KeyCode.T))
                trainSwitch = !trainSwitch;

            // Record traindata
            if (Input.GetKeyDown(KeyCode.R))
                collectData = !collectData;

            // Save AI
            if (Input.GetKeyDown(KeyCode.O))
            {
                NeuralNetwork.Save("save", nn);
                Debug.Log("Saved");
            }

            // Load AI
            if (Input.GetKeyDown(KeyCode.L))
            {
                nn = NeuralNetwork.Read("save");
                Debug.Log("Loaded");
            }

            // Delete AI
            if (Input.GetKeyDown(KeyCode.P))
            {
                File.Delete("save");
            }

            // Reset AI
            if (Input.GetKeyDown(KeyCode.C))
            {
                nn = new NeuralNetwork(0.5, new int[] { m_Car.Sensor().Length, 5, 2 });
                Debug.Log("ResetAI");
            }

            // Save train data
            if (Input.GetKeyDown(KeyCode.I))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create("trainData");
                bf.Serialize(file, trainset);
                file.Close();
                Debug.Log("TrainData Saved");
            }

            // Load train data
            if (Input.GetKeyDown(KeyCode.K))
            {
                trainset = null;
                if (File.Exists("trainData"))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open("trainData", FileMode.Open);
                    trainset = (List<double[][]>)bf.Deserialize(file);
                    file.Close();
                }
                trainsetCount = trainset.Count;
                Debug.Log("TrainData Loaded");
            }

            // Delete train data
            if (Input.GetKeyDown(KeyCode.U))
            {
                File.Delete("trainData");
            }

            // Reset training data
            if (Input.GetKeyDown(KeyCode.V))
            {
                trainset = new List<double[][]>();
                Debug.Log("ResetData");
            }
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            nn = new NeuralNetwork(0.5, new int[] { m_Car.Sensor().Length, 5, 2 });
            nnSwitch = false;
            trainSwitch = false;

        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            double[][] info = null;
            double[][] traindata = null;

            input = Array.ConvertAll(m_Car.Sensor(), x => (double)x/300);

            if (collectData)
            {
                trainsetCount += 1;
                traindata = new double[2][];
                traindata[0] = input;
                traindata[1] = new double[] { (h+1)/2, (v+1)/2 };
                trainset.Add(traindata);
                Debug.Log("Number of data: " + trainsetCount);
            }

            if (nnSwitch)
            {
                if (!trainSwitch)
                {
                    output = nn.Run(input);
                    h = (float)((output[0]*2)-1);
                    v = (float)((output[1]*2)-1);
                    Debug.Log("value of h: " + h);
                    Debug.Log("value of v: " + v);
                }
            }

            if (trainSwitch)
            {
                testcase = (int)((new CryptoRandom()).RandomValue * trainsetCount);
                info = nn.Train(trainset[testcase][0], trainset[testcase][1]);
                Debug.Log("nn error: " + info[0][0]);
            }

            m_Car.Move(h, v, v, 0f);
        }


    }
}
