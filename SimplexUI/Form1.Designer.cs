namespace SimplexUI
{
    partial class Form1
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
            panelFunc = new DbBufPanel();
            buttonReturnSize = new Button();
            labelFunc = new Label();
            textBoxFunc = new TextBox();
            labelFuncError = new Label();
            buttonFunc = new Button();
            listBoxIterations = new ListBox();
            labelIterations = new Label();
            panelFunc.SuspendLayout();
            SuspendLayout();
            // 
            // panelFunc
            // 
            panelFunc.Controls.Add(buttonReturnSize);
            panelFunc.Location = new System.Drawing.Point(404, -1);
            panelFunc.Name = "panelFunc";
            panelFunc.Size = new Size(396, 261);
            panelFunc.TabIndex = 0;
            panelFunc.Paint += PanelFunc_Paint;
            panelFunc.MouseClick += PanelFunc_MouseClick;
            panelFunc.MouseDown += PanelFunc_MouseDown;
            panelFunc.MouseMove += PanelFunc_MouseMove;
            panelFunc.MouseUp += PanelFunc_MouseUp;
            // 
            // buttonReturnSize
            // 
            buttonReturnSize.Location = new System.Drawing.Point(353, 226);
            buttonReturnSize.Name = "buttonReturnSize";
            buttonReturnSize.Size = new Size(31, 23);
            buttonReturnSize.TabIndex = 0;
            buttonReturnSize.Text = "1:1";
            buttonReturnSize.UseVisualStyleBackColor = true;
            buttonReturnSize.Click += ButtonReturnSize_Click;
            // 
            // labelFunc
            // 
            labelFunc.AutoSize = true;
            labelFunc.Location = new System.Drawing.Point(12, 9);
            labelFunc.Name = "labelFunc";
            labelFunc.Size = new Size(54, 15);
            labelFunc.TabIndex = 1;
            labelFunc.Text = "Function";
            // 
            // textBoxFunc
            // 
            textBoxFunc.Location = new System.Drawing.Point(12, 27);
            textBoxFunc.Name = "textBoxFunc";
            textBoxFunc.Size = new Size(262, 23);
            textBoxFunc.TabIndex = 2;
            textBoxFunc.TextChanged += TextBoxFunc_TextChanged;
            // 
            // labelFuncError
            // 
            labelFuncError.AutoSize = true;
            labelFuncError.ForeColor = Color.Red;
            labelFuncError.Location = new System.Drawing.Point(12, 53);
            labelFuncError.Name = "labelFuncError";
            labelFuncError.Size = new Size(38, 15);
            labelFuncError.TabIndex = 3;
            labelFuncError.Text = "! Error";
            // 
            // buttonFunc
            // 
            buttonFunc.Location = new System.Drawing.Point(280, 27);
            buttonFunc.Name = "buttonFunc";
            buttonFunc.Size = new Size(118, 23);
            buttonFunc.TabIndex = 5;
            buttonFunc.Text = "Calculate";
            buttonFunc.UseVisualStyleBackColor = true;
            buttonFunc.Click += ButtonFunc_Click;
            // 
            // listBoxIterations
            // 
            listBoxIterations.FormattingEnabled = true;
            listBoxIterations.ItemHeight = 15;
            listBoxIterations.Location = new System.Drawing.Point(12, 109);
            listBoxIterations.Name = "listBoxIterations";
            listBoxIterations.Size = new Size(386, 139);
            listBoxIterations.TabIndex = 6;
            // 
            // labelIterations
            // 
            labelIterations.AutoSize = true;
            labelIterations.Location = new System.Drawing.Point(12, 91);
            labelIterations.Name = "labelIterations";
            labelIterations.Size = new Size(59, 15);
            labelIterations.TabIndex = 7;
            labelIterations.Text = "Iterations:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 260);
            Controls.Add(labelIterations);
            Controls.Add(listBoxIterations);
            Controls.Add(buttonFunc);
            Controls.Add(panelFunc);
            Controls.Add(labelFuncError);
            Controls.Add(textBoxFunc);
            Controls.Add(labelFunc);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximumSize = new Size(816, 299);
            MinimumSize = new Size(816, 299);
            Name = "Form1";
            Text = "SimplexUI";
            panelFunc.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DbBufPanel panelFunc;
        private Label labelFunc;
        private TextBox textBoxFunc;
        private Label labelFuncError;
        private Button buttonFunc;
        private Button buttonReturnSize;
        private ListBox listBoxIterations;
        private Label labelIterations;
    }
}
