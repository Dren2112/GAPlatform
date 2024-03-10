using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GAPlatform
{


    //TODO - see why pmx is causing duplicates
    //remember to remove test code



    public class Rand
    {
        public static Random rnd = new Random();
    }

    public class GAApplication
    {
        /// <summary>
        /// main method for the genetic algorithm
        /// </summary>
        public void GeneticAlgorithm(GAForm form)
        {
            GAInitialisation GAInitialisation = new GAInitialisation();
            GASelection GASelection = new GASelection();
            GACrossover GACrossover = new GACrossover();
            GAMutation GAMutation = new GAMutation();

            //initialise the first generation of solutions
            List<Tour> generation = GAInitialisation.Initialisation();

            //repeat for each generation wanted in the run
            for (int i = 0; i < Operators.runs; i++)
            {

                //initilise the list of new solutions
                List<Tour> nextGen = new List<Tour>();

                //assign a fitness value to each tour
                FitnessCheck(generation);

                //output the best generation and best overall every 10 runs
                if (i%10  == 0) { form.OutputBests(i); }

                //until the next generation has the desired number of new tours
                while (nextGen.Count != Operators.crossover)
                {
                    //select parents for crossover
                    List<Tour> selected = GASelection.Selection(generation);

                    //perform the crossover operation on the parents
                    List<Tour> children = GACrossover.Crossover(selected);
                    foreach (Tour t in children)
                    {
                        if (t.sites.Distinct().Count() != 52)
                        {
                            Console.WriteLine("Duplicate FOUND!!!");
                        }
                    }

                    //add the children to the list
                    for (int j = 0; j < children.Count; j++)
                    {
                        //mutate the child according to the mutation rate
                        if (Rand.rnd.NextDouble() < Operators.mutationRate)
                        {
                            children[j] = GAMutation.Mutation(children[j]);
                        }

                        if (children[j].sites.Distinct().Count() != 52)
                        {
                            Console.WriteLine("Duplicate FOUND!!!");
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

        }



        /// <summary>
        /// calculate the distance between two sites
        /// </summary>
        /// <param name="first">the first site</param>
        /// <param name="second">the second site</param>
        /// <returns>the distance between the two sites</returns>
        public double Distance(Site first, Site second)
        {
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
        public void FitnessCheck(List<Tour> generation)
        {

            //repeat for each tour in the generation
            for (int j = 0; j < generation.Count; j++)
            {
                //for each site in the tour find and add the total distance travelled between sites
                for (int i = 0; i < generation[j].sites.Count - 1; i++)
                {
                    generation[j].fitness += Distance(ProblemData.Sitelist[generation[j].sites[i]], ProblemData.Sitelist[generation[j].sites[i + 1]]);
                }

                //add the distance between the last and first sites, going bacl t the depot
                generation[j].fitness += Distance(ProblemData.Sitelist[generation[j].sites.Last()], ProblemData.Sitelist[generation[j].sites.First()]);

                // if fitness is better than the best fit then update the best fit - for both the current generation and overall
                if (generation[j].fitness < ProblemData.genFit.fitness || ProblemData.genFit.fitness == 0)
                {
                    ProblemData.genFit = new Tour
                    {
                        fitness = generation[j].fitness,
                        sites = generation[j].sites
                    };
                }
                if (generation[j].fitness < ProblemData.bestFit.fitness || ProblemData.bestFit.fitness == 0) 
                {
                    ProblemData.bestFit = new Tour
                    {
                        fitness = generation[j].fitness,
                        sites = generation[j].sites
                    };
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
                cityNumber = ProblemData.Sitelist.Count;
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
                    int k = Rand.rnd.Next(cityNumber + 1);

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
            //initialise local variables
            int cityNumber;
            List<Tour> generation = new List<Tour>();

            //create twice as many tours as required
            for (int i = 0; i < Operators.genSize * 2; i++)
            {
                cityNumber = ProblemData.Sitelist.Count;

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
                    int k = Rand.rnd.Next(cityNumber + 1);

                    //swap the last unswapped city with a randomly picked city
                    (tour.sites[cityNumber], tour.sites[k]) = (tour.sites[k], tour.sites[cityNumber]);
                }

                //add the tour to the generation
                generation.Add(tour);
            }

            //evaluate the fitness of the generation
            GA.FitnessCheck(generation);

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
                t.sites.Add(Rand.rnd.Next(ProblemData.Sitelist.Count));

                //while the required number of sites has not been reached
                while (t.sites.Count < ProblemData.Sitelist.Count)
                {
                    double min = double.MaxValue;
                    int minSite = -1;

                    //find the site with the smallest distance to the last site in the tour that is not already in the tour
                    for (int j = 0; j < ProblemData.Sitelist.Count; j++)
                    {
                        if (t.sites.Contains(j)) continue;

                        if (GA.Distance(ProblemData.Sitelist[t.sites.Last()], ProblemData.Sitelist[j]) < min)
                        {
                            min = GA.Distance(ProblemData.Sitelist[t.sites.Last()], ProblemData.Sitelist[j]);
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
            string method = Operators.SelectionMethods[Rand.rnd.Next(0, Operators.SelectionMethods.Count)];

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
            //initialise parents variable
            List<Tour> parents = new List<Tour>();

            //while less than two parents have been selected
            while (parents.Count < 2)
            {
                //choose a random tour to start with
                Tour winner = generation[Rand.rnd.Next(0, generation.Count)];

                //repeat three times
                for (int i = 0; i < 3; i++)
                {
                    //if a randomly selected tour is better than the current winner, replace the current winner
                    int index = Rand.rnd.Next(0, generation.Count);
                    if (generation[index].fitness > winner.fitness)
                    {
                        winner = generation[index];
                    }
                }

                //add the final winner to the parent list
                parents.Add(winner);
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
                SortedGeneration.Add(GA.LeastFit(GenCopy));
                GenCopy.RemoveAt(GenCopy.IndexOf(GA.LeastFit(GenCopy)));
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
                double roll = Rand.rnd.NextDouble();

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
                double roll = Rand.rnd.NextDouble();
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
            string method = Operators.CrossoverMethods[Rand.rnd.Next(0, Operators.CrossoverMethods.Count)];

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

            List<Tour> Children = new List<Tour>(Parents);

            //for each of the two children
            for (int i = 0; i < 2; i++)
            {
                List<int> swapDigits = new List<int>();
                List<int> orderedDigits = new List<int>();

                //randomly determine the amount of digits to be swapped
                int amountSwapped = Rand.rnd.Next(2, ProblemData.Sitelist.Count);

                //for each swapped digit
                for (int j = 0; j < amountSwapped; j++)
                {
                    //create a list of as many desired random digits that does not repeat
                    int digit;
                    do
                    {
                        digit = Rand.rnd.Next(0, ProblemData.Sitelist.Count);
                    } while (swapDigits.Contains(digit));

                    swapDigits.Add(digit);
                }

                //for every destination in the other parents tour
                foreach (int site in Parents[Math.Abs(i - 1)].sites)
                {
                    //if the digit is one to be swapped, add it to the ordered list
                    if (swapDigits.Contains(site)) orderedDigits.Add(site);
                }

                //for every destination in a tour - uless the amount of digits still to be swapped is 0
                for (int j = 0; j < ProblemData.Sitelist.Count && orderedDigits.Count > 0; j++)
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
            //randomly determine start and end indexes of swapped portion
            int startIndex = Rand.rnd.Next(0, ProblemData.Sitelist.Count / 2);
            int endIndex = Rand.rnd.Next(startIndex + 1, ProblemData.Sitelist.Count);

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
                for (int j = 0; j < ProblemData.Sitelist.Count; j++)
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
            GAInitialisation I = new GAInitialisation();

            //initilise list of children
            List<Tour> Children = new List<Tour>(Parents);

            //randomly determine start and end indexes of swapped portion
            int startIndex = Rand.rnd.Next(0, ProblemData.Sitelist.Count / 2);
            int endIndex = Rand.rnd.Next(startIndex + 1, ProblemData.Sitelist.Count);

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
            //initialise child tours
            List<Tour> Children = new List<Tour>();
            for (int i = 0; i < 2; i++)
            {
                Tour t = new Tour
                {
                    sites = new List<int>()
                };
                foreach (Site x in ProblemData.Sitelist)
                {
                    t.sites.Add(-1);
                }
                Children.Add(t);
            }


            //identify primary and secondary parents
            int startparent = Rand.rnd.Next(0, Parents.Count);
            int otherparent = Math.Abs(startparent - 1);

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

        //TODO - Re comment
        /// <summary>
        /// perform partially mapped crossover on the selected parent tours
        /// </summary>
        /// <param name="Parents">the selected parent tours</param>
        /// <returns>list of two child tours</returns>
        public List<Tour> PMCrossover(List<Tour> Parents)
        {

            //initialise child tours
            List<Tour> Children = new List<Tour>();
            Children.Add(new Tour());
            Children.Add(new Tour());

            //randomly determine the positions of the swapped sections in the tour. The start position will be in the first half to help ensure an adequate length
            int startIndex = Rand.rnd.Next(0, ProblemData.Sitelist.Count / 2);
            int endIndex = Rand.rnd.Next(startIndex + 1, ProblemData.Sitelist.Count);

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
                for (int i = 0; i < ProblemData.Sitelist.Count; i++)
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

                 
                    if (Children[j].sites.Distinct().Count() != Children[j].sites.Count())
                    {
                        Console.WriteLine("Duplicate FOUND!!!");
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
            //initialise child tour list
            List<Tour> Children = new List<Tour>();
            for (int i = 0; i < 2; i++)
            {
                Tour t = new Tour
                {
                    sites = new List<int>()
                };

                //initilise all sites to -1, so it's clear which have not been replaced later
                foreach (Site x in ProblemData.Sitelist)
                {
                    t.sites.Add(-1);
                }
                Children.Add(t);
            }

            //randomly dtermone the number of positions to be preserved
            int PositionNum = Rand.rnd.Next(0, ProblemData.Sitelist.Count);

            //create a list of randomly selected positions
            List<int> Positions = new List<int>();
            while (Positions.Count != PositionNum)
            {
                int position = Rand.rnd.Next(0, ProblemData.Sitelist.Count);
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
                for (int j = 0; j < ProblemData.Sitelist.Count; j++)
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
                    int randtour = Rand.rnd.Next(0, missing.Count);
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
            string method = Operators.MutationMethods[Rand.rnd.Next(0, Operators.MutationMethods.Count)];

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
            //initialise local variables
            int startIndex = Rand.rnd.Next(0, tour.sites.Count / 2);
            int endIndex = Rand.rnd.Next(startIndex + 1, tour.sites.Count);
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
            //randomly determine the indexes of the sites to be swapped
            int first = Rand.rnd.Next(0, tour.sites.Count - 1);
            int second = Rand.rnd.Next(0, tour.sites.Count - 1);

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
            //for each site in teh tour
            for (int i = 0; i < tour.sites.Count; i++)
            {
                //randomly determined based on mutation operator
                if (Rand.rnd.NextDouble() < Operators.mutationRate)
                {
                    //randomly determine the indexes of the site to be swapped
                    int swap = Rand.rnd.Next(0, tour.sites.Count - 1);

                    //if the swapped site = 1 keep shuffling until it doesn't
                    while (swap == i) swap = Rand.rnd.Next(0, tour.sites.Count - 1);

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
            //choose a random point in the tour
            int point = Rand.rnd.Next(1, tour.sites.Count - 1);

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
