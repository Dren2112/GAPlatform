using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Text;

namespace GAPlatform
{

    internal static class Rand
    {
        [ThreadStatic]
        private static Random _local;
        private static readonly Random Global = new Random();

        private static Random Instance
        {
            get
            {
                if(_local == null)
                {
                    int seed;
                    lock (Global)
                    {
                        seed = Global.Next();
                    }

                    _local = new Random(seed);
                }
                return _local;
            }
        }

        public static int Next(int min, int max) => Instance.Next(min, max);
        public static double NextDouble() => Instance.NextDouble();
    }

    public class GAApplication
    {

        /// <summary>
        /// main method for the genetic algorithm
        /// </summary>
        public double GeneticAlgorithm(GAForm form)
        {
            GAInitialisation GAInitialisation = new GAInitialisation();
            GASelection GASelection = new GASelection();
            GACrossover GACrossover = new GACrossover();
            GAMutation GAMutation = new GAMutation();
            ProblemData problemData = new ProblemData();

            //initialise the first generation of solutions
            List<Tour> generation = GAInitialisation.Initialisation();

            //repeat for each generation wanted in the run
            for (int i = 0; i < Operators.runs; i++)
            {

                //initilise the list of new solutions
                List<Tour> nextGen = new List<Tour>();

                //assign a fitness value to each tour
                FitnessCheck(generation, problemData);

                //output the best generation and best overall every 10 runs
                //if (i%10  == 0) { form.OutputBests(i); }

                //until the next generation has the desired number of new tours
                while (nextGen.Count != Operators.crossover)
                {
                    //select parents for crossover
                    List<Tour> selected = GASelection.Selection(generation);

                    //perform the crossover operation on the parents
                    List<Tour> children = GACrossover.Crossover(selected);

                    //add the children to the list
                    for (int j = 0; j < children.Count; j++)
                    {
                        //mutate the child according to the mutation rate
                        if (Rand.NextDouble() < Operators.mutationRate)
                        {
                            children[j] = GAMutation.Mutation(children[j]);
                        }


                        nextGen.Add(children[j]);
                    }

                    //if too many children have been added, e.g. if the dsired number of tours in a generation is odd, then remove tours until the desirred number is reached
                    while (nextGen.Count > Operators.crossover)
                    {
                        nextGen.RemoveAt(0);
                    }

                }

                //if GA is generational then replace the old generation with the new one
                if (Operators.crossover == Operators.genSize)
                    generation = new List<Tour>(nextGen);
                else
                {
                    //replace the least fit member of the old generation with the new one
                    for (int j = 0; j < nextGen.Count; j++)
                    {
                        if (generation.Count == 0) break;
                        generation.RemoveAt(generation.IndexOf(LeastFit(generation)));
                    }

                    //create a copy of each new tour and insert it into the genration array
                    foreach (Tour t in nextGen)
                    {
                        Tour copy = new Tour
                        {
                            sites = new List<int>(t.sites)
                        };

                        generation.Add(copy);
                    }
                }
            }

            return problemData.bestFit.fitness;

        }



        /// <summary>
        /// calculate the distance between two sites
        /// </summary>
        /// <param name="first">the first site</param>
        /// <param name="second">the second site</param>
        /// <returns>the distance between the two sites</returns>
        public double Distance(Site first, Site second)
        {
            //if given null return max value so tour with null should not be selected and therefore not pass on to another generation
            if (first == null || second == null) return double.MaxValue;

            //find the lengths of the horizontal axes between the sites
            double Xdistance = Math.Abs(first.X - second.X);
            double ydistance = Math.Abs(first.Y - second.Y);

            //use pythagoras to find the distance between the sites
            double distance = Math.Sqrt(Math.Pow(Xdistance, 2) + Math.Pow(ydistance, 2));

            //return the distance
            return distance;
        }

