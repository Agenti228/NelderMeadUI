namespace SimplexUI
{
    public partial class Form1 : Form
    {
        public struct BezierPoint
        {
            public PointF position;
            public float weight;
        }
        //функция

        //скаляры
        private float scaler;
        //стиль
        private Color funcColor;
        private Brush backBrush;
        //точки
        private PointF centerXY;
        private PointF lastXY; //для определения дельты перемещения графика
        //флаги
        private bool isMovingScreen = false;

        private List<Point[]> simplexes = new List<Point[]>();

        Function expr;


        public Form1()
        {
            InitializeComponent();
            SetStartParametres();
            funcColor = Color.Red;
            panelFunc.MouseWheel += PanelFunc_MouseWheel;
            backBrush = new SolidBrush(Color.White);
            expr = new Function(textBoxFunc.Text);
        }

        #region XY methods
        private void SetStartParametres()
        {
            centerXY = new PointF((panelFunc.Right - panelFunc.Left) / 2, (panelFunc.Bottom - panelFunc.Top) / 2);
            scaler = 40;
            buttonReturnSize.Visible = buttonReturnSize.Enabled = labelFuncError.Visible = false;
            listBoxIterations.Items.Clear();
        }

        private void XYLinesInit(Graphics g)
        {
            try
            {
                float[] dash = { scaler / 8, scaler / 8 };
                Pen p = new Pen(Color.Black);
                p.Width = 2;
                g.DrawLine(p, centerXY.X, 0, centerXY.X, panelFunc.ClientSize.Height);
                g.DrawLine(p, 0, centerXY.Y, panelFunc.ClientSize.Width, centerXY.Y);
                p.DashPattern = dash;
                g.DrawEllipse(p, centerXY.X - scaler, centerXY.Y - scaler, scaler * 2, scaler * 2);
                p.Dispose();
            }
            catch
            {
                MessageBox.Show("Out of memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStartParametres();
                panelFunc.Invalidate();
            }
        }

        private PointF PanelToFuncXY(PointF pnt)
        {
            if (pnt.Y > centerXY.Y) pnt.Y = -Math.Abs(pnt.Y - centerXY.Y) / scaler;
            else pnt.Y = Math.Abs(centerXY.Y - pnt.Y) / scaler;
            if (pnt.X > centerXY.X) pnt.X = Math.Abs(pnt.X - centerXY.X) / scaler;
            else pnt.X = -Math.Abs(centerXY.X - pnt.X) / scaler;
            return pnt;
        }

        private PointF FuncToPanelXY(PointF pnt)
        {
            pnt.X = scaler * pnt.X + centerXY.X;
            pnt.Y = -scaler * pnt.Y + centerXY.Y;
            return pnt;
        }
        #endregion

        #region Draw methods
        private void DrawFunc(Graphics g)
        {
            List<PointF> funcPointList = new List<PointF>();
            PointF pnt = new System.Drawing.Point();
            Pen p = new Pen(funcColor);
            p.Width = 2;
            if (expr != null && expr.IsCorrect)
            {
                for (float i = 0; i < panelFunc.Width; i++)
                {
                    pnt.X = i;
                    pnt.Y = 0;
                    pnt = PanelToFuncXY(pnt);
                    expr.TryCalculate(pnt.X, out var result);
                    pnt.Y = (float)result;
                    funcPointList.Add(FuncToPanelXY(pnt));
                }

                for (int i = 1; i < funcPointList.Count(); i++)
                {
                    if (Math.Abs(funcPointList[i - 1].Y - funcPointList[i].Y) < panelFunc.Height)
                    {
                        g.DrawLine(p, funcPointList[i - 1], funcPointList[i]);
                    }

                }
            }
        }
        private void DrawSimplex(Graphics g)
        {

            PointF pnt = new System.Drawing.Point();
            Pen p = new Pen(Color.DeepPink);
            p.Width = 2;
            if (expr != null && expr.IsCorrect)
            {
                for (int i = 0; i < simplexes.Count; i++)
                {
                    for (int j = 1; j < simplexes[i].Length; j++)
                    {
                        PointF frstPnt = FuncToPanelXY(new PointF((float)simplexes[i][j - 1][0], (float)simplexes[i][j - 1].Value));
                        PointF scndPnt = FuncToPanelXY(new PointF((float)simplexes[i][j][0], (float)simplexes[i][j].Value));
                        g.DrawLine(p, frstPnt, scndPnt);
                        g.DrawRectangle(Pens.Blue, frstPnt.X - 2, frstPnt.Y - 2, 4, 4);
                        g.DrawRectangle(Pens.Blue, scndPnt.X - 2, scndPnt.Y - 2, 4, 4);
                    }
                }
            }
        }
        #endregion

        #region Buttons

        private void ButtonColorFunc_Click(object sender, EventArgs e)
        {
            ColorDialog colorChanger = new ColorDialog();
            colorChanger.FullOpen = true;
            colorChanger.Color = funcColor;
            if (colorChanger.ShowDialog() == DialogResult.Cancel) return;
            funcColor = colorChanger.Color;
            panelFunc.Invalidate();
        }

        private void ButtonStartScale_Click(object sender, EventArgs e)
        {
            centerXY = new PointF((panelFunc.Right - panelFunc.Left) / 2 + panelFunc.Left, (panelFunc.Top - panelFunc.Bottom) / 2 + panelFunc.Bottom);
            scaler = 40;
            panelFunc.Invalidate();
        }

        private void ButtonFunc_Click(object sender, EventArgs e)
        {
            expr = new Function(textBoxFunc.Text);
            if (!expr.IsCorrect) labelFuncError.Visible = true;
            else labelFuncError.Visible = false;
            listBoxIterations.Items.Clear();
            SimplexMethod();
            panelFunc.Invalidate();
        }

        private void ButtonReturnSize_Click(object sender, EventArgs e)
        {
            scaler = 40;
            buttonReturnSize.Visible = buttonReturnSize.Enabled = false;
            panelFunc.Invalidate();
        }
        #endregion

        #region panelFunc
        private void PanelFunc_Paint(object sender, PaintEventArgs e)
        {
            if (panelFunc.BackgroundImage == null) e.Graphics.FillRectangle(backBrush, panelFunc.ClientRectangle);
            if (scaler != 40) buttonReturnSize.Visible = buttonReturnSize.Enabled = true;
            else buttonReturnSize.Visible = buttonReturnSize.Enabled = false;
            XYLinesInit(e.Graphics);
            DrawFunc(e.Graphics);
            DrawSimplex(e.Graphics);
        }

        private void PanelFunc_MouseWheel(object sender, MouseEventArgs e)
        {
            int MAX_SCALE = 300;
            int MIN_SCALE = 1;
            PointF worldPoint = new PointF
            {
                X = (e.Location.X - centerXY.X) / scaler,
                Y = (e.Location.Y - centerXY.Y) / scaler
            };

            if (e.Delta > 0 && scaler < MAX_SCALE)
            {
                scaler = Math.Min(scaler * 1.1f, MAX_SCALE);
            }

            else if (e.Delta < 0 && scaler > MIN_SCALE)
            {
                scaler = Math.Max(scaler / 1.1f, MIN_SCALE);
            }

            centerXY.X = e.Location.X - worldPoint.X * scaler;
            centerXY.Y = e.Location.Y - worldPoint.Y * scaler;
            panelFunc.Invalidate();
        }

        private void PanelFunc_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMovingScreen = true;
                lastXY = e.Location;
            }
        }

        private void PanelFunc_MouseUp(object sender, MouseEventArgs e)
        {
            isMovingScreen = false;
        }

        private void PanelFunc_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMovingScreen)
            {
                PointF currentXY = e.Location;
                PointF delta = new PointF
                {
                    X = currentXY.X - lastXY.X,
                    Y = currentXY.Y - lastXY.Y
                };
                lastXY = currentXY;
                centerXY.X += delta.X;
                centerXY.Y += delta.Y;
                panelFunc.Invalidate();
            }
        }

        private void PanelFunc_MouseClick(object sender, MouseEventArgs e)
        {
            PointF position = e.Location;
            panelFunc.Invalidate();
        }
        #endregion

        #region Other
        private void TextBoxFunc_TextChanged(object sender, EventArgs e)
        {
            if (textBoxFunc.Text == "")
            {
                labelFuncError.Visible = false;
                listBoxIterations.Items.Clear();
            }
        }

        private void SimplexMethod()
        {

            var settings = new Settings(1, 100);

            simplexes.Clear();

            double function(double[] point)
            {
                expr.TryCalculate(point[0], out var result);
                return result;
            }

            double[] x0 = [10];
            double[] x1 = [x0[0] + 2];

            var simplex = new Simplex(settings, [x0, x1], function);

            for (int r = 0; r < settings.MaxIterations; r++)
            {
                simplex.Iteration();
                simplexes.Add(simplex.ClonePoints());
                Console.WriteLine(simplex.GetBest);
                listBoxIterations.Items.Add($"Iter. {r + 1}: X = {simplex.GetBest[0]}, F[X] = {simplex.GetBest.Value}");
            }
        }
        #endregion
    }
}
