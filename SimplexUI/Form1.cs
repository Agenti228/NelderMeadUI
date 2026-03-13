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
        private float _scaler;
        //стиль
        //стиль
        private Color _funcColor;
        private readonly Brush _backBrush;
        //точки
        private PointF _centerXY;
        private PointF _lastXY; //для определения дельты перемещения графика
        //флаги
        private bool _isMovingScreen = false;

        private readonly List<Point[]> _simplexes = [];

        private Function _expr;


        public Form1()
        {
            InitializeComponent();
            SetStartParametres();
            _funcColor = Color.Red;
            panelFunc.MouseWheel += PanelFunc_MouseWheel;
            _backBrush = new SolidBrush(Color.White);
            _expr = new Function(textBoxFunc.Text);
        }

        #region XY methods
        private void SetStartParametres()
        {
            _centerXY = new PointF((panelFunc.Right - panelFunc.Left) / 2, (panelFunc.Bottom - panelFunc.Top) / 2);
            _scaler = 40;
            buttonReturnSize.Visible = buttonReturnSize.Enabled = labelFuncError.Visible = false;
            listBoxIterations.Items.Clear();
        }

        private void XYLinesInit(Graphics g)
        {
            try
            {
                float[] dash = { _scaler / 8, _scaler / 8 };
                using var p = new Pen(Color.Black);
                p.Width = 2;
                g.DrawLine(p, _centerXY.X, 0, _centerXY.X, panelFunc.ClientSize.Height);
                g.DrawLine(p, 0, _centerXY.Y, panelFunc.ClientSize.Width, _centerXY.Y);
                p.DashPattern = dash;
                g.DrawEllipse(p, _centerXY.X - _scaler, _centerXY.Y - _scaler, _scaler * 2, _scaler * 2);
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
            if (pnt.Y > _centerXY.Y)
            {
                pnt.Y = -Math.Abs(pnt.Y - _centerXY.Y) / _scaler;
            }
            else
            {
                pnt.Y = Math.Abs(_centerXY.Y - pnt.Y) / _scaler;
            }

            if (pnt.X > _centerXY.X)
            {
                pnt.X = Math.Abs(pnt.X - _centerXY.X) / _scaler;
            }
            else
            {
                pnt.X = -Math.Abs(_centerXY.X - pnt.X) / _scaler;
            }

            return pnt;
        }

        private PointF FuncToPanelXY(PointF pnt)
        {
            pnt.X = (_scaler * pnt.X) + _centerXY.X;
            pnt.Y = (-_scaler * pnt.Y) + _centerXY.Y;
            return pnt;
        }
        #endregion

        #region Draw methods
        private void DrawFunc(Graphics g)
        {
            var funcPointList = new List<PointF>();
            PointF pnt = new System.Drawing.Point();
            var p = new Pen(_funcColor)
            {
                Width = 2
            };
            if (_expr != null && _expr.IsCorrect)
            {
                for (float i = 0; i < panelFunc.Width; i++)
                {
                    pnt.X = i;
                    pnt.Y = 0;
                    PointF pointF = PanelToFuncXY(pnt);
                    pnt = pointF;
                    _expr.TryCalculate(pnt.X, out double result);
                    pnt.Y = (float)result;
                    funcPointList.Add(FuncToPanelXY(pnt));
                }

                for (int i = 1; i < funcPointList.Count; i++)
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
            using var p = new Pen(Color.DeepPink);
            p.Width = 2;
            if (_expr != null && _expr.IsCorrect)
            {
                for (int i = 0; i < _simplexes.Count; i++)
                {
                    for (int j = 1; j < _simplexes[i].Length; j++)
                    {
                        PointF frstPnt = FuncToPanelXY(new PointF((float)_simplexes[i][j - 1][0], (float)_simplexes[i][j - 1].Value));
                        PointF scndPnt = FuncToPanelXY(new PointF((float)_simplexes[i][j][0], (float)_simplexes[i][j].Value));
                        if (double.IsNegativeInfinity(frstPnt.Y) || double.IsInfinity(frstPnt.Y) || double.IsNegativeInfinity(scndPnt.Y) || double.IsInfinity(scndPnt.Y)) 
                        { 
                            continue; 
                        }

                        g.DrawLine(p, frstPnt, scndPnt);
                        g.DrawRectangle(Pens.Blue, frstPnt.X - 2, frstPnt.Y - 2, 4, 4);
                        g.DrawRectangle(Pens.Blue, scndPnt.X - 2, scndPnt.Y - 2, 4, 4);
                    }
                }
            }
        }
        #endregion

        #region Buttons

        private void ButtonStartScale_Click(object sender, EventArgs e)
        {
            _centerXY = new PointF(((panelFunc.Right - panelFunc.Left) / 2) + panelFunc.Left, ((panelFunc.Top - panelFunc.Bottom) / 2) + panelFunc.Bottom);
            _scaler = 40;
            panelFunc.Invalidate();
        }

        private void ButtonFunc_Click(object sender, EventArgs e)
        {
            _expr = new Function(textBoxFunc.Text);
            if (!_expr.IsCorrect)
            {
                labelFuncError.Visible = true;
            }
            else
            {
                labelFuncError.Visible = false;
            }

            listBoxIterations.Items.Clear();
            SimplexMethod();
            panelFunc.Invalidate();
        }

        private void ButtonReturnSize_Click(object sender, EventArgs e)
        {
            _scaler = 40;
            buttonReturnSize.Visible = buttonReturnSize.Enabled = false;
            panelFunc.Invalidate();
        }
        #endregion

        #region panelFunc
        private void PanelFunc_Paint(object sender, PaintEventArgs e)
        {
            if (panelFunc.BackgroundImage == null)
            {
                e.Graphics.FillRectangle(_backBrush, panelFunc.ClientRectangle);
            }

            if (_scaler != 40)
            {
                buttonReturnSize.Visible = buttonReturnSize.Enabled = true;
            }
            else
            {
                buttonReturnSize.Visible = buttonReturnSize.Enabled = false;
            }

            XYLinesInit(e.Graphics);
            DrawFunc(e.Graphics);
            DrawSimplex(e.Graphics);
        }

        private void PanelFunc_MouseWheel(object? sender, MouseEventArgs? e)
        {
            if (e is null)
            {
                return;
            }

            int MAX_SCALE = 300;
            int MIN_SCALE = 1;
            var worldPoint = new PointF
            {
                X = (e.Location.X - _centerXY.X) / _scaler,
                Y = (e.Location.Y - _centerXY.Y) / _scaler
            };

            if (e.Delta > 0 && _scaler < MAX_SCALE)
            {
                _scaler = Math.Min(_scaler * 1.1f, MAX_SCALE);
            }

            else if (e.Delta < 0 && _scaler > MIN_SCALE)
            {
                _scaler = Math.Max(_scaler / 1.1f, MIN_SCALE);
            }

            _centerXY.X = e.Location.X - (worldPoint.X * _scaler);
            _centerXY.Y = e.Location.Y - (worldPoint.Y * _scaler);
            panelFunc.Invalidate();
        }

        private void PanelFunc_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMovingScreen = true;
                _lastXY = e.Location;
            }
        }

        private void PanelFunc_MouseUp(object sender, MouseEventArgs e)
        {
            _isMovingScreen = false;
        }

        private void PanelFunc_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMovingScreen)
            {
                PointF currentXY = e.Location;
                var delta = new PointF
                {
                    X = currentXY.X - _lastXY.X,
                    Y = currentXY.Y - _lastXY.Y
                };
                _lastXY = currentXY;
                _centerXY.X += delta.X;
                _centerXY.Y += delta.Y;
                panelFunc.Invalidate();
            }
        }

        private void PanelFunc_MouseClick(object sender, MouseEventArgs e)
        {
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

            _simplexes.Clear();

            double function(double[] point)
            {
                _ = _expr.TryCalculate(point[0], out double result);
                return result;
            }

            double[] x0 = [10];
            double[] x1 = [x0[0] + 2];

            var simplex = new Simplex(settings, 1, [10], 2, function);

            for (int r = 0; r < settings.MaxIterations; r++)
            {
                simplex.Iteration();
                _simplexes.Add(simplex.ClonePoints());
                Console.WriteLine(simplex.GetBest);
                _ = listBoxIterations.Items.Add($"Iter. {r + 1}: X = {simplex.GetBest[0]}, F[X] = {simplex.GetBest.Value}");
            }
        }
        #endregion
    }
}