        /// <summary>
        /// determine the fitness of each tour in the generation
        /// </summary>
        /// <param name="generation">the generation of tours to be analysed</param>
        public void FitnessCheck(List<Tour> generation, ProblemData problemData)
        {
            if (generation == null || generation.Count == 0) return;


            //repeat for each tour in the generation
            for (int j = 0; j < generation.Count; j++)
            {
                //for each site in the tour find and add the total distance travelled between sites
                for (int i = 0; i < generation[j].sites.Count - 1; i++)
                {
                    generation[j].fitness += Distance(Operators.Sitelist[generation[j].sites[i]], Operators.Sitelist[generation[j].sites[i + 1]]);
                }

                //add the distance between the last and first sites, going back to the depot
                generation[j].fitness += Distance(Operators.Sitelist[generation[j].sites.Last()], Operators.Sitelist[generation[j].sites.First()]);

                if (problemData != null)
                {
                    // if fitness is better than the best fit then update the best fit - for both the current generation and overall
                    if (generation[j].fitness < problemData.genFit.fitness || problemData.genFit.fitness == 0)
                    {
                        problemData.genFit = new Tour
                        {
                            fitness = generation[j].fitness,
                            sites = generation[j].sites
                        };
                    }
                    if (generation[j].fitness < problemData.bestFit.fitness || problemData.bestFit.fitness == 0)
                    {
                        problemData.bestFit = new Tour
                        {
                            fitness = generation[j].fitness,
                            sites = generation[j].sites
                        };
                    }
                }

                //since a smaller distance is better, invert the distance for later evaluation
                generation[j].fitness = 1/ generation[j].fitness;
            }
        }

        /// <summary>
        /// determine the least fit member fo a generation
        /// </summary>
        /// <param name="generation">the generation to be searched</param>
        /// <returns>the least fit member of the generation</returns>
        public Tour LeastFit(List<Tour> generation)
        {
            if (generation == null) { return null; }

            //initialise return variable
            Tour worst = generation[0];

            //find the minimum fitness in the generatiion
            foreach (Tour t in generation)
            {
                if (t.fitness < worst.fitness)
                {
                    worst = t;
                }
            }

            //return the least fit member of the generation
            return worst;
        }
    }

    public class GAInitialisation
    {
        GAApplication GA = new GAApplication();
      

        /// <summary>
        /// Control initialisation methods
        /// </summary>
        /// <returns>the initilised generation</returns>
        public List<Tour> Initialisation()
        {
            //choose the correct initilisation method based on the users selection
            switch (Operators.InitialisationMethod)
            {
                case "Random":
                    return RandomInitialisation(Operators.genSize);
                case "Sorted":
                    return SortedInitialisation();
                case "Nearest Neighbour":
                    return NNInitialisation();
            }

            //if the code reaches here an error has occured
            return null;
        }

        /// <summary>
        /// initialise a generation of random tours
        /// </summary>
        /// <returns>the generated generation</returns>
        public List<Tour> RandomInitialisation(int GenerationSize)
        {
            //initialise local variables
            int cityNumber;
            List<Tour> generation = new List<Tour>();

            //for each desired tour in the generation
            for (int i = 0; i < GenerationSize; i++)
            {
                cityNumber = Operators.Sitelist.Count;
                Tour tour = new Tour
                {
                    sites = new List<int>()
                };

                //populate the tour with every site
                for (int j = 0; j < cityNumber; j++)
                {
                    tour.sites.Add(j);
                }

                //shuffle the tour
                while (cityNumber > 1)
                {
                    //decrement the number of unswapped cities
                    cityNumber--;
                    int k = Rand.Next(0,cityNumber + 1);

                    //swap the last unswapped city with a randomly picked city
                    (tour.sites[cityNumber], tour.sites[k]) = (tour.sites[k], tour.sites[cityNumber]);
                }

                //add the tour to the generation
                generation.Add(tour);
            }

            //return the shuffled generation
            return generation;
        }

        /// <summary>
        /// initialise a generation using sorted initialisation
        /// </summary>
        /// <returns>the initialised generation</returns>
        public List<Tour> SortedInitialisation()
        {
            //create twice as many tours as required
            List<Tour> generation = RandomInitialisation(Operators.genSize * 2);

            //evaluate the fitness of the generation
            GA.FitnessCheck(generation, null);

            //remove the least fit member of the generation until only the desired number remains
            while (generation.Count > Operators.genSize)
            {
                generation.RemoveAt(generation.IndexOf(GA.LeastFit(generation)));
            }

            //return the generation
            return generation;
        }

