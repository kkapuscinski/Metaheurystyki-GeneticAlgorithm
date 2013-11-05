using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    public class GeneticAlgorithm
    {
        private Random _random = new Random();
        public delegate double FunctionToOptimize(double[] values);
        public FunctionToOptimize EvaluateFunction;

        public int PopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }
        public float MutationRate { get; set; }
        public int GenotypeSize { get; set; }
        public int TournamentSize { get; set; }
        public int EliteSize { get; set; }
        public Genotype BestGenotype { get; set; }

        public List<Genotype> ThisGeneration;
        public List<Genotype> NextGeneration;

        public GeneticAlgorithm(int populationSize, int numberOfGenerations, float mutationRate, int genotypeSize, int tournamentSize, int eliteSize, FunctionToOptimize function)
        {
            if (function == null) throw new ArgumentNullException("Function can't be null");
            if (populationSize % 2 != 0) throw new ArgumentException("population size must be even");
            if (eliteSize > populationSize / 2) throw new ArgumentOutOfRangeException("eliteSize is too big. must be less than populationSize/2");
            if (mutationRate >= 1.0 || mutationRate <= 0) throw new ArgumentOutOfRangeException("mutationRate mus be between 0 and 1");

            PopulationSize = populationSize;
            NumberOfGenerations = numberOfGenerations;
            MutationRate = mutationRate;
            GenotypeSize = genotypeSize;
            TournamentSize = tournamentSize;
            EliteSize = eliteSize;
            EvaluateFunction = function;

        }


        public void Run()
        {

            ThisGeneration = new List<Genotype>(PopulationSize);
            NextGeneration = new List<Genotype>(PopulationSize);
            BestGenotype = null;

            CreateFirstGeneration();
            RankPopulation(ref ThisGeneration);
            BestGenotype = ThisGeneration.First();

            for (int i = 0; i < NumberOfGenerations; i++)
            {
                if (ThisGeneration.First().FunctionValue < BestGenotype.FunctionValue)
                {
                    BestGenotype = ThisGeneration.First();
                }
                Reproduction();
                GeneticOperations();
                RankPopulation(ref NextGeneration);
                Succession();
            }
        }

        

        private void CreateFirstGeneration()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                ThisGeneration.Add(new Genotype(GenotypeSize));
            }
        }

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

        private void Reproduction()
        {
            NextGeneration.Clear();
            for (int i = 0; i < PopulationSize/2; i++)
            {
                var parent1 = TournamentSelection();
                var parent2 = TournamentSelection();

                var childrens = parent1.Crossover(parent2);

                NextGeneration.Add(childrens[0]);
                NextGeneration.Add(childrens[1]);
            }
        }

        private Genotype TournamentSelection()
        {
            var tmpGenotypes = new List<Genotype>();
            for (int i = 0; i < TournamentSize; i++)
            {
                tmpGenotypes.Add(ThisGeneration[_random.Next(PopulationSize)]);
            }
            tmpGenotypes = tmpGenotypes.OrderByDescending(t => t.FunctionValue).ToList();

            return tmpGenotypes.First();
        }

        private void GeneticOperations()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                NextGeneration[i].Mutate(MutationRate);
            }
        }

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
