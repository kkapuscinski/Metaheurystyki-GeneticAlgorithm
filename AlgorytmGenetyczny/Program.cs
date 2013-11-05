using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    class Program
    {

        public static double theActualFunction(double[] values)
        {
            if (values.GetLength(0) != 2)
                throw new ArgumentOutOfRangeException("should only have 2 args");

            double x = values[0];
            double y = values[1];

            double f1 = 0.5+(( (Math.Pow(Math.Sin(Math.Sqrt(Math.Pow(x,2) + Math.Pow(y,2))),2) - 0.5)/Math.Pow(1 + 0.001*(Math.Pow(x,2) + Math.Pow(y,2)),2)));
            return f1;
        }

        static void Main(string[] args)
        {

            var a = theActualFunction(new double[] { 0, 0 });
            var function = new AlgorytmGenetyczny.GeneticAlgorithm.FunctionToOptimize(theActualFunction);
            
            var algorithm = new GeneticAlgorithm(100, 1000, 0.8F, 2, 10, 50, function);

            algorithm.Run();
            
            Console.ReadLine();

        }
    }
}