        /// <summary>
        /// initilise a generation using Nearest Neighbour Initialisation
        /// </summary>
        /// <returns></returns>
        public List<Tour> NNInitialisation()
        {
            //initialise local variables
            List<Tour> generation = new List<Tour>();

            //for the desired number of tours in the generation
            for (int i = 0; i < Operators.genSize; i++)
            {
                //populate a tour with a randomly selected first site
                Tour t = new Tour();
                t.sites.Add(Rand.Next(0,Operators.Sitelist.Count));

                //while the required number of sites has not been reached
                while (t.sites.Count < Operators.Sitelist.Count)
                {
                    double min = double.MaxValue;
                    int minSite = -1;

                    //find the site with the smallest distance to the last site in the tour that is not already in the tour
                    for (int j = 0; j < Operators.Sitelist.Count; j++)
                    {
                        if (t.sites.Contains(j)) continue;

                        if (GA.Distance(Operators.Sitelist[t.sites.Last()], Operators.Sitelist[j]) < min)
                        {
                            min = GA.Distance(Operators.Sitelist[t.sites.Last()], Operators.Sitelist[j]);
                            minSite = j;
                        }
                    }

                    //add the closest site
                    t.sites.Add(minSite);
                }

                //add the tour to the generation
                generation.Add(t);
            }

            //return the generation
            return generation;
        }
    }

    public class GASelection
    {
        GAApplication GA = new GAApplication();


        /// <summary>
        /// control selection method based on user input
        /// </summary>
        /// <param name="generation">the generation to be selected from</param>
        /// <returns>the selected parents</returns>
        public List<Tour> Selection(List<Tour> generation)
        {
            //randomly choose from one of the selected methods
            string method = Operators.SelectionMethods[Rand.Next(0, Operators.SelectionMethods.Count)];

            //choose the correct method of selection using the user imput
            switch (method)
            {
                case "Tournament":
                    return TournamentSelection(generation);
                case "Roulette":
                    return RouletteSelection(generation);
                case "Ranked":
                    return RankedSelection(generation);
            }

            //if the code reaches this far an error has occured
            return null;
        }

        /// <summary>
        /// perform tournament selection on the given generation
        /// </summary>
        /// <param name="generation">the generation to be selected from</param>
        /// <returns>the selected parents</returns>
        public List<Tour> TournamentSelection(List<Tour> generation)
        {
            if (generation == null) { return null; }

            //initialise parents variable
            List<Tour> parents = new List<Tour>();

            //initialise list of used tours
            List<int> used = new List<int>();

            //initialise index variable
            int index;

            //while less than two parents have been selected
            while (parents.Count < 2)
            {
                //choose a random unused tour to start with
                index = Rand.Next(0, generation.Count);
                while (used.Contains(index)) { index = Rand.Next(0, generation.Count);}
                Tour winner = generation[index];
                used.Add(index);

                //repeat three times
                for (int i = 0; i < 3; i++)
                {
                    //if a randomly selected tour is better than the current winner, replace the current winner
                    index = Rand.Next(0, generation.Count);
                    while (used.Contains(index)) { index = Rand.Next(0, generation.Count); }
                    used.Add(index);

                    if (generation[index].fitness > winner.fitness)
                    {
                        winner = generation[index];
                    }
                }

                //add the final winner to the parent list
                parents.Add(winner);
                used = new List<int> { used.First() };
            }

            return parents;
        }

