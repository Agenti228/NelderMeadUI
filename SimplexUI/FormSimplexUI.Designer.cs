namespace SimplexUI
{
    partial class FormSimplexUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelFunction = new DoubleBufferedPanel();
            buttonReturnSize = new Button();
            labelFunction = new Label();
            textBoxFunction = new TextBox();
            labelFunctionError = new Label();
            buttonFunction = new Button();
            listBoxIterations = new ListBox();
            labelIterations = new Label();
            panelFunction.SuspendLayout();
            SuspendLayout();
            // 
            // panelFunction
            // 
            panelFunction.AutoSize = true;
            panelFunction.Controls.Add(buttonReturnSize);
            panelFunction.Location = new Point(404, 0);
            panelFunction.Name = "panelFunction";
            panelFunction.Size = new Size(1180, 861);
            panelFunction.TabIndex = 0;
            panelFunction.Paint += PanelFunction_Paint;
            panelFunction.MouseClick += PanelFunction_MouseClick;
            panelFunction.MouseDown += PanelFunction_MouseDown;
            panelFunction.MouseMove += PanelFunction_MouseMove;
            panelFunction.MouseUp += PanelFunction_MouseUp;
            // 
            // buttonReturnSize
            // 
            buttonReturnSize.Location = new Point(1094, 835);
            buttonReturnSize.Name = "buttonReturnSize";
            buttonReturnSize.Size = new Size(83, 23);
            buttonReturnSize.TabIndex = 0;
            buttonReturnSize.Text = "Reset scale";
            buttonReturnSize.UseVisualStyleBackColor = true;
            buttonReturnSize.Click += ButtonReturnSize_Click;
            // 
            // labelFunction
            // 
            labelFunction.AutoSize = true;
            labelFunction.Location = new Point(12, 9);
            labelFunction.Name = "labelFunction";
            labelFunction.Size = new Size(54, 15);
            labelFunction.TabIndex = 1;
            labelFunction.Text = "Function";
            // 
            // textBoxFunction
            // 
            textBoxFunction.Location = new Point(12, 27);
            textBoxFunction.Name = "textBoxFunction";
            textBoxFunction.Size = new Size(262, 23);
            textBoxFunction.TabIndex = 2;
            textBoxFunction.TextChanged += TextBoxFunction_TextChanged;
            // 
            // labelFunctionError
            // 
            labelFunctionError.AutoSize = true;
            labelFunctionError.ForeColor = Color.Red;
            labelFunctionError.Location = new Point(12, 53);
            labelFunctionError.Name = "labelFunctionError";
            labelFunctionError.Size = new Size(49, 15);
            labelFunctionError.TabIndex = 3;
            labelFunctionError.Text = "! ERROR";
            // 
            // buttonFunction
            // 
            buttonFunction.Location = new Point(280, 27);
            buttonFunction.Name = "buttonFunction";
            buttonFunction.Size = new Size(118, 23);
            buttonFunction.TabIndex = 5;
            buttonFunction.Text = "Calculate";
            buttonFunction.UseVisualStyleBackColor = true;
            buttonFunction.Click += ButtonFunction_Click;
            // 
            // listBoxIterations
            // 
            listBoxIterations.FormattingEnabled = true;
            listBoxIterations.ItemHeight = 15;
            listBoxIterations.Location = new Point(12, 109);
            listBoxIterations.Name = "listBoxIterations";
            listBoxIterations.Size = new Size(386, 739);
            listBoxIterations.TabIndex = 6;
            // 
            // labelIterations
            // 
            labelIterations.AutoSize = true;
            labelIterations.Location = new Point(12, 91);
            labelIterations.Name = "labelIterations";
            labelIterations.Size = new Size(59, 15);
            labelIterations.TabIndex = 7;
            labelIterations.Text = "Iterations:";
            // 
            // FormSimplexUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 861);
            Controls.Add(labelIterations);
            Controls.Add(listBoxIterations);
            Controls.Add(buttonFunction);
            Controls.Add(panelFunction);
            Controls.Add(labelFunctionError);
            Controls.Add(textBoxFunction);
            Controls.Add(labelFunction);
            DoubleBuffered = true;
            MaximumSize = new Size(6000, 4000);
            MinimumSize = new Size(600, 400);
            Name = "FormSimplexUI";
            Text = "Nelder Mead";
            panelFunction.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DoubleBufferedPanel panelFunction;
        private Label labelFunction;
        private TextBox textBoxFunction;
        private Label labelFunctionError;
        private Button buttonFunction;
        private Button buttonReturnSize;
        private ListBox listBoxIterations;
        private Label labelIterations;
    }
}
