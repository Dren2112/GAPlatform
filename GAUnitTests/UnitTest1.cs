using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GAPlatform;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace GAUnitTests
{
    [TestClass]
    public class Initialisation
    {
        [TestMethod]
        public void Random()
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
            GAInitialisation initialisation = new GAInitialisation();
            Random rnd = new Random();

            //ACT
            List<Tour> result = initialisation.RandomInitialisation(50);

            //ASSERT
            Assert.AreEqual(result.Count, 50);

            foreach (Tour t in result)
            {
                Assert.IsTrue(t.sites.Count() == t.sites.Distinct().Count());
            }

            int first = rnd.Next(50);
            int second = rnd.Next(50);
            while(first == second) second = rnd.Next(50);

            for(int i = 0; i <10; i++)
            {
                Assert.IsFalse(result[first].sites.SequenceEqual(result[second].sites));
            }
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
            Operators.genSize = 50;
            GAInitialisation initialisation = new GAInitialisation();
            Random rnd = new Random();


            //ACT
            List<Tour> result = initialisation.SortedInitialisation();

            //ASSERT
            Assert.AreEqual(result.Count, 50);

            foreach (Tour t in result)
            {
                Assert.IsTrue(t.sites.Count() == t.sites.Distinct().Count());
            }

            int first = rnd.Next(50);
            int second = rnd.Next(50);
            while (first == second) second = rnd.Next(50);

            for (int i = 0; i <10; i++)
            {
                Assert.IsFalse(result[first].sites.SequenceEqual(result[second].sites));
            }
        }
    }

    [TestClass]
    public class HelperFunctions
    {
        [TestMethod]
        public void Leastfit()
        {
            //ARRANGE
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
        public void FitnessCheck()
        {
            //ARRANGE
            GAApplication GAApplication = new GAApplication();
            List<int> Sites = new List<int>();
            for(int i = 0; i < 10; i++)
            {
                Sites.Add(i);
            }
            Tour t = new Tour { sites = Sites };
            List<Tour> list = new List<Tour> { t };

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
    }
}
