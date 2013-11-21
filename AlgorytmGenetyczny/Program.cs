using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    public delegate double FunctionToOptimize(double[] values);

    class Program
    {

        public static double Function(double[] values)
        {
            

            List<double> powValues = new List<double>();
            foreach (var value in values)
            {
                powValues.Add(Math.Pow(value, 2));
            }

            double f1 = 0.5 + (((Math.Pow(Math.Sin(Math.Sqrt(powValues.Sum())), 2) - 0.5) / Math.Pow(1 + 0.001 * (powValues.Sum()), 2)));
            return f1;
        }

        static void Main(string[] args)
        {
            FileStream fs = new FileStream(DateTime.Now.ToString("HH_mm_ss") + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);
            
            
            int populationSize = 100;
            int numberOfGenerations = 1000;
            float mutationRate = 0.8F;
            float reproductionRate = 0.8F;
            float crossoverRate = 0.8F;
            int genotypeSize = 2;
            int tournamentSize = 10;
            int eliteSize = 20;


            var argsCount = args.Count();
            if (argsCount >= 1)
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("Parametry do podania nalezy podawać po kolei oddzielone spacjami");
                    Console.WriteLine("1. rozmiar populacji integer");
                    Console.WriteLine("2. ilość generacji integer");
                    Console.WriteLine("3. prawdopodobieństwo mutacji float np. 0,8");
                    Console.WriteLine("4. prawdopodobieństwo reprodukcji float");
                    Console.WriteLine("5. prawdopodobieństwo krzyżowania float");
                    Console.WriteLine("6. rozmiar genotypu (ilość wymiarów funkcji) integer");
                    Console.WriteLine("7. Wielkość turnieju integer");
                    Console.WriteLine("8. Wielkość elity integer");
                    return;
                }
                else if (argsCount != 8)
                {
                    Console.WriteLine("nie prawidłowa ilość parametrów. skorzystaj z opcji -help");
                    return;
                }
                else
                {
                    populationSize = Convert.ToInt32(args[0]);
                    numberOfGenerations = Convert.ToInt32(args[1]);
                    mutationRate = Convert.ToSingle(args[2]);
                    reproductionRate = Convert.ToSingle(args[3]);
                    crossoverRate = Convert.ToSingle(args[4]);
                    genotypeSize = Convert.ToInt32(args[5]);
                    tournamentSize = Convert.ToInt32(args[6]);
                    eliteSize = Convert.ToInt32(args[7]);
                }


            }

            var function = new FunctionToOptimize(Function);
            var bestGenotypes = new List<Genotype>();
            var bestGenotypesGeneration = new List<int>();
            var worstGenotypes = new List<Genotype>();
            var worstGenotypesGeneration = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                var algorithm = new GeneticAlgorithm(populationSize, numberOfGenerations, mutationRate, reproductionRate, crossoverRate, genotypeSize, tournamentSize, eliteSize, function);
                algorithm.Run();
                bestGenotypes.Add(algorithm.BestGenotype);
                bestGenotypesGeneration.Add(algorithm.BestGenotypeGeneration);
                worstGenotypes.Add(algorithm.WorstGenotype);
                worstGenotypesGeneration.Add(algorithm.WorstGenotypeGeneration);
                
            }

            

            for (int i = 0; i < 10; i++)
			{
                Console.WriteLine("Generacja najlepszego Genotypu; Wartość najlepszego Genotypu; Generacja najgorszego Genotypu; Wartość najgorszego Genotypu;");
                Console.WriteLine("{0};{1};{2};{3}", bestGenotypesGeneration[i], bestGenotypes[i].FunctionValue.ToString("0.000000000"), worstGenotypesGeneration[i], worstGenotypes[i].FunctionValue.ToString("0.000000000"));
			}
            sw.Flush();
            fs.Flush(true);
            fs.Close();

        }
    }
}