        /// <summary>
        /// perform ranked selection on a given generation
        /// </summary>
        /// <param name="generation">the generation to be selected from</param>
        /// <returns>the selected parents</returns>
        public List<Tour> RankedSelection(List<Tour> generation)
        {
            if (generation == null) { return null; }

            //initilise local variables
            List<Tour> SortedGeneration = new List<Tour>();
            List<Tour> GenCopy = new List<Tour>(generation);
            double[] Problist = new double[generation.Count + 1];
            List<Tour> parents = new List<Tour>();
            double sum = 0;
            double probSum = 0;

            //sort the generation from least fit to best
            while (GenCopy.Count > 0)
            {
                SortedGeneration.Add(GenCopy.First());
                for(int i = 1; i < GenCopy.Count; i++)
                {
                    for(int j = 0; j < SortedGeneration.Count; j++)
                    {
                        if (GenCopy[i].fitness < SortedGeneration[j].fitness)
                        {
                            SortedGeneration.Insert(j, GenCopy[i]);
                        }else if(j == SortedGeneration.Count - 1)
                        {
                            SortedGeneration.Add(GenCopy[i]);
                        }
                    }
                }
            }

            //replace fitness with rank then add all fitnesses together
            for (int i = 0; i < SortedGeneration.Count; i++)
            {
                SortedGeneration[i].fitness = i;
                sum += SortedGeneration[i].fitness;
            }

            //dtermine the probability of each tour based on its fitness
            for (int i = 0; i < SortedGeneration.Count; i++)
            {
                Problist[i + 1] = probSum + (SortedGeneration[i].fitness / sum);
                probSum += SortedGeneration[i].fitness / sum;
            }

            //while less than two parents have been selected 
            while (parents.Count < 2)
            {
                //choose a random number between 0 and 1
                double roll = Rand.NextDouble();

                //add the corresponding tour based on its probability
                for (int k = 0; k < SortedGeneration.Count; k++)
                {
                    if (roll > Problist[k] && roll < Problist[k + 1] && !parents.Contains(SortedGeneration[k]))
                    {
                        parents.Add(SortedGeneration[k]);
                    }
                }
            }

            return parents;
        }

        /// <summary>
        /// select parents from a given generation using roulette selection
        /// </summary>
        /// <param name="generation">the generation to be selected from</param>
        /// <returns>the selected parents</returns>
        public List<Tour> RouletteSelection(List<Tour> generation)
        {
            if (generation == null) { return null; }

            //initialise local variables
            double sum = 0;
            double[] Probablities = new double[generation.Count + 1];
            double probSum = 0;
            List<Tour> parents = new List<Tour>();

            //add all fitnesses together
            for (int i = 0; i < generation.Count; i++)
            {
                sum += generation[i].fitness;
            }

            //dtermine the probability of each tour based on its fitness
            for (int i = 0; i < generation.Count; i++)
            {
                Probablities[i + 1] = probSum + (generation[i].fitness / sum);
                probSum += generation[i].fitness / sum;
            }

            //while there are less than two parents
            while (parents.Count < 2)
            {
                //select a random number
                double roll = Rand.NextDouble();
                for (int k = 0; k < generation.Count; k++)
                {
                    //add the selected parent to the parent list
                    if (roll > Probablities[k] && roll < Probablities[k + 1] && !parents.Contains(generation[k]))
                    {
                        parents.Add(generation[k]);
                    }
                }
            }

            return parents;
        }
    }

    public class GACrossover
    {
        /// <summary>
        /// perform crossover based on the users desired method
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>a list of child tours</returns>
        public List<Tour> Crossover(List<Tour> Parents)
        {
            //randomly choose a crossover method based on the users input
            string method = Operators.CrossoverMethods[Rand.Next(0, Operators.CrossoverMethods.Count)];

            //choose the correct method of crossover
            switch (method)
            {
                case "Partially Mapped":
                    return PMCrossover(Parents);
                case "GeneRepair":
                    return GeneRepairCrossover(Parents);
                case "Cycle":
                    return CycleCrossover(Parents);
                case "Ordered":
                    return OrderedCrossover(Parents);
                case "Order Based":
                    return OrderBasedCrossover(Parents);
                case "Position Based":
                    return PositionBasedCrossover(Parents);
            }

            //the code should not reach this far, if it does an error has occured
            return null;
        }

