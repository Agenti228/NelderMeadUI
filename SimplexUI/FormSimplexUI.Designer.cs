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
            buttonResetView = new Button();
            labelFunction = new Label();
            textBoxFunction = new TextBox();
            labelFunctionError = new Label();
            listBoxIterations = new ListBox();
            labelIterations = new Label();
            groupBoxControlElements = new GroupBox();
            groupBoxControlElements.SuspendLayout();
            SuspendLayout();
            // 
            // panelFunction
            // 
            panelFunction.Dock = DockStyle.Fill;
            panelFunction.Location = new Point(0, 0);
            panelFunction.Name = "panelFunction";
            panelFunction.Size = new Size(1584, 861);
            panelFunction.TabIndex = 0;
            panelFunction.Paint += PanelFunction_Paint;
            panelFunction.MouseDown += PanelFunction_MouseDown;
            panelFunction.MouseMove += PanelFunction_MouseMove;
            panelFunction.MouseUp += PanelFunction_MouseUp;
            panelFunction.MouseWheel += PanelFunction_MouseWheel;
            // 
            // buttonResetView
            // 
            buttonResetView.Location = new Point(351, 96);
            buttonResetView.Name = "buttonResetView";
            buttonResetView.Size = new Size(83, 23);
            buttonResetView.TabIndex = 0;
            buttonResetView.Text = "Reset view";
            buttonResetView.UseVisualStyleBackColor = true;
            buttonResetView.Click += ButtonResetView_Click;
            // 
            // labelFunction
            // 
            labelFunction.AutoSize = true;
            labelFunction.Location = new Point(0, 4);
            labelFunction.Name = "labelFunction";
            labelFunction.Size = new Size(54, 15);
            labelFunction.TabIndex = 1;
            labelFunction.Text = "Function";
            // 
            // textBoxFunction
            // 
            textBoxFunction.Location = new Point(0, 22);
            textBoxFunction.Name = "textBoxFunction";
            textBoxFunction.Size = new Size(434, 23);
            textBoxFunction.TabIndex = 2;
            textBoxFunction.KeyDown += TextBoxFunction_KeyDown;
            // 
            // labelFunctionError
            // 
            labelFunctionError.AutoSize = true;
            labelFunctionError.ForeColor = Color.Red;
            labelFunctionError.Location = new Point(0, 48);
            labelFunctionError.Name = "labelFunctionError";
            labelFunctionError.Size = new Size(119, 15);
            labelFunctionError.TabIndex = 3;
            labelFunctionError.Text = "INVALID EXPRESSION";
            labelFunctionError.Visible = false;
            // 
            // listBoxIterations
            // 
            listBoxIterations.FormattingEnabled = true;
            listBoxIterations.ItemHeight = 15;
            listBoxIterations.Location = new Point(0, 122);
            listBoxIterations.Name = "listBoxIterations";
            listBoxIterations.Size = new Size(437, 739);
            listBoxIterations.TabIndex = 6;
            // 
            // labelIterations
            // 
            labelIterations.AutoSize = true;
            labelIterations.Location = new Point(0, 104);
            labelIterations.Name = "labelIterations";
            labelIterations.Size = new Size(59, 15);
            labelIterations.TabIndex = 7;
            labelIterations.Text = "Iterations:";
            // 
            // groupBoxControlElements
            // 
            groupBoxControlElements.Controls.Add(buttonResetView);
            groupBoxControlElements.Controls.Add(labelIterations);
            groupBoxControlElements.Controls.Add(listBoxIterations);
            groupBoxControlElements.Controls.Add(textBoxFunction);
            groupBoxControlElements.Controls.Add(labelFunction);
            groupBoxControlElements.Controls.Add(labelFunctionError);
            groupBoxControlElements.Dock = DockStyle.Right;
            groupBoxControlElements.Location = new Point(1147, 0);
            groupBoxControlElements.Name = "groupBoxControlElements";
            groupBoxControlElements.Size = new Size(437, 861);
            groupBoxControlElements.TabIndex = 8;
            groupBoxControlElements.TabStop = false;
            // 
            // FormSimplexUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 861);
            Controls.Add(groupBoxControlElements);
            Controls.Add(panelFunction);
            DoubleBuffered = true;
            MaximumSize = new Size(6000, 4000);
            MinimumSize = new Size(600, 400);
            Name = "FormSimplexUI";
            Text = "Nelder Mead";
            Resize += FormSimplexUI_Resize;
            groupBoxControlElements.ResumeLayout(false);
            groupBoxControlElements.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DoubleBufferedPanel panelFunction;
        private Label labelFunction;
        private TextBox textBoxFunction;
        private Label labelFunctionError;
        private Button buttonResetView;
        private ListBox listBoxIterations;
        private Label labelIterations;
        private GroupBox groupBoxControlElements;
    }
}
