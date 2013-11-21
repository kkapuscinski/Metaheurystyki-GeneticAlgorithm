using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
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
        public Genotype WorstGenotype { get; set; }
        public int BestGenotypeGeneration { get; set; }
        public int WorstGenotypeGeneration { get; set; }

        public List<Genotype> ThisGeneration;
        public List<Genotype> NextGeneration;

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

            random = new Random();

        }

        /// <summary>
        /// uruchomienie algorytmu
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
                if (BestGenotype == null || ThisGeneration.First().FunctionValue < BestGenotype.FunctionValue)
                {
                    BestGenotype = ThisGeneration.First();
                    BestGenotypeGeneration = i;
                }

                if (WorstGenotype == null || ThisGeneration.Last().FunctionValue > WorstGenotype.FunctionValue)
                {
                    WorstGenotype = ThisGeneration.Last();
                    WorstGenotypeGeneration = i;
                }
                Reproduction();
                GeneticOperations();
                RankPopulation(ref NextGeneration);
                Succession();
            }
        }

        
        /// <summary>
        /// Tworzonie pierwszej generacji
        /// </summary>
        private void CreateFirstGeneration()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                ThisGeneration.Add(new Genotype(GenotypeSize, this));
            }
        }
        /// <summary>
        /// Szeregoawnie populacji według wartosci funkcji (pierwszy element jest najelpszy)
        /// </summary>
        /// <param name="generation">lista genotypów do szeregowania</param>
        private void RankPopulation(ref List<Genotype> generation)
        {
            foreach (var individual in generation)
            {
                individual.FunctionValue = EvaluateFunction(individual.GetValues());
            }
            for (int i = 0; i < PopulationSize; i++)
            {
                var genotype = ThisGeneration[i];
                genotype.FunctionValue = EvaluateFunction(genotype.GetValues());
            }
            generation.Sort(new GenoTypeComparer());

        }

        /// <summary>
        /// Reprodukcja genotypów
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
        }

        /// <summary>
        /// Selekcja turniejowa
        /// </summary>
        /// <returns></returns>
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
        /// Wykonanie operacji genetycznych (mutacja)
        /// </summary>
        private void GeneticOperations()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                NextGeneration[i].Mutate(MutationRate);
            }
        }

        /// <summary>
        /// sukcesja.  wybranie kol
        /// </summary>
        private void Succession()
        {
            var tmpGeneration = new List<Genotype>(PopulationSize);
            tmpGeneration.AddRange(ThisGeneration.Take(EliteSize));
            tmpGeneration.AddRange(NextGeneration.Take(PopulationSize - EliteSize));
            tmpGeneration.Sort(new GenoTypeComparer());
            ThisGeneration = tmpGeneration;
        }


    }
}