        /// <summary>
        /// Performs order based crossover on the selected parents
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>two children solutions</returns>
        public List<Tour> OrderBasedCrossover(List<Tour> Parents)
        {
            if (Parents == null) { return null; }
            List<Tour> Children = new List<Tour> {new Tour { sites = new List<int>(Parents[0].sites) }, new Tour { sites = new List<int>(Parents[1].sites) } };


            //for each of the two children
            for (int i = 0; i < 2; i++)
            {
                List<int> swapDigits = new List<int>();
                List<int> orderedDigits = new List<int>();

                //randomly determine the amount of digits to be swapped
                int amountSwapped = Rand.Next(2, Parents[0].sites.Count());

                //for each swapped digit
                for (int j = 0; j < amountSwapped; j++)
                {
                    //create a list of as many desired random digits that does not repeat
                    int digit;
                    do
                    {
                        digit = Rand.Next(0, Parents[0].sites.Count());
                    } while (swapDigits.Contains(digit));

                    swapDigits.Add(digit);
                }

                //for every destination in the other parents tour
                foreach (int site in Parents[Math.Abs(i - 1)].sites)
                {
                    //if the digit is one to be swapped, add it to the ordered list
                    if (swapDigits.Contains(site)) orderedDigits.Add(site);
                }

                //for every destination in a tour - unless the amount of digits still to be swapped is 0
                for (int j = 0; j < Parents[0].sites.Count() && orderedDigits.Count > 0; j++)
                {
                    //if the current digit is one to be swapped
                    if (swapDigits.Contains(Children[i].sites[j]))
                    {
                        //replace the digit with the next in the order, then remove the digit from the ordered list
                        Children[i].sites[j] = orderedDigits[0];
                        orderedDigits.RemoveAt(0);
                    }
                }
            }

            //return child tours
            return Children;
        }

        /// <summary>
        /// perform ordered crossove ron the selected parents
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>two children produced by the crossover</returns>
        public List<Tour> OrderedCrossover(List<Tour> Parents)
        {
            if (Parents == null) { return null; }

            //randomly determine start and end indexes of swapped portion
            int startIndex = Rand.Next(0, Parents[0].sites.Count() / 2);
            int endIndex = Rand.Next(startIndex + 1, Parents[0].sites.Count());

            List<Tour> Children = new List<Tour>();

            //for each child
            for (int i = 0; i < 2; i++)
            {
                Tour child = new Tour();

                //create a copy of the secondary parent
                List<int> temp = new List<int>(Parents[Math.Abs(i - 1)].sites);

                //reorder the copy so that index 0 is the beginning of the swapped portion
                List<int> ending = temp.GetRange(0, startIndex);

                foreach (int j in ending)
                {
                    temp.Remove(j);
                }

                temp.AddRange(ending);

                //initialise child site list
                child.sites = new List<int>(new int[Parents[0].sites.Count]);

                //for each destination in the swapped region
                for (int j = startIndex; j < endIndex + 1; j++)
                {
                    //copy the swapped region from the primary parent into the child
                    temp.Remove(Parents[i].sites[j]);
                    child.sites[j] = Parents[i].sites[j];
                }

                //for each destination in a tour
                for (int j = 0; j < Parents[0].sites.Count(); j++)
                {
                    //if the current index is outside the swapped region
                    if (!(j >= startIndex && j <= endIndex))
                    {
                        //populate the child with the reorded list from the secondary parent
                        child.sites[j] = temp.First();
                        temp.RemoveAt(0);
                    }
                }

                //add child to list
                Children.Add(child);
            }

            //return child list
            return Children;
        }

        /// <summary>
        /// perform gener repair crossovr on selected parents
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>two child tours</returns>
        public List<Tour> GeneRepairCrossover(List<Tour> Parents)
        {
            if (Parents == null) { return null; }

            GAInitialisation I = new GAInitialisation();

            //initilise list of children
            List<Tour> Children = new List<Tour> { new Tour { sites = new List<int>(Parents[0].sites) }, new Tour { sites = new List<int>(Parents[1].sites) } };

            //randomly determine start and end indexes of swapped portion
            int startIndex = Rand.Next(0, Parents[0].sites.Count() / 2);
            int endIndex = Rand.Next(startIndex + 1, Parents[0].sites.Count());

            //extract swapped portions
            List<int> Sublist1 = Parents[0].sites.GetRange(startIndex, endIndex - startIndex);
            List<int> Sublist2 = Parents[1].sites.GetRange(startIndex, endIndex - startIndex);

            //generate random tour to use as a template
            List<int> Template = I.RandomInitialisation(1)[0].sites;

            //swap the middle portion of both tours
            for (int i = startIndex; i < endIndex; i++)
            {
                Children[0].sites[i] = Sublist2[i - startIndex];
                Children[1].sites[i] = Sublist1[i - startIndex];
            }

            //for each child
            for (int j = 0; j < Children.Count; j++)
            {
                //initialise loop variables
                bool[] found = new bool[Children[0].sites.Count];
                List<int> TempTemplate = new List<int>(Template);

                //for each city
                for (int i = 0; i < Children[j].sites.Count; i++)
                {
                    //if the city is in the child already, remove it from the template
                    if (Children[j].sites.Contains(i)) TempTemplate.Remove(i);
                }

                //for each city while there are still numbers in the template
                for (int i = 0; i < Children[j].sites.Count && TempTemplate.Count > 0; i++)
                {
                    //if a city has already been found
                    if (found[Children[j].sites[i]])
                    {
                        //replace duplicate city with the first number in the template
                        Children[j].sites[i] = TempTemplate.First();
                        TempTemplate.RemoveAt(0);
                    }
                    //if the number hasn't already been found, mark it as found
                    if (!found[Children[j].sites[i]]) found[Children[j].sites[i]] = true;
                }
            }

            //return children
            return Children;
        }

