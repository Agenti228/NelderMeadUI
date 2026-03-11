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
            labelFunc = new Label();
            textBoxFunc = new TextBox();
            labelFuncError = new Label();
            buttonFunc = new Button();
            SuspendLayout();
            // 
            // panelFunc
            // 
            panelFunc.Location = new System.Drawing.Point(404, -1);
            panelFunc.Name = "panelFunc";
            panelFunc.Size = new Size(396, 261);
            panelFunc.TabIndex = 0;
            panelFunc.Paint += PanelFunc_Paint;
            panelFunc.MouseClick += panelFunc_MouseClick;
            panelFunc.MouseDown += panelFunc_MouseDown;
            panelFunc.MouseMove += panelFunc_MouseMove;
            panelFunc.MouseUp += panelFunc_MouseUp;
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
            textBoxFunc.TextChanged += textBoxFunc_TextChanged;
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
            buttonFunc.Click += buttonFunc_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 260);
            Controls.Add(buttonFunc);
            Controls.Add(panelFunc);
            Controls.Add(labelFuncError);
            Controls.Add(textBoxFunc);
            Controls.Add(labelFunc);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DbBufPanel panelFunc;
        private Label labelFunc;
        private TextBox textBoxFunc;
        private Label labelFuncError;
        private Button buttonFunc;
    }
}
