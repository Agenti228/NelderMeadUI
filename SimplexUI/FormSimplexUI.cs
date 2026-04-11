namespace SimplexUI
{
    public partial class FormSimplexUI : Form
    {
        private PointF _lastMouseLocation;
        private bool _movingScreen = false;
        private readonly List<EvaluateableVector[]> _simplexes = [];
        private Function _function = new(string.Empty);
        private const int _maxScale = 5000000;
        private const int _minScale = 1;

        private readonly Dictionary<Layers, Layer> _drawableLayers;

        public FormSimplexUI()
        {
            InitializeComponent();

            _drawableLayers = new Dictionary<Layers, Layer>
            {
                [Layers.Axis] = new AxisLayer((Width, Height)),
                [Layers.Graph] = new GraphLayer((Width, Height), _function),
                [Layers.Simplex] = new SimplexLayer((Width, Height), _function),
            };
        }

        private void TextBoxFunction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            ((GraphLayer)_drawableLayers[Layers.Graph]).Function = new Function(textBoxFunction.Text); // maybe do smth with it
            _function = new Function(textBoxFunction.Text);
            labelFunctionError.Visible = !_function.IsCorrect;

            listBoxIterations.Items.Clear();
            panelFunction.Invalidate();

            UpdateSimplexes();
        }

        #region Buttons
        private void ButtonReturnSize_Click(object sender, EventArgs e)
        {
            Layer.Scale = 40;

            panelFunction.Invalidate();
        }
        #endregion

        #region Panel
        private void PanelFunction_Paint(object sender, PaintEventArgs e)
        {
            foreach (KeyValuePair<Layers, Layer> layer in _drawableLayers)
            {
                layer.Value.Draw(e.Graphics);
            }
        }

        private void PanelFunction_MouseWheel(object? sender, MouseEventArgs? e)
        {
            if (e is null)
            {
                return;
            }

            var worldPoint = new PointF()
            {
                X = (e.Location.X - Layer.Center.X) / Layer.Scale,
                Y = (e.Location.Y - Layer.Center.Y) / Layer.Scale
            };

            if (e.Delta > 0 && Layer.Scale < _maxScale)
            {
                Layer.Scale = Math.Min(Layer.Scale * 1.1f, _maxScale);
            }
            else if (e.Delta < 0 && Layer.Scale > _minScale)
            {
                Layer.Scale = Math.Max(Layer.Scale / 1.1f, _minScale);
            }

            Layer.Center = new PointF(e.Location.X - (worldPoint.X * Layer.Scale), e.Location.Y - (worldPoint.Y * Layer.Scale));

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

                Layer.Center = new PointF(Layer.Center.X + delta.X, Layer.Center.Y + delta.Y);
                Layer.Offset = delta;
                panelFunction.Invalidate();
            }
        }

        private void PanelFunction_MouseClick(object sender, MouseEventArgs e)
        {
            panelFunction.Invalidate();
        }

        private void FormSimplexUI_Resize(object sender, EventArgs e)
        {
            foreach (var layer in _drawableLayers)
            {
                layer.Value.PanelDimentions = (Width, Height);
            }
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

        public void UpdateSimplexes()
        {
            if (!_function.IsCorrect)
            {
                return;
            }

            List<Simplex> simplexes = [.. IterateSimplexMethod()];
            ((SimplexLayer)_drawableLayers[Layers.Simplex]).Simplexes.Clear();

            for (int i = 0; i < simplexes.Count; i++)
            {
                Simplex simplex = simplexes[i];
                simplex.SortVectors();
                ((SimplexLayer)_drawableLayers[Layers.Simplex]).Simplexes.Add(simplex.Vectors);
                _ = listBoxIterations.Items.Add($"Iteration {i + 1}: {simplex.GetBestInSorted}");
            }
        }

        public Simplex GetInitialSimlpex()
        {
            _simplexes.Clear();

            var initialVector = new EvaluateableVector([10], OneDimentionalFunction);
            double edgeLength = 2;
            var initialConditions = new InitialConditions(initialVector, edgeLength);

            int maxIterations = 100;
            var settings = new Settings(maxIterations);

            return new Simplex(settings, initialConditions);

            double OneDimentionalFunction(double[] point)
            {
                _ = _function.TryCalculate(point[0], out double result);
                return result;
            }
        }

        public IEnumerable<Simplex> IterateSimplexMethod()
        {
            var simplex = GetInitialSimlpex();

            for (int i = 0; i < simplex.Settings.MaxIterations; i++)
            {
                simplex.SortVectors();
                simplex.IterationOnSorted();

                yield return (Simplex)simplex.Clone();
            }
        }
        #endregion
    }
}