        /// <summary>
        /// perform cycle crossover on the selected parents
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>two child tours</returns>
        public List<Tour> CycleCrossover(List<Tour> Parents)
        {
            if (Parents == null) { return null; }

            //initialise child tours
            List<Tour> Children = new List<Tour>();
            for (int i = 0; i < 2; i++)
            {
                Tour t = new Tour
                {
                    sites = new List<int>()
                };
                foreach (int x in Parents[0].sites)
                {
                    t.sites.Add(-1);
                }
                Children.Add(t);
            }


            //initialise primary and secondary parents
            int startparent = 0;
            int otherparent = 1;

            //for each child
            for (int i = 0; i < Children.Count; i++)
            {
                //while the child tour contains some unfilled sections
                while (Children[i].sites.Contains(-1))
                {
                    //swap primary and secondary parents
                    startparent = Math.Abs(startparent - 1);
                    otherparent = Math.Abs(otherparent - 1);

                    //set the start index to the first -1 in the child
                    int startindex = Children[i].sites.IndexOf(-1);
                    int index = startindex;

                    //set the first -1 to the value found in the same position in the primary parent
                    Children[i].sites[startindex] = Parents[startparent].sites[startindex];

                    //do ... while the cycle is not complete
                    do
                    {
                        //set the index to the position of the newly added value in the secondary parent
                        index = Parents[otherparent].sites.IndexOf(Parents[startparent].sites[index]);

                        //copy the value from the primary parent to the child
                        Children[i].sites[index] = Parents[startparent].sites[index];
                    } while (index != startindex);
                }
            }

            //return child list
            return Children;
        }


        /// <summary>
        /// perform partially mapped crossover on the selected parent tours
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>list of two child tours</returns>
        public List<Tour> PMCrossover(List<Tour> Parents)
        {

            if (Parents == null) { return null; }

            //initialise child tours
            List<Tour> Children = new List<Tour>();
            Children.Add(new Tour());
            Children.Add(new Tour());

            //randomly determine the positions of the swapped sections in the tour. The start position will be in the first half to help ensure an adequate length
            int startIndex = Rand.Next(0, Parents[0].sites.Count() / 2);
            int endIndex = Rand.Next(startIndex + 1, Parents[0].sites.Count());

            List<List<int>> Sublists = new List<List<int>>
            {
                //create sublists from each parent based on the random posiitions
                Parents[0].sites.GetRange(startIndex, endIndex - startIndex),
                Parents[1].sites.GetRange(startIndex, endIndex - startIndex)
            };

            //create map variables
            List<int> mapStart = new List<int>();
            List<int> mapEnd = new List<int>();

            //for each city in the sub tour
            for (int i = 0; i < Sublists[0].Count; i++)
            {
                //if both sublists contain a value that value is in the middle of a path and should be skipped for now
                if (!Sublists[1].Contains(Sublists[0][i]))
                {
                    //initilise a list containing the path of each number on the map
                    List<int> chain = new List<int>
                    {
                        //add the starting city
                        Sublists[0][i],

                        //add the corresponding city in the other sublist
                        Sublists[1][i]
                    };

                    //if the last city can be found in the starting sublist, add the corresponding ending city. repeat until the last city only appears in the ending sublist
                    while (Sublists[0].Contains(chain.Last()))
                    {
                        chain.Add(Sublists[1][Sublists[0].IndexOf(chain.Last())]);
                    }

                    //put the start and end points in the chain into map variables
                    mapStart.Add(chain.First());
                    mapEnd.Add(chain.Last());
                }
            }

            for (int j = 0; j < 2; j++)
            {
                //for each city in a parent
                for (int i = 0; i < Parents[0].sites.Count(); i++)
                {
                    //if the current index is outside the swapped region
                    if (!(i >= startIndex && i < endIndex))
                    {
                        //if the current city is found in either map variable then add the coresponding city, otherwise keep the original city from the parent
                        if (mapStart.Contains(Parents[j].sites[i]))
                        {
                            Children[j].sites.Add(mapEnd[mapStart.IndexOf(Parents[j].sites[i])]);
                        }
                        else if (mapEnd.Contains(Parents[j].sites[i]))
                        {
                            Children[j].sites.Add(mapStart[mapEnd.IndexOf(Parents[j].sites[i])]);
                        }
                        else
                        {
                            Children[j].sites.Add(Parents[j].sites[i]);
                        }

                    }
                    else
                    {
                        //if the index is in the swapped region then add it into the child dirtectly
                        Children[j].sites.Add(Sublists[Math.Abs(j - 1)][i - startIndex]);
                    }
              
                }
            }
            return Children;
        }

