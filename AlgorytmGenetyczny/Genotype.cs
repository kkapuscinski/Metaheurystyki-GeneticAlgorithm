using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    public class Genotype
    {
        private static Random _random = new Random();

        public BitArray Genes { get; set; }
        public int Length { get; set; }
        public Double FunctionValue {get; set;}

        public Genotype(int length, bool generateRandomGenes = true)
        {
            Length = length;
            if (generateRandomGenes)
            {
                Genes = GenerateGenes();
            }
            else
            {
                Genes = new BitArray(Length * 32);
            }
        }


        public double[] GetValues()
        {
            var doubleNumbers = new double[Length];
            var numbers = new int[Length];
            Genes.CopyTo(numbers, 0);
            for (int i = 0; i < Length; i++)
            {
                var value = numbers[i] / 10000000.0;
                if (value > 100.0) value = 100.0;
                if (value < -100.0) value = -100.0;
                doubleNumbers[i] = value;
            }
            return doubleNumbers;
        }

        private BitArray GenerateGenes()
        {
            var byteList = new List<byte>();
            for (int i = 0; i < Length; i++)
            {
                var randInt = _random.Next(-1000000000, 1000000000);
                var intByte = BitConverter.GetBytes(randInt);
                foreach (var singleByte in intByte)
                {
                    byteList.Add(singleByte);
                }
            }
            return new BitArray(byteList.ToArray());

        }

        public Genotype Copy()
        {
            var copiedGenotype = new Genotype(this.Length, false);
            copiedGenotype.Genes = this.Genes;
            return copiedGenotype;
        }


        public Genotype[] Crossover(Genotype parent2, float crossoverRate)
        {
            if (this.Length != parent2.Length) throw new Exception("Can't Crossover genotypes with diffrent length");

            var copyArray = new BitArray(Length * 32);
            for (int i = 0; i < copyArray.Length; i++)
            {
                copyArray[i] = _random.NextDouble() < crossoverRate ? true : false;
            }

            var child1 = new Genotype(this.Length, false);
            var child2 = new Genotype(this.Length, false);
            for (int i = 0; i < copyArray.Length; i++)
            {
                if (copyArray[i])
                {
                    child1.Genes[i] = this.Genes[i];
                    child2.Genes[i] = parent2.Genes[i];
                }
                else
                {
                    child1.Genes[i] = parent2.Genes[i];
                    child2.Genes[i] = this.Genes[i];
                }
            }

            return new Genotype[2] { child1, child2 };

        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Length * 32; i++)
            {
                if (_random.NextDouble() < mutationRate)
                    Genes[i] = Genes[i] ? false : true;
            }
        }



    }
}
