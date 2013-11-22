using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    /// <summary>
    /// Klasa reprezentująca algorytm genetyczny
    /// </summary>
    public class GeneticAlgorithm
    {
        public Random random;
        
        public FunctionToOptimize EvaluateFunction;

        public int PopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }
        public float MutationRate { get; set; }
        public float ReproductionRate { get; set; }
        public float CrossoverRate { get; set; }
        public int GenotypeSize { get; set; }
        public int TournamentSize { get; set; }
        public int EliteSize { get; set; }
        public Genotype BestGenotype { get; set; }
        public int BestGenotypeGeneration { get; set; }

        public List<Genotype> ThisGeneration;
        public List<Genotype> NextGeneration;

        /// <summary>
        /// Konstruktor inicjalizujący parametry algorytmu
        /// </summary>
        /// <param name="populationSize">Rozmiar populacji</param>
        /// <param name="numberOfGenerations">Ilość Generacji</param>
        /// <param name="mutationRate">współczynnik mutacji</param>
        /// <param name="reproductionRate">Współczynnik reprodukcji</param>
        /// <param name="crossoverRate">Współczynnik krzyżowania</param>
        /// <param name="genotypeSize">Ilość wymiarów badanej funkcji</param>
        /// <param name="tournamentSize">Wielkość turnieju</param>
        /// <param name="eliteSize">Wielkość elity</param>
        /// <param name="function">Delegat wskazujący na badaną funkcję</param>
        public GeneticAlgorithm(int populationSize, int numberOfGenerations, float mutationRate, float reproductionRate, float crossoverRate, int genotypeSize, int tournamentSize, int eliteSize, FunctionToOptimize function)
        {
            if (function == null) throw new ArgumentNullException("Function can't be null");
            if (populationSize % 2 != 0) throw new ArgumentException("population size must be even");
            if (eliteSize > populationSize / 2) throw new ArgumentOutOfRangeException("eliteSize is too big. must be less than populationSize/2");
            if (mutationRate >= 1.0 || mutationRate <= 0) throw new ArgumentOutOfRangeException("mutationRate must be between 0 and 1");
            if (reproductionRate >= 1.0 || reproductionRate <= 0) throw new ArgumentOutOfRangeException("reproductionRate must be between 0 and 1");
            if (crossoverRate >= 1.0 || crossoverRate <= 0) throw new ArgumentOutOfRangeException("crossoverRate must be between 0 and 1");

            PopulationSize = populationSize;
            NumberOfGenerations = numberOfGenerations;
            MutationRate = mutationRate;
            ReproductionRate = reproductionRate;
            CrossoverRate = crossoverRate;
            GenotypeSize = genotypeSize;
            TournamentSize = tournamentSize;
            EliteSize = eliteSize;
            EvaluateFunction = function;

            //tworzenie nowego generatora pseudolosowego z nowym ziarnem(konstruktor domyślny bierze pod uwagę aktualną datę
            random = new Random();

        }

        /// <summary>
        /// metoda uruchomiająca algorytm
        /// </summary>
        public void Run()
        {
            ThisGeneration = new List<Genotype>(PopulationSize);
            NextGeneration = new List<Genotype>(PopulationSize);
            BestGenotype = null;

            CreateFirstGeneration();
            RankPopulation(ref ThisGeneration);
            for (int i = 0; i < NumberOfGenerations; i++)
            {
                // zapis najlepszego osobnika
                if (BestGenotype == null || ThisGeneration.First().FunctionValue < BestGenotype.FunctionValue)
                {
                    BestGenotype = ThisGeneration.First();
                    BestGenotypeGeneration = i;
                }

                Reproduction();
                RankPopulation(ref NextGeneration);
                Succession();
            }
        }

        
        /// <summary>
        /// Metoda tworząca pierwszą generację losowo
        /// </summary>
        private void CreateFirstGeneration()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                ThisGeneration.Add(new Genotype(GenotypeSize, this));
            }
        }
        /// <summary>
        /// Metoda Szeregująca populację według wartosci funkcji (pierwszy element jest najelpszy)
        /// </summary>
        /// <param name="generation">lista genotypów do szeregowania</param>
        private void RankPopulation(ref List<Genotype> generation)
        {
            // ewaluacja funkcji
            foreach (var individual in generation)
            {
                individual.FunctionValue = EvaluateFunction(individual.GetValues());
            }
            // sortowanie listy
            generation.Sort(new GenoTypeComparer());

        }

        /// <summary>
        /// Metoda wykonująca reprodukcję osobników
        /// </summary>
        private void Reproduction()
        {
            NextGeneration.Clear();

            while (NextGeneration.Count < PopulationSize)
            {
                var parent1 = TournamentSelection();
                var parent2 = TournamentSelection();
                if (random.NextDouble() < ReproductionRate)
                {
                    var childrens = parent1.Crossover(parent2, CrossoverRate);
                    NextGeneration.Add(childrens[0]);
                    NextGeneration.Add(childrens[1]);
                }
                else
                {
                    NextGeneration.Add(parent1);
                    NextGeneration.Add(parent2);
                }
            }

            for (int i = 0; i < PopulationSize; i++)
            {
                NextGeneration[i].Mutate(MutationRate);
            }
        }

        /// <summary>
        /// Metoda wykonująca selekcję turniejową
        /// </summary>
        /// <returns>Zwraca osobnika zwycięskiego</returns>
        private Genotype TournamentSelection()
        {
            var tmpGenotypes = new List<Genotype>();
            for (int i = 0; i < TournamentSize; i++)
            {
                tmpGenotypes.Add(ThisGeneration[random.Next(PopulationSize)]);
            }
            tmpGenotypes = tmpGenotypes.OrderByDescending(t => t.FunctionValue).ToList();

            return tmpGenotypes.First();
        }

        /// <summary>
        /// Metoda wykonująca sukcesję
        /// </summary>
        private void Succession()
        {
            var tmpGeneration = new List<Genotype>(PopulationSize);
            // pobieram elitę z aktualnej generacji
            tmpGeneration.AddRange(ThisGeneration.Take(EliteSize));
            // pobieram pozostałe osobniniki
            tmpGeneration.AddRange(NextGeneration.Take(PopulationSize - EliteSize));
            tmpGeneration.Sort(new GenoTypeComparer());
            // przepisuję osobników do następnej pętli
            ThisGeneration = tmpGeneration;
        }


    }
}