        /// <summary>
        /// Performs position based crossover on selected parent tours
        /// </summary>
        /// <param name="Parents">the parent tours</param>
        /// <returns>the child tours</returns>
        public List<Tour> PositionBasedCrossover(List<Tour> Parents)
        {
            if (Parents == null) { return null; }

            //initialise child tour list
            List<Tour> Children = new List<Tour>();
            for (int i = 0; i < 2; i++)
            {
                Tour t = new Tour
                {
                    sites = new List<int>()
                };

                //initilise all sites to -1, so it's clear which have not been replaced later
                foreach (int x in Parents[0].sites)
                {
                    t.sites.Add(-1);
                }
                Children.Add(t);
            }

            //randomly determine the number of positions to be preserved
            int PositionNum = Rand.Next(0, Parents[0].sites.Count());

            //create a list of randomly selected positions
            List<int> Positions = new List<int>();
            while (Positions.Count != PositionNum)
            {
                int position = Rand.Next(0, Parents[0].sites.Count());
                if (!Positions.Contains(position)) Positions.Add(position);
            }

            //for each child
            for (int i = 0; i < 2; i++)
            {
                //copy preserved positions
                foreach (int x in Positions)
                {
                    Children[i].sites[x] = Parents[i].sites[x];
                }

                //for each site in the child tour
                for (int j = 0; j < Parents[0].sites.Count(); j++)
                {
                    //if the site has already been replaced, move on to the next
                    if (Children[i].sites[j] != -1) continue;

                    //try to replace the current site with the site in the secondary parent tour
                    if (!Children[i].sites.Contains(Parents[Math.Abs(i - 1)].sites[j]))
                    {
                        Children[i].sites[j] = Parents[Math.Abs(i-1)].sites[j];
                    }
                    //otherwise try to replace the site with the site in the primary parent tour
                    else if (!Children[i].sites.Contains(Parents[i].sites[j]))
                    {
                        Children[i].sites[j] = Parents[i].sites[j];
                    }
                }

                //if some sites have still not ben replaced
                while (Children[i].sites.Contains(-1))
                {
                    //create a list of all missing sites from the child tour
                    List<int> missing = new List<int>();
                    for (int j = 0; j < Children[i].sites.Count; j++)
                    {
                        if (!Children[i].sites.Contains(j))
                        {
                            missing.Add(j);
                        }
                    }

                    //replace the -1 site with a randomly chosen site from the missing list - then remove site from the missing list
                    int randtour = Rand.Next(0, missing.Count);
                    Children[i].sites[Children[i].sites.IndexOf(-1)] = missing[randtour];
                    missing.RemoveAt(randtour);
                }
            }
            return Children;
        }
    }

