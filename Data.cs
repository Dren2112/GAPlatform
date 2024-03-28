using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAPlatform
{
    public static class Operators
    {
        public static int genSize;
        public static int runs;
        public static int crossover;
        public static double mutationRate;
        public static String InitialisationMethod;
        public static List<String> SelectionMethods = new List<string>();
        public static List<String> CrossoverMethods = new List<string>();
        public static List<String> MutationMethods = new List<string>();
        public static List<Site> Sitelist = new List<Site>();
    }

    public class ProblemData
    {
        public Tour bestFit = new Tour();
        public Tour genFit = new Tour();
    }

    public class Site
    {
        public double X;
        public double Y;
    }

    public class Tour
    {
        public double fitness = 0;
        public List<int> sites = new List<int>();
    }
}
