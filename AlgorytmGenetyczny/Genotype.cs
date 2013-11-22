using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    /// <summary>
    /// klasa opisująca genotyp
    /// </summary>
    public class Genotype
    {

        public BitArray Genes { get; set; }
        public int Length { get; set; }
        public Double FunctionValue {get; set;}
        public GeneticAlgorithm geneticAlgorithm {get; set;}

        /// <summary>
        /// konstruktor tworzy genotyp i ewentualnie losuje wartości genów ( przy krzyżowaniu wstrzykiwane sa geny)
        /// </summary>
        /// <param name="length">długość genotypu( ilość wymiarów funkcji)</param>
        /// <param name="algorithm">instancja algorytmu dla którego wykownywana jest optymalizacja</param>
        /// <param name="generateRandomGenes">czy losować geny</param>
        public Genotype(int length, GeneticAlgorithm algorithm, bool generateRandomGenes = true)
        {
            Length = length;
            geneticAlgorithm = algorithm;
            if (generateRandomGenes)
            {
                Genes = GenerateGenes();
            }
            else
            {
                Genes = new BitArray(Length * 32);
            }
        }

        /// <summary>
        /// Metoda Pobierająca wartości punktów funkcji
        /// </summary>
        /// <returns>Tablica wartości wymiarów funkcji</returns>
        public double[] GetValues()
        {
            var doubleNumbers = new double[Length];
            var bytearray = new byte[4 * Length];
            Genes.CopyTo(bytearray, 0);
            for (int i = 0; i < Length; i++)
            {
                var singleByteValue = bytearray.Skip(i * Length).Take(4).ToArray(); // konwersja na bajty
                var rawValue = BitConverter.ToUInt32(singleByteValue, 0); // konwersja na Uint
                doubleNumbers[i] = ((rawValue * 0.4656612874161595) / 10000000) - 100; //skalowanie do dziedziny

            }
            return doubleNumbers;
        }

        /// <summary>
        /// Metoda Generująca geny
        /// </summary>
        /// <returns>Tablica genów</returns>
        private BitArray GenerateGenes()
        {
            var byteList = new List<byte>();
            for (int i = 0; i < Length; i++)
            {
                var tmpByteArray = new byte[4];
                geneticAlgorithm.random.NextBytes(tmpByteArray);

                byteList.AddRange(tmpByteArray);
            }
            return new BitArray(byteList.ToArray());

        }

        /// <summary>
        /// Metoda Wykonuje kopię Genotypu
        /// </summary>
        /// <returns></returns>
        public Genotype Copy()
        {
            var copiedGenotype = new Genotype(this.Length, this.geneticAlgorithm, false);
            copiedGenotype.Genes = this.Genes;
            return copiedGenotype;
        }

        /// <summary>
        /// Krzyżowanie genotypu
        /// </summary>
        /// <param name="parent2">genotyp z którym ma nastąpić krzyżowanie</param>
        /// <param name="crossoverRate">współczynnik krzyżowania</param>
        /// <returns>tablica dzieci </returns>
        public Genotype[] Crossover(Genotype parent2, float crossoverRate)
        {
            if (this.Length != parent2.Length) throw new Exception("Can't Crossover genotypes with diffrent length");

            var copyArray = new BitArray(Length * 32);
            for (int i = 0; i < copyArray.Length; i++)
            {
                copyArray[i] = geneticAlgorithm.random.NextDouble() < crossoverRate ? true : false;
            }

            var child1 = new Genotype(this.Length, this.geneticAlgorithm, false);
            var child2 = new Genotype(this.Length, this.geneticAlgorithm, false);
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

        /// <summary>
        /// Mutacja genotypu
        /// </summary>
        /// <param name="mutationRate">współczynnik mutacji genotypu</param>
        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Length * 32; i++)
            {
                if (geneticAlgorithm.random.NextDouble() < mutationRate)
                    Genes[i] = Genes[i] ? false : true;
            }
        }
    }
}
