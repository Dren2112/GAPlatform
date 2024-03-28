using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GAPlatform;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace GAUnitTests
{
    [TestClass]
    public class Initialisation
    {
        [TestMethod]
        public void Random()
        {
            //ARRANGE
            //set up sample TSP problem file upload
            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"berlin52.tsp");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }

            //inititlise required variables
            GAInitialisation initialisation = new GAInitialisation();
            Random rnd = new Random();

            //ACT
            List<Tour> result = initialisation.RandomInitialisation(50);

            //ASSERT
            //the method should return 50 tours
            Assert.AreEqual(result.Count, 50);

            foreach (Tour t in result)
            {
                //every tour should contain only one of each number
                Assert.IsTrue(t.sites.Count() == t.sites.Distinct().Count());
            }

            //select two tours at random and ensure they aren't identical
            int first = rnd.Next(50);
            int second = rnd.Next(50);
            while (first == second) second = rnd.Next(50);

            for (int i = 0; i <10; i++)
            {
                Assert.IsFalse(result[first].sites.SequenceEqual(result[second].sites));
            }
        }

        [TestMethod]
        public void RandomZero()
        {
            //ARRANGE
            GAInitialisation initialisation = new GAInitialisation();

            //ACT
            List<Tour> result = initialisation.RandomInitialisation(0);

            //ASSERT
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Sorted()
        {
            //ARRANGE
            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"berlin52.tsp");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }

            //initialise other required variables
            Operators.genSize = 50;
            GAApplication GAApplication = new GAApplication();
            GAInitialisation initialisation = new GAInitialisation();
            Random rnd = new Random();


            //ACT
            List<Tour> result = initialisation.SortedInitialisation();
            List<Tour> comparison = initialisation.RandomInitialisation(50);

            //ASSERT
            //method should return 50 tours
            Assert.AreEqual(result.Count, 50);

            //each tour should contain each number once
            foreach (Tour t in result)
            {
                Assert.IsTrue(t.sites.Count() == t.sites.Distinct().Count());
            }

            //select two tours at random and ensure they aren't identical
            int first = rnd.Next(50);
            int second = rnd.Next(50);
            while (first == second) second = rnd.Next(50);

            for (int i = 0; i <10; i++)
            {
                Assert.IsFalse(result[first].sites.SequenceEqual(result[second].sites));
            }

            //average of selected method tours should be greater than average of random tours
            double sum = 0;
            GAApplication.FitnessCheck(comparison);

            foreach (Tour T in result)
            {
                sum += T.fitness;
            }
            double sortedFit = sum / 50;
            sum = 0;

            foreach(Tour T in comparison)
            {
                sum += T.fitness;
            }

            double comparisonFit = sum / 50;

            Assert.IsTrue(comparisonFit < sortedFit);

        }

        [TestMethod]
        public void SortedNull()
        {
            //ARRANGE
            GAInitialisation initialisation = new GAInitialisation();

            //ACT
            List<Tour> result = initialisation.SortedInitialisation();

            //ASSERT
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NearestNeighbour()
        {
            //ARRANGE
            GAInitialisation initialisation = new GAInitialisation();
            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"test.txt");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }
            Operators.genSize = 50;

            //create sample tour
            List<int> NearestNeighbourZero = new List<int> {0, 6, 2, 1, 4, 7, 9, 5, 8, 3};


            //ACT
            List<Tour> result = initialisation.NNInitialisation();

            //ASSERT
            foreach (Tour tour in result)
            {
                if (tour.sites[0] == 0)
                {
                    Assert.IsTrue(tour.sites.SequenceEqual(NearestNeighbourZero));
                    break;
                }

                Assert.AreEqual(tour.sites.Count(), tour.sites.Distinct().Count());
            }
        }

        [TestMethod]
        public void NNNull()
        {
            //ARRANGE
            GAInitialisation initialisation = new GAInitialisation();

            //ACT
            List<Tour> result = initialisation.NNInitialisation();

            //ASSERT
            Assert.AreEqual(0, result.Count());
        }
    }

    [TestClass]
    public class HelperFunctions
    {
        [TestMethod]
        public void Leastfit()
        {
            //ARRANGE
            //create a list of tours with a random fitness, and save the smallest fitness
            List<Tour> generation = new List<Tour>();
            Random rnd = new Random();
            int minfit = 999999999;
            for(int i = 0; i < 10; i++)
            {
                int fit = rnd.Next();
                generation.Add(new Tour { fitness = fit});
                if (fit < minfit) minfit = fit;
            }
            GAApplication GAApplication = new GAApplication();

            //ACT
            Tour result = GAApplication.LeastFit(generation);

            //ASSERT
            Assert.AreEqual(result.fitness, minfit);
        }

        [TestMethod]
        public void LeastFitNull()
        {
            //ARRANGE
            GAApplication GAApplication = new GAApplication();

            //ACT
            Tour result = GAApplication.LeastFit(null);

            //ASSERT
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Distance()
        {
            //ARRANGE
            GAApplication GAApplication = new GAApplication();
            Site S = new Site {
                X = 3,
                Y = 4
            };
            Site S2 = new Site
            {
                X = 6,
                Y = 8
            };

            //ACT
            double result = GAApplication.Distance(S, S2);

            //ASSERT
            Assert.AreEqual(result, 5);
        }

        [TestMethod]
        public void DistanceNull()
        {
            //ARRANGE
            GAApplication GAApplication = new GAApplication();

            //ACT
            double result = GAApplication.Distance(null, null);

            //ASSERT
            Assert.AreEqual(double.MaxValue, result);
        }

        [TestMethod]
        public void FitnessCheck()
        {
            //ARRANGE
            GAApplication GAApplication = new GAApplication();
            List<Tour> list = new List<Tour> { new Tour { sites = {0,1,2,3,4,5,6,7,8,9 } } };

            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"test.txt");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }


            //ACT
            GAApplication.FitnessCheck(list);

            //ASSERT
            Assert.AreEqual(list[0].fitness, 0.023719202860804461);
        }

        [TestMethod]
        public void FitnessNull()
        {
            //ARRANGE
            GAApplication GAApplication = new GAApplication();

            //ACT
            GAApplication.FitnessCheck(null);

            //ASSERT
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    public class Selection
    {
        [TestMethod]
        public void Tournament()
        {
            //ARRANGE
            List<Tour> generation = new List<Tour>
            {
                new Tour { fitness = 1 },
                new Tour { fitness = 2 },
                new Tour { fitness = 3 },
                new Tour { fitness = 4 },
                new Tour { fitness = 5 }
            };
            GASelection selection = new GASelection();

            //ACT
            List<Tour> result = selection.TournamentSelection(generation);

            //ASSERT
            Assert.IsTrue(result[0].fitness >= 4);
            Assert.IsTrue(result[1].fitness >= 4);
        }

        [TestMethod]
        public void TournamentNull()
        {
            //ARRANGE
            GASelection selection = new GASelection();

            //ACT
            List<Tour> result = selection.TournamentSelection(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void Roulette() 
        {
            //ARRANGE
            GAInitialisation initialisation = new GAInitialisation();
            GASelection selection = new GASelection();
            GAApplication application = new GAApplication();

            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"test.txt");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }

            List<Tour> Generation = initialisation.RandomInitialisation(50);
            application.FitnessCheck(Generation);
            List<Tour> selected = new List<Tour>();

            //find the average fitness of the randomly generated generation
            double generationAverage;
            double sum = 0;

            foreach(Tour t in Generation)
            {
                sum += t.fitness;
            }
            generationAverage = sum / 50;

            //ACT
            //select 50 members of the generation - selected members can be copies of each other
            while (selected.Count < 50)
            {
                List<Tour> Parents = selection.RouletteSelection(Generation);
                selected.Add(Parents[0]);
                selected.Add(Parents[1]);
            }

            //ensure the average fitness of the selected tours is better than the average fitness of the randomised set of tours
            double selectedAverage;
            foreach (Tour t in selected)
            {
                sum += t.fitness;
            }
            selectedAverage = sum / 50;

            //ASSERT
            Assert.IsTrue(selectedAverage > generationAverage);
        }

        [TestMethod]
        public void RouletteNull()
        {
            //ARRANGE
            GASelection selection = new GASelection();

            //ACT
            List<Tour> result = selection.RouletteSelection(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void Ranked()
        {
            //ARRANGE
            GAInitialisation initialisation = new GAInitialisation();
            GASelection selection = new GASelection();
            GAApplication application = new GAApplication();

            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"test.txt");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }

            List<Tour> Generation = initialisation.RandomInitialisation(50);
            application.FitnessCheck(Generation);
            List<Tour> selected = new List<Tour>();

            //find the average fitness of the randomly generated generation
            double generationAverage;
            double sum = 0;

            foreach (Tour t in Generation)
            {
                sum += t.fitness;
            }
            generationAverage = sum / 50;

            //ACT
            //select 50 members of the generation - selected members can be copies of each other

            while (selected.Count < 50)
            {
                List<Tour> Parents = selection.RankedSelection(Generation);
                selected.Add(Parents[0]);
                selected.Add(Parents[1]);
            }

            //ensure the average fitness of the selected tours is better than the average fitness of the randomised set of tours
            double selectedAverage;
            foreach (Tour t in selected)
            {
                sum += t.fitness;
            }
            selectedAverage = sum / 50;

            //ASSERT
            Assert.IsTrue(selectedAverage > generationAverage);
        }

        [TestMethod]
        public void RankedNull()
        {
            //ARRANGE
            GASelection selection = new GASelection();

            //ACT
            List<Tour> result = selection.RankedSelection(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }
    }

    [TestClass]
    public class Crossover
    {
        [TestMethod]
        public void OrderBased()
        {
            //ARRANGE
            Tour Parent1 = new Tour 
            {
                sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Tour Parent2 = new Tour 
            {
                sites = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
            };

            List<Tour> Parents = new List<Tour> {Parent1, Parent2 };

            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.OrderBasedCrossover(Parents);

            //ASSERT
            //ensure only valid tours are returned
            Assert.AreEqual(10, result[0].sites.Distinct().Count());
            Assert.AreEqual(10, result[1].sites.Distinct().Count());

            //each digit should match the position in one parent
            int FirstMatch = 0;
            int SecondMatch = 0;
            int j = 0;

            for (int i =0; i < result[0].sites.Count; i++)
            {
                if (result[0].sites[i] == Parent1.sites[i]) { FirstMatch++; }
                else
                {
                    if (j <= Parents[1].sites.IndexOf(result[0].sites[i]))
                    {
                        j = Parents[1].sites.IndexOf(result[0].sites[i]);
                        SecondMatch++;
                    }
                }
            }

            //ensure all digits match
            Assert.AreEqual(10, FirstMatch + SecondMatch);
        }

        [TestMethod]
        public void OrderBasedNull()
        {
            //ARRANGE
            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.OrderBasedCrossover(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void Ordered()
        {
            //ARRANGE
            Tour Parent1 = new Tour
            {
                sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Tour Parent2 = new Tour
            {
                sites = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
            };

            List<Tour> Parents = new List<Tour> { Parent1, Parent2 };

            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.OrderedCrossover(Parents);

            //ASSERT
            //ensure only valid tours are returned
            Assert.AreEqual(10, result[0].sites.Distinct().Count());
            Assert.AreEqual(10, result[1].sites.Distinct().Count());

            //find the start of teh swapped region
            int index = 0;
            for (int i = 0; i < 10; i++)
            {
                if (result[0].sites[i] == Parent1.sites[i]) { index = i; break; }
            }

            //rearragnge temp so it begins at the start of the swapped region
            List<int> temp = new List<int>(Parents[1].sites);

            List<int> ending = temp.GetRange(0, index);

            foreach (int k in ending)
            {
                temp.Remove(k);
            }

            temp.AddRange(ending);

            //ensure each digit matches the position of the first parent or the order of the second
            int FirstMatch = 0;
            int SecondMatch = 0;
            int j = 0;

            for (int i = 0; i < result[0].sites.Count; i++)
            {
                if (result[0].sites[i] == Parent1.sites[i]) { FirstMatch++; }
                else
                {
                    if (j <= temp.IndexOf(result[0].sites[i]))
                    {
                        j = temp.IndexOf(result[0].sites[i]);
                        SecondMatch++;
                    }
                }
            }

            Assert.AreEqual(10, FirstMatch + SecondMatch);

        }

        [TestMethod]
        public void OrderedNull()
        {
            //ARRANGE
            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.OrderedCrossover(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GeneRepair()
        {
            //ARRANGE
            Tour Parent1 = new Tour
            {
                sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Tour Parent2 = new Tour
            {
                sites = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
            };

            List<Tour> Parents = new List<Tour> { Parent1, Parent2 };

            GACrossover Crossover = new GACrossover();

            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(@"test.txt");
            string[] ProblemStrings = Problem.Split('\n');

            //builds each TSP destination as a site object based on the file
            for (int i = 0; i < ProblemStrings.Length; i++)
            {
                if (ProblemStrings[i] != "")
                {
                    if (char.IsDigit(ProblemStrings[i][0]))
                    {
                        string[] Coords = ProblemStrings[i].Split(' ');
                        Site site = new Site();
                        site.X = Double.Parse(Coords[1]);
                        site.Y = Double.Parse(Coords[2]);
                        ProblemData.Sitelist.Add(site);
                    }
                }
            }

            //ACT
            List<Tour> result = Crossover.GeneRepairCrossover(Parents);

            //ASSERT
            //ensure each returned tour is valid, and is not the same as the parent tour
            Assert.AreEqual(10, result[0].sites.Distinct().Count());
            Assert.AreEqual(10, result[1].sites.Distinct().Count());

            Assert.IsFalse(result[0].sites.SequenceEqual(Parent1.sites));
            Assert.IsFalse(result[1].sites.SequenceEqual(Parent2.sites));
        }

        [TestMethod]
        public void GeneRepairNull()
        {
            //ARRANGE
            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.GeneRepairCrossover(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void Cycle()
        {
            //ARRANGE
            Tour Parent1 = new Tour
            {
                sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Tour Parent2 = new Tour
            {
                sites = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
            };

            List<Tour> Parents = new List<Tour> { Parent1, Parent2 };

            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.CycleCrossover(Parents);

            //ASSERT
            Assert.IsTrue(result[0].sites.SequenceEqual(new List<int> {9,1,7,3,5,4,6,2,8,0}));
        }

        [TestMethod]
        public void CycleNull()
        {
            //ARRANGE
            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.CycleCrossover(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void PartiallyMapped()
        {
            //ARRANGE
            Tour Parent1 = new Tour
            {
                sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Tour Parent2 = new Tour
            {
                sites = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
            };

            List<Tour> Parents = new List<Tour> { Parent1, Parent2 };

            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.PMCrossover(Parents);

            //ASSERT
            //ensure valid tours are returned that aren't the same as the parent tours
            Assert.AreEqual(10, result[0].sites.Distinct().Count());
            Assert.AreEqual(10, result[1].sites.Distinct().Count());

            Assert.IsFalse(result[0].sites.SequenceEqual(Parent1.sites));
            Assert.IsFalse(result[1].sites.SequenceEqual(Parent2.sites));
        }

        [TestMethod]
        public void PartiallyMappedNull()
        {
            //ARRANGE
            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.PMCrossover(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }



        [TestMethod]
        public void PositionBased()
        {
            //ARRANGE
            Tour Parent1 = new Tour
            {
                sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Tour Parent2 = new Tour
            {
                sites = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
            };

            List<Tour> Parents = new List<Tour> { Parent1, Parent2 };

            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.PositionBasedCrossover(Parents);

            //ASSERT
            //ensure valid tours are returned that aren't the same as the parent tours
            Assert.AreEqual(10, result[0].sites.Distinct().Count());
            Assert.AreEqual(10, result[1].sites.Distinct().Count());

            Assert.IsTrue(!result[0].sites.SequenceEqual(Parent1.sites) || !result[1].sites.SequenceEqual(Parent2.sites));

        }

        [TestMethod]
        public void PositionBasedNull()
        {
            //ARRANGE
            GACrossover Crossover = new GACrossover();

            //ACT
            List<Tour> result = Crossover.PositionBasedCrossover(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }
    }

    [TestClass]
    public class Mutation
    {
        [TestMethod]
        public void ReverseSequence()
        {
            //ARRANGE
            Tour tour = new Tour { sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

            GAMutation mutation = new GAMutation();

            //ACT
            Tour result = mutation.ReverseSequenceMutation(tour);

            //ASSERT
            //each digit should either match the index or the next digit should match the next index, or be one less than the current digit
            bool pass = true;
            for(int i =0; i < 10; i++)
            {
                if (i != result.sites[i])
                {
                    if (!(i + 1 ==  result.sites[i + 1] || result.sites[i] - 1 == result.sites[i + 1]))
                    {
                        pass = false;
                    }
                }
            }

            Assert.IsTrue(pass);
            Assert.AreEqual(result.sites.Count(), result.sites.Distinct().Count());
        }

        [TestMethod]
        public void ReverseNull()
        {
            //ARRANGE
            GAMutation mutation = new GAMutation(); ;

            //ACT
            Tour result = mutation.ReverseSequenceMutation(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void Swap()
        {
            //ARRANGE
            Tour tour = new Tour { sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

            GAMutation mutation = new GAMutation();

            //ACT
            Tour result = mutation.SwapMutation(tour);

            //ASSERT
            //find the two out pf place tours and ensure they have been correctly swapped
            int index = -1;
            for(int i = 0; i < 10; i++)
            {
                if (result.sites[i] != i)
                {
                    if (index == -1)
                    {
                        index = i;
                    }
                    else
                    {
                        if (result.sites[i] != index) { Assert.Fail(); }
                    }
                }
            }
            Assert.AreEqual(result.sites.Count(), result.sites.Distinct().Count());
        }

        [TestMethod]
        public void SwapNull()
        {
            //ARRANGE
            GAMutation mutation = new GAMutation(); ;

            //ACT
            Tour result = mutation.SwapMutation(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void PartialShuffle()
        {
            //ARRANGE
            Tour tour = new Tour { sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

            GAMutation mutation = new GAMutation();

            //ACT
            Tour result = mutation.SwapMutation(tour);

            //ASSERT
            //ensure the tour is different and still valid
            Assert.IsFalse(result.sites.SequenceEqual(new List<int> { 0,1,2,3,4,5,6,7,8,9}));
            Assert.AreEqual(result.sites.Count(), result.sites.Distinct().Count());
        }

        [TestMethod]
        public void PartialShuffleNull()
        {
            //ARRANGE
            GAMutation mutation = new GAMutation(); ;

            //ACT
            Tour result = mutation.PartialShuffleMutation(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CentreInverse()
        {
            //ARRANGE
            Tour tour = new Tour { sites = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

            GAMutation mutation = new GAMutation();

            //ACT
            Tour result = mutation.CentreInverseMutation(tour);

            //ASSERT
            int index = result.sites[0];

            //each tour should be one less than the preceding tour, unless it is at the start of the region split
            for (int i = 0; i < 9; i++)
            {
                if(i == index) continue;

                if(result.sites[i] - 1 != result.sites[i + 1])
                {
                    Assert.Fail();
                }
            }
            Assert.AreEqual(result.sites.Count(), result.sites.Distinct().Count());
        }

        [TestMethod]
        public void CentreInverseNull()
        {
            //ARRANGE
            GAMutation mutation = new GAMutation(); ;

            //ACT
            Tour result = mutation.CentreInverseMutation(null);

            //ASSERT
            Assert.AreEqual(null, result);
        }
    }
}
