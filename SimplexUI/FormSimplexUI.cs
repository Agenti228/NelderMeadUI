namespace SimplexUI
{
    /// <summary>
    /// TODO:
    /// Сделать приоритет у рендера для координат по X, а не по Y, чтобы очень сложенные графики рендерились нормально при прибижении
    /// Добавить отрисовку координат по осям
    /// </summary>
    public partial class FormSimplexUI : Form
    {
        private float _scale;
        private readonly Color _functionColor;
        private readonly Brush _backBrush;
        private PointF _center;
        private PointF _lastMouseLocation;
        private bool _movingScreen = false;
        private readonly List<EvaluateableVector[]> _simplexes = [];
        private Function _function;
        private const int _maxScale = 5000000;
        private const int _minScale = 1;

        public FormSimplexUI()
        {
            InitializeComponent();
            SetStartParametres();
            _functionColor = Color.Red;
            panelFunction.MouseWheel += PanelFunction_MouseWheel;
            _backBrush = new SolidBrush(Color.White);
            _function = new Function(textBoxFunction.Text);
        }

        #region XY methods
        private void SetStartParametres()
        {
            _center = new PointF((panelFunction.Right - panelFunction.Left) / 2, (panelFunction.Bottom - panelFunction.Top) / 2);
            _scale = 40;
            buttonReturnSize.Visible = false;
            buttonReturnSize.Enabled = false;
            labelFunctionError.Visible = false;
            listBoxIterations.Items.Clear();
        }

        private void DrawCoordinateAxes(Graphics g)
        {
            try
            {
                float[] dash = [_scale / 8, _scale / 8];
                using var p = new Pen(Color.Black);
                p.Width = 2;
                g.DrawLine(p, _center.X, 0, _center.X, panelFunction.ClientSize.Height);
                g.DrawLine(p, 0, _center.Y, panelFunction.ClientSize.Width, _center.Y);
            }
            catch
            {
                _ = MessageBox.Show("Out of memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStartParametres();
                panelFunction.Invalidate();
            }
        }

        private PointF ConvertPanelToFunction(PointF pnt)
        {
            if (pnt.Y > _center.Y)
            {
                pnt.Y = -Math.Abs(pnt.Y - _center.Y) / _scale;
            }
            else
            {
                pnt.Y = Math.Abs(_center.Y - pnt.Y) / _scale;
            }

            if (pnt.X > _center.X)
            {
                pnt.X = Math.Abs(pnt.X - _center.X) / _scale;
            }
            else
            {
                pnt.X = -Math.Abs(_center.X - pnt.X) / _scale;
            }

            return pnt;
        }

        private PointF ConvertFunctionToPanel(PointF pnt)
        {
            pnt.X = (_scale * pnt.X) + _center.X;
            pnt.Y = (-_scale * pnt.Y) + _center.Y;
            return pnt;
        }
        #endregion

        #region Draw methods
        private void DrawFunction(Graphics g)
        {
            var funcPointList = new List<PointF>();
            PointF pnt = new System.Drawing.Point();
            var p = new Pen(_functionColor)
            {
                Width = 2
            };
            if (_function != null && _function.IsCorrect)
            {
                for (float i = 0; i < panelFunction.Width; i++)
                {
                    pnt.X = i;
                    pnt.Y = 0;
                    PointF pointF = ConvertPanelToFunction(pnt);
                    pnt = pointF;
                    _function.TryCalculate(pnt.X, out double result);
                    pnt.Y = (float)result;
                    funcPointList.Add(ConvertFunctionToPanel(pnt));
                }

                for (int i = 1; i < funcPointList.Count; i++)
                {
                    if (Math.Abs(funcPointList[i - 1].Y - funcPointList[i].Y) < panelFunction.Height)
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
            if (_function != null && _function.IsCorrect)
            {
                for (int i = 0; i < _simplexes.Count; i++)
                {
                    for (int j = 1; j < _simplexes[i].Length; j++)
                    {
                        PointF frstPnt = ConvertFunctionToPanel(new PointF((float)_simplexes[i][j - 1][0], (float)_simplexes[i][j - 1].Value));
                        PointF scndPnt = ConvertFunctionToPanel(new PointF((float)_simplexes[i][j][0], (float)_simplexes[i][j].Value));
                        if (frstPnt.X < 0 || scndPnt.X < 0 || frstPnt.X > panelFunction.Width || scndPnt.X > panelFunction.Width)
                        {
                            continue;
                        }

                        if (frstPnt.Y < 0 || scndPnt.Y < 0 || frstPnt.Y > panelFunction.Height || scndPnt.Y > panelFunction.Height)
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
            _center = new PointF((panelFunction.Right - panelFunction.Left) / 2 + panelFunction.Left, (panelFunction.Top - panelFunction.Bottom) / 2 + panelFunction.Bottom);
            _scale = 40;
            panelFunction.Invalidate();
        }

        private void ButtonFunction_Click(object sender, EventArgs e)
        {
            _function = new Function(textBoxFunction.Text);
            labelFunctionError.Visible = !_function.IsCorrect;
            
            listBoxIterations.Items.Clear();
            SimplexMethod();
            panelFunction.Invalidate();
        }

        private void ButtonReturnSize_Click(object sender, EventArgs e)
        {
            _scale = 40;
            buttonReturnSize.Visible = false;
            buttonReturnSize.Enabled = false;
            panelFunction.Invalidate();
        }
        #endregion

        #region panelFunc
        private void PanelFunction_Paint(object sender, PaintEventArgs e)
        {
            if (panelFunction.BackgroundImage == null)
            {
                e.Graphics.FillRectangle(_backBrush, panelFunction.ClientRectangle);
            }

            if (_scale != 40)
            {
                buttonReturnSize.Visible = buttonReturnSize.Enabled = true;
            }
            else
            {
                buttonReturnSize.Visible = buttonReturnSize.Enabled = false;
            }

            DrawCoordinateAxes(e.Graphics);
            DrawFunction(e.Graphics);
            DrawSimplex(e.Graphics);
        }

        private void PanelFunction_MouseWheel(object? sender, MouseEventArgs? e)
        {
            if (e is null)
            {
                return;
            }

            var worldPoint = new PointF()
            {
                X = (e.Location.X - _center.X) / _scale,
                Y = (e.Location.Y - _center.Y) / _scale
            };

            if (e.Delta > 0 && _scale < _maxScale)
            {
                _scale = Math.Min(_scale * 1.1f, _maxScale);
            }
            else if (e.Delta < 0 && _scale > _minScale)
            {
                _scale = Math.Max(_scale / 1.1f, _minScale);
            }

            _center.X = e.Location.X - (worldPoint.X * _scale);
            _center.Y = e.Location.Y - (worldPoint.Y * _scale);
            panelFunction.Invalidate();
        }

        private void PanelFunction_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _movingScreen = true;
                _lastMouseLocation = e.Location;
            }
        }

        private void PanelFunction_MouseUp(object sender, MouseEventArgs e)
        {
            _movingScreen = false;
        }

        private void PanelFunction_MouseMove(object sender, MouseEventArgs e)
        {
            if (_movingScreen)
            {
                var delta = new PointF
                {
                    X = e.Location.X - _lastMouseLocation.X,
                    Y = e.Location.Y - _lastMouseLocation.Y
                };

                _lastMouseLocation = e.Location;
                _center.X += delta.X;
                _center.Y += delta.Y;
                panelFunction.Invalidate();
            }
        }

        private void PanelFunction_MouseClick(object sender, MouseEventArgs e)
        {
            panelFunction.Invalidate();
        }
        #endregion

        #region Other
        private void TextBoxFunction_TextChanged(object sender, EventArgs e)
        {
            if (textBoxFunction.Text == string.Empty)
            {
                labelFunctionError.Visible = false;
                listBoxIterations.Items.Clear();
            }
        }

        private void SimplexMethod()
        {
            _simplexes.Clear();

            var initialVector = new EvaluateableVector([10], OneDimentionalFunction);
            double edgeLength = 2;
            var initialConditions = new InitialConditions(initialVector, edgeLength);

            int maxIterations = 100;
            var settings = new Settings(maxIterations);

            var simplex = new Simplex(settings, initialConditions);

            for (int i = 0; i < settings.MaxIterations; i++)
            {
                _simplexes.Add(simplex.ClonePoints());
                AddBestVectorOnIteration(i); //do something with double sorting

                simplex.Iteration(); //also sorts _points

            }

            void AddBestVectorOnIteration(int iteration)
            {
                simplex.SortPoints();
                _ = listBoxIterations.Items.Add($"Iteration {iteration + 1}: {simplex.GetBestInSorted}");
            }

            double OneDimentionalFunction(double[] point)
            {
                _ = _function.TryCalculate(point[0], out double result);
                return result;
            }
        }
        #endregion
    }
}
