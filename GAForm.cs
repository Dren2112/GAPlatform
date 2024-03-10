using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAPlatform
{
 

    public partial class GAForm : Form
    {

        GAApplication GAApplication = new GAApplication();
        private static readonly Random rnd = new Random();

        public GAForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// takes in file from user that defines the TSP problem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Define_Problem(object sender, EventArgs e)
        {
            //opens a dialogue for user to select a file
            OpenFileDialog defineProblem = new OpenFileDialog();
            defineProblem.ShowDialog(this);

            //reads the selected file and splits it by line into an array
            string Problem = File.ReadAllText(defineProblem.FileName);
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

            //reads user input to initialise nad validate operator variables

            if (!int.TryParse(GenSize.Text, out Operators.genSize))
            {
                Output.Text = "Please enter a number into every text box";
                return;
            }

            if (Operators.genSize <= 0)
            {
                Output.Text = "Invalid Generation size. Generation size must be more than 0";
                return;
            }

            if (!int.TryParse(GenNum.Text, out Operators.runs))
            {
                Output.Text = "Please enter a number into every text box";
                return;
            }

            if (Operators.runs <= 0)
            {
                Output.Text = "Invalid number of generations. Number of generation must be more than 0";
                return;
            }

            if (!int.TryParse(ReplacementNum.Text, out Operators.crossover))
            {
                Output.Text = "Please enter a number into every text box";
                return;
            }

            if (Operators.crossover <= 0 || Operators.crossover > Operators.genSize)
            {
                Output.Text = "Invalid replacement number. Replacement number must be between 0 and generation size";
                return;
            }

            if (!double.TryParse(MutationRate.Text, out Operators.mutationRate))
            {
                Output.Text = "Please enter a number into every text box";
                return;
            }

            Operators.mutationRate /= 100;

            if (Operators.mutationRate < 0 || Operators.mutationRate > 1)
            {
                Output.Text = "Invalid mutation rate. Mutation rate must be between 0 and 100";
                return;
            }

            if (InitialisationCheck.CheckedItems.Count == 0 || SelectionCheck.CheckedItems.Count == 0 || CrossoverCheck.CheckedItems.Count == 0 || MutationCheck.CheckedItems.Count == 0)
            {
                Output.Text = "Please ensure at least one of each initialisation, selection, crossover, and mutation methods are selected";
                return;
            }

            Operators.InitialisationMethod = (string)InitialisationCheck.CheckedItems[0];

            for (int i = 0; i < SelectionCheck.CheckedItems.Count; i++)
            {
                Operators.SelectionMethods.Add((string)SelectionCheck.CheckedItems[i]);
            }

            for (int i = 0; i < CrossoverCheck.CheckedItems.Count; i++)
            {
                Operators.CrossoverMethods.Add((string)CrossoverCheck.CheckedItems[i]);
            }

            for (int i = 0; i < MutationCheck.CheckedItems.Count; i++)
            {
                Operators.MutationMethods.Add((string)MutationCheck.CheckedItems[i]);
            }

            //build and run the main genetic algorithm thread
            Thread thread = new Thread(() => GAApplication.GeneticAlgorithm(this));
            thread.Start();
        }

        /// <summary>
        /// output the best solutions to the textbox
        /// </summary>
        /// <param name="gen">the current generation</param>
        public void OutputBests(int gen)
        {
            try
            {
                Invoke(new Action(() =>
                    {
                        Output.Text = "Best Overall Fitness: " + ProblemData.bestFit.fitness.ToString() + '\n' + "Best fitness of generation " + gen.ToString() + ":" + ProblemData.genFit.fitness.ToString();
                    }
                    ));
            }
            catch
            {

            }
            DrawTour();
        }

       

        /// <summary>
        /// ensures only one initialisation method is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialisationCheck_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(e.NewValue == CheckState.Checked && InitialisationCheck.CheckedItems.Count > 0)
            {
                InitialisationCheck.ItemCheck -= InitialisationCheck_ItemCheck;
                InitialisationCheck.SetItemChecked(InitialisationCheck.CheckedIndices[0], false);
                InitialisationCheck.ItemCheck += InitialisationCheck_ItemCheck;
            }       
        }

        public (float, float) Scale()
        {
            float maxX = 0;
            float maxY = 0;
            foreach(Site s in ProblemData.Sitelist)
            {
                if (s.X > maxX) maxX = (float)s.X;
                if (s.Y > maxY) maxY = (float)s.Y;
            }

            float XScale = maxX / DrawOutput.Width;
            float YScale = maxY / DrawOutput.Height;

            return (XScale, YScale);
        }

        private void DrawTour()
        {
            var (xScale, yScale) = Scale();

            Tour Drawn = new Tour();
            Drawn.sites = ProblemData.genFit.sites;

            Drawn.sites.Add(Drawn.sites[0]);

            System.Drawing.Graphics g = DrawOutput.CreateGraphics();
            g.Clear(Color.White);

            PointF[] points = new PointF[Drawn.sites.Count];

            for (int i = 0; i < Drawn.sites.Count; i++)
            {
                PointF point = new PointF((float)ProblemData.Sitelist[Drawn.sites[i]].X / xScale, (float)ProblemData.Sitelist[Drawn.sites[i]].Y / yScale);
                points[i] = point;
            }

            Pen blackPen = new Pen(Color.Black);
            
            
            g.DrawPolygon(blackPen, points);

        }

    }
}
