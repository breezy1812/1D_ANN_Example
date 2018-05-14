using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using MatsurikaG;

namespace ANN_Example
{
    class Program
    {
        static ArtificialNeuralNetwork Ann;
        static void Main(string[] args)
        {
            double[][] Data;
            int DataSize = 0;
            int input = 0;
            int output = 0;
            
            using (StreamReader SR = new StreamReader("trainingData.csv")) 
            {
                bool flag = true;
                string line = SR.ReadLine();
                List<double[]> _data = new List<double[]>();
                while(line != null)
                {
                    string[] pline = line.Split(',');
                    if (flag)
                    {
                        foreach(string i in pline)
                        {
                            if (i.Contains("x_"))
                                input++;
                            else
                                output++;
                        }
                        flag = false;
                    }
                    else
                    {
                        double[] temp = new double[input + output];
                        //輸入在前、輸出在後，以array堆疊數據總長度
                        for (int i  = 0; i < temp.Length; i++)
                            temp[i] = Convert.ToDouble(pline[i]);

                        _data.Add(temp);

                        DataSize++;
                    }
                    line = SR.ReadLine();
                }
                Data = _data.ToArray();
                Console.WriteLine("Welcome to ANN .NET library. \nChoose a function next. " +
                    "\n1 = Tran \n2 = Improve");

                string index = Console.ReadLine();
                switch(Convert.ToInt32(index))
                {
                    case 1:
                        {
                            Train(Data, input, output);
                            break;
                        }
                    case 2:
                        {
                            Improve(Data, input, output);
                            break;
                        }
                }
               
            }
            Console.Read();

        }

        static private void Train (double[][] Data, int input, int output)
        {
            //set parameters in layers
            int numHidden = 10;

            //create layers and construct them
            List<NNlayers> Nlist = new List<NNlayers>();
            NNlayers N1 = new NNlayers(NNlayers.Layers_family.Affine, input, numHidden);
            NNlayers N2 = new NNlayers(NNlayers.Layers_family.BN, numHidden, numHidden);
            NNlayers N3 = new NNlayers(NNlayers.Layers_family.Tanh, numHidden, numHidden);
            NNlayers N4 = new NNlayers(NNlayers.Layers_family.Affine, numHidden, output);
            Nlist.Add(N1);
            Nlist.Add(N2);
            Nlist.Add(N3);
            Nlist.Add(N4);

            //create a NN class
            Ann = new ArtificialNeuralNetwork(Nlist.ToArray(), input, output);
            Ann.PositiveLimit = 0.5;//default = 0.7
            int maxEpochs = 100;
            double learnRate = 0.05;

            //create a error monitor backgroundworker
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(backgroundWorker_NN_DoWork);
            BGW.RunWorkerAsync(maxEpochs);

            //train
            Ann.TrainModel(Data, maxEpochs, learnRate, 0);
            


            double trainAcc = Ann.Accu_train;
            Console.Write("\nFinal accuracy on train data = " +
            trainAcc.ToString("F4"));

            double testAcc = Ann.Accu_test;
            Console.Write("\nFinal accuracy on test data = " +
            testAcc.ToString("F4"));
            Console.Write("\nTrain finish");

            string site =  System.DateTime.Now.ToString("yyMMddHHmm") + "_learnproj";
            Directory.CreateDirectory(site);
            Ann.Save_network(site, learnRate);
            Ann.Save_H5files(site);

            Console.Write("\nVariables have been save in " + site);
            
        }

        static private void Improve(double[][] Data, int input, int output)
        {
            
            //create a NN class
            Ann = new ArtificialNeuralNetwork( input, output);
            Ann.PositiveLimit = 0.5;//default = 0.7
            int maxEpochs = 100;
            double learnRate = 0.05;

            //import old learning project
            
            DirectoryInfo di = new DirectoryInfo(System.Environment.CurrentDirectory);
            
            int date = 0;
            string path = "";
            List<string> projects = new List<string>();

            Console.WriteLine("\nChoose a learning project:");
            int N = 0;
            foreach(var fi in di.GetDirectories())
            {
                if(fi.Name.Contains("_learnproj"))
                {
                    projects.Add(fi.Name);
                    Console.WriteLine(N + " = " + fi.Name);
                    N++;
                }
            }
            if (N == 0)
            {
                Console.Write("\nCannot Find any learnproject");
                return;
            }
            string n = Console.ReadLine();
            

            bool error = Ann.ImportOldProject(projects[Convert.ToInt32(n)]);
            if (!error)
            {
                Console.Write("\nCannot import learnproject");
                return;
            }
            //create a error monitor backgroundworker
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(backgroundWorker_NN_DoWork);
            BGW.RunWorkerAsync(maxEpochs);

            //train
            Ann.ImproveModel(Data, maxEpochs, learnRate, 0);

            double trainAcc = Ann.Accu_train;
            Console.Write("\nFinal accuracy on train data = " +
            trainAcc.ToString("F4"));

            double testAcc = Ann.Accu_test;
            Console.Write("\nFinal accuracy on test data = " +
            testAcc.ToString("F4"));
            Console.Write("\nTrain finish");

            string site = System.DateTime.Now.ToString("yyMMddHHmm") + "_learnproj";
            Directory.CreateDirectory(site);
            Ann.Save_network(site, learnRate);
            Ann.Save_H5files(site);

            Console.Write("\nVariables have been save in " + site);
            
        }

            static private void backgroundWorker_NN_DoWork(object sender, DoWorkEventArgs e)
        {
            int epoch = 0;

            int max = (int)e.Argument;
            while (epoch != max - 10)
            {
                if (Ann.trainerror!= null)
                {
                    double a = Ann.trainerror[0];
                    double b = Ann.trainerror[1];
                    if (epoch != a)
                    {
                        epoch = (int)a;
                        Console.Write("\neapoch " + epoch + " = " + b.ToString("F4"));
                    }
                }
            }
        }
    }
}
