namespace GAPlatform
{
    partial class GAForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Output = new System.Windows.Forms.RichTextBox();
            this.GenSize = new System.Windows.Forms.TextBox();
            this.GenSizeLabel = new System.Windows.Forms.Label();
            this.GenNum = new System.Windows.Forms.TextBox();
            this.GenNumLabel = new System.Windows.Forms.Label();
            this.ReplacementNum = new System.Windows.Forms.TextBox();
            this.ReplacementNumLabel = new System.Windows.Forms.Label();
            this.MutationRate = new System.Windows.Forms.TextBox();
            this.MutationRateLabel = new System.Windows.Forms.Label();
            this.SelectionLabel = new System.Windows.Forms.Label();
            this.SelectionCheck = new System.Windows.Forms.CheckedListBox();
            this.CrossoverCheck = new System.Windows.Forms.CheckedListBox();
            this.CrossoverLabel = new System.Windows.Forms.Label();
            this.MutationCheck = new System.Windows.Forms.CheckedListBox();
            this.MutationLabel = new System.Windows.Forms.Label();
            this.InitialisationCheck = new System.Windows.Forms.CheckedListBox();
            this.InitialisationLabel = new System.Windows.Forms.Label();
            this.DrawOutput = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DrawOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(260, 184);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(228, 48);
            this.button2.TabIndex = 1;
            this.button2.Text = "define problem";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Define_Problem);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Output
            // 
            this.Output.Location = new System.Drawing.Point(260, 12);
            this.Output.Name = "Output";
            this.Output.Size = new System.Drawing.Size(228, 157);
            this.Output.TabIndex = 2;
            this.Output.Text = "";
            // 
            // GenSize
            // 
            this.GenSize.Location = new System.Drawing.Point(146, 12);
            this.GenSize.Name = "GenSize";
            this.GenSize.Size = new System.Drawing.Size(100, 20);
            this.GenSize.TabIndex = 3;
            // 
            // GenSizeLabel
            // 
            this.GenSizeLabel.AutoSize = true;
            this.GenSizeLabel.Location = new System.Drawing.Point(12, 12);
            this.GenSizeLabel.Name = "GenSizeLabel";
            this.GenSizeLabel.Size = new System.Drawing.Size(85, 13);
            this.GenSizeLabel.TabIndex = 4;
            this.GenSizeLabel.Text = "Generation Size:";
            // 
            // GenNum
            // 
            this.GenNum.Location = new System.Drawing.Point(146, 42);
            this.GenNum.Name = "GenNum";
            this.GenNum.Size = new System.Drawing.Size(100, 20);
            this.GenNum.TabIndex = 5;
            // 
            // GenNumLabel
            // 
            this.GenNumLabel.AutoSize = true;
            this.GenNumLabel.Location = new System.Drawing.Point(12, 42);
            this.GenNumLabel.Name = "GenNumLabel";
            this.GenNumLabel.Size = new System.Drawing.Size(119, 13);
            this.GenNumLabel.TabIndex = 6;
            this.GenNumLabel.Text = "Number of Generations:";
            // 
            // ReplacementNum
            // 
            this.ReplacementNum.Location = new System.Drawing.Point(146, 69);
            this.ReplacementNum.Name = "ReplacementNum";
            this.ReplacementNum.Size = new System.Drawing.Size(100, 20);
            this.ReplacementNum.TabIndex = 7;
            // 
            // ReplacementNumLabel
            // 
            this.ReplacementNumLabel.AutoSize = true;
            this.ReplacementNumLabel.Location = new System.Drawing.Point(12, 69);
            this.ReplacementNumLabel.Name = "ReplacementNumLabel";
            this.ReplacementNumLabel.Size = new System.Drawing.Size(99, 13);
            this.ReplacementNumLabel.TabIndex = 8;
            this.ReplacementNumLabel.Text = "Replacement Rate:";
            // 
            // MutationRate
            // 
            this.MutationRate.Location = new System.Drawing.Point(146, 95);
            this.MutationRate.Name = "MutationRate";
            this.MutationRate.Size = new System.Drawing.Size(100, 20);
            this.MutationRate.TabIndex = 9;
            // 
            // MutationRateLabel
            // 
            this.MutationRateLabel.AutoSize = true;
            this.MutationRateLabel.Location = new System.Drawing.Point(13, 95);
            this.MutationRateLabel.Name = "MutationRateLabel";
            this.MutationRateLabel.Size = new System.Drawing.Size(94, 13);
            this.MutationRateLabel.TabIndex = 10;
            this.MutationRateLabel.Text = "Mutation Rate (%):";
            // 
            // SelectionLabel
            // 
            this.SelectionLabel.AutoSize = true;
            this.SelectionLabel.Location = new System.Drawing.Point(13, 190);
            this.SelectionLabel.Name = "SelectionLabel";
            this.SelectionLabel.Size = new System.Drawing.Size(104, 13);
            this.SelectionLabel.TabIndex = 12;
            this.SelectionLabel.Text = "Selection Method(s):";
            // 
            // SelectionCheck
            // 
            this.SelectionCheck.FormattingEnabled = true;
            this.SelectionCheck.Items.AddRange(new object[] {
            "Tournament",
            "Roulette",
            "Ranked"});
            this.SelectionCheck.Location = new System.Drawing.Point(126, 190);
            this.SelectionCheck.Name = "SelectionCheck";
            this.SelectionCheck.Size = new System.Drawing.Size(120, 49);
            this.SelectionCheck.TabIndex = 13;
            // 
            // CrossoverCheck
            // 
            this.CrossoverCheck.FormattingEnabled = true;
            this.CrossoverCheck.Items.AddRange(new object[] {
            "Partially Mapped",
            "GeneRepair",
            "Cycle",
            "Ordered",
            "Order Based",
            "Position Based"});
            this.CrossoverCheck.Location = new System.Drawing.Point(126, 245);
            this.CrossoverCheck.Name = "CrossoverCheck";
            this.CrossoverCheck.Size = new System.Drawing.Size(120, 94);
            this.CrossoverCheck.TabIndex = 14;
            // 
            // CrossoverLabel
            // 
            this.CrossoverLabel.AutoSize = true;
            this.CrossoverLabel.Location = new System.Drawing.Point(12, 245);
            this.CrossoverLabel.Name = "CrossoverLabel";
            this.CrossoverLabel.Size = new System.Drawing.Size(107, 13);
            this.CrossoverLabel.TabIndex = 15;
            this.CrossoverLabel.Text = "Crossover Method(s):";
            // 
            // MutationCheck
            // 
            this.MutationCheck.FormattingEnabled = true;
            this.MutationCheck.Items.AddRange(new object[] {
            "Reverse Sequence",
            "Swap",
            "Partial Shuffle",
            "Centre Inverse"});
            this.MutationCheck.Location = new System.Drawing.Point(126, 345);
            this.MutationCheck.Name = "MutationCheck";
            this.MutationCheck.Size = new System.Drawing.Size(120, 64);
            this.MutationCheck.TabIndex = 16;
            // 
            // MutationLabel
            // 
            this.MutationLabel.AutoSize = true;
            this.MutationLabel.Location = new System.Drawing.Point(11, 345);
            this.MutationLabel.Name = "MutationLabel";
            this.MutationLabel.Size = new System.Drawing.Size(101, 13);
            this.MutationLabel.TabIndex = 17;
            this.MutationLabel.Text = "Mutation Method(s):";
            // 
            // InitialisationCheck
            // 
            this.InitialisationCheck.FormattingEnabled = true;
            this.InitialisationCheck.Items.AddRange(new object[] {
            "Random",
            "Sorted",
            "Nearest Neighbour"});
            this.InitialisationCheck.Location = new System.Drawing.Point(126, 135);
            this.InitialisationCheck.Name = "InitialisationCheck";
            this.InitialisationCheck.Size = new System.Drawing.Size(120, 49);
            this.InitialisationCheck.TabIndex = 18;
            this.InitialisationCheck.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.InitialisationCheck_ItemCheck);
            // 
            // InitialisationLabel
            // 
            this.InitialisationLabel.AutoSize = true;
            this.InitialisationLabel.Location = new System.Drawing.Point(11, 135);
            this.InitialisationLabel.Name = "InitialisationLabel";
            this.InitialisationLabel.Size = new System.Drawing.Size(103, 13);
            this.InitialisationLabel.TabIndex = 19;
            this.InitialisationLabel.Text = "Initialisation Method:";
            // 
            // DrawOutput
            // 
            this.DrawOutput.Location = new System.Drawing.Point(260, 238);
            this.DrawOutput.Name = "DrawOutput";
            this.DrawOutput.Size = new System.Drawing.Size(228, 171);
            this.DrawOutput.TabIndex = 20;
            this.DrawOutput.TabStop = false;
            // 
            // GAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 414);
            this.Controls.Add(this.DrawOutput);
            this.Controls.Add(this.InitialisationLabel);
            this.Controls.Add(this.InitialisationCheck);
            this.Controls.Add(this.MutationLabel);
            this.Controls.Add(this.MutationCheck);
            this.Controls.Add(this.CrossoverLabel);
            this.Controls.Add(this.CrossoverCheck);
            this.Controls.Add(this.SelectionCheck);
            this.Controls.Add(this.SelectionLabel);
            this.Controls.Add(this.MutationRateLabel);
            this.Controls.Add(this.MutationRate);
            this.Controls.Add(this.ReplacementNumLabel);
            this.Controls.Add(this.ReplacementNum);
            this.Controls.Add(this.GenNumLabel);
            this.Controls.Add(this.GenNum);
            this.Controls.Add(this.GenSizeLabel);
            this.Controls.Add(this.GenSize);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.button2);
            this.Name = "GAForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.DrawOutput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RichTextBox Output;
        private System.Windows.Forms.TextBox GenSize;
        private System.Windows.Forms.Label GenSizeLabel;
        private System.Windows.Forms.TextBox GenNum;
        private System.Windows.Forms.Label GenNumLabel;
        private System.Windows.Forms.TextBox ReplacementNum;
        private System.Windows.Forms.Label ReplacementNumLabel;
        private System.Windows.Forms.TextBox MutationRate;
        private System.Windows.Forms.Label MutationRateLabel;
        private System.Windows.Forms.Label SelectionLabel;
        private System.Windows.Forms.CheckedListBox SelectionCheck;
        private System.Windows.Forms.CheckedListBox CrossoverCheck;
        private System.Windows.Forms.Label CrossoverLabel;
        private System.Windows.Forms.CheckedListBox MutationCheck;
        private System.Windows.Forms.Label MutationLabel;
        private System.Windows.Forms.CheckedListBox InitialisationCheck;
        private System.Windows.Forms.Label InitialisationLabel;
        private System.Windows.Forms.PictureBox DrawOutput;
    }
}