    public class GAMutation
    {
        /// <summary>
        /// perform mutation based on the users desired method
        /// </summary>
        /// <param name="tour">the tour to be mutated</param>
        /// <returns>the mutated tour</returns>
        public Tour Mutation(Tour tour)
        {
            //choose a random method of mutation from the users selected methods
            string method = Operators.MutationMethods[Rand.Next(0, Operators.MutationMethods.Count)];

            Console.WriteLine(method);

            //choose the correct method of mutation based on the users choice
            switch (method)
            {
                case "Swap":
                    
                    return SwapMutation(tour);
                case "Reverse Sequence":
                    return ReverseSequenceMutation(tour);
                case "Partial Shuffle":
                    return PartialShuffleMutation(tour);
                case "Centre Inverse":
                    return CentreInverseMutation(tour);
            }

            //if the code reached this far, an error has occured
            return null;
        }

        /// <summary>
        /// Performs reverse sequence mutation on the given tour
        /// </summary>
        /// <param name="tour">the tour to be mutated</param>
        /// <returns>the mutated tour</returns>
        public Tour ReverseSequenceMutation(Tour tour)
        {
            if (tour == null) { return null; }

            //initialise local variables
            int startIndex = Rand.Next(0, tour.sites.Count / 2);
            int endIndex = Rand.Next(startIndex + 1, tour.sites.Count);
            Tour mutant = new Tour
            {
                sites = new List<int>()
            };
            int i;

            //copy the first section of the tour directly
            for (i = 0; i < startIndex; i++)
            {
                mutant.sites.Add(tour.sites[i]);
            }

            //add the middle section of the tour in reverse order
            for (int j = endIndex; j >= startIndex; j--)
            {
                mutant.sites.Add((tour.sites[j]));
            }

            //copy the ending of the tour directly
            for (i = endIndex + 1; i < tour.sites.Count; i++)
            {
                mutant.sites.Add(tour.sites[i]);
            }

            //return mutated tour
            return mutant;
        }

        /// <summary>
        /// perform swap mutation on a tour
        /// </summary>
        /// <param name="tour">the tour to be mutated</param>
        /// <returns>the mutated tour</returns>
        public Tour SwapMutation(Tour tour)
        {
            if (tour == null) { return null; }

            //randomly determine the indexes of the sites to be swapped
            int first = Rand.Next(0, tour.sites.Count - 1);
            int second = Rand.Next(0, tour.sites.Count - 1);

            //swap the selected sites
            (tour.sites[second], tour.sites[first]) = (tour.sites[first], tour.sites[second]);

            //return the mutated tour
            return tour;
        }

        /// <summary>
        /// Perform Partial shuffle mutation on the tour to be mutated
        /// </summary>
        /// <param name="tour">the tour to be mutated</param>
        /// <returns>the mutated tour</returns>
        public Tour PartialShuffleMutation(Tour tour)
        {
            if (tour == null) { return null; }

            //for each site in teh tour
            for (int i = 0; i < tour.sites.Count; i++)
            {
                //one in four chance for each city to be swapped
                if (Rand.NextDouble() < 0.25)
                {
                    //randomly determine the indexes of the site to be swapped
                    int swap = Rand.Next(0, tour.sites.Count - 1);

                    //if the swapped site = 1 keep shuffling until it doesn't
                    while (swap == i) swap = Rand.Next(0, tour.sites.Count - 1);

                    //swap the selected sites
                    (tour.sites[i], tour.sites[swap]) = (tour.sites[swap], tour.sites[i]);
                }
            }
            return tour;
        }

        /// <summary>
        /// mutate a tour using centre inverse mutation
        /// </summary>
        /// <param name="tour">the tour to be mutated</param>
        /// <returns>the mutated tour</returns>
        public Tour CentreInverseMutation(Tour tour)
        {
            if (tour == null) { return null; }

            //choose a random point in the tour
            int point = Rand.Next(1, tour.sites.Count - 1);

            //split the tour into two halves
            List<int> start = tour.sites.GetRange(0, point);
            List<int> end = tour.sites.GetRange(point, tour.sites.Count - point);

            //reverse the tour halves
            start.Reverse(); end.Reverse();

            //rebuild the halves into the tour
            tour.sites = start;
            tour.sites.AddRange(end);

            //return the tour
            return tour;
        }
    }
}
