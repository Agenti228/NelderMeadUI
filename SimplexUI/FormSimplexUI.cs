using SimplexUI.Layers;
using SimplexUI.SimplexIterationStrategies;

namespace SimplexUI
{
    public partial class FormSimplexUI : Form
    {
        private const int MAX_SCALE = 5000000;
        private const int MIN_SCALE = 1;
        private const double MIN_TOPOGRAPHIC_SCALE = 20.0;
        private const double MAX_TOPOGRAPHIC_SCALE = 500.0;

        private PointF _lastMouseLocation = default;
        private bool _movingScreen = false;
        private Function _function = new(string.Empty);

        private readonly Dictionary<LayerNames, Layer> _drawableLayers;
        private readonly ISimplexIterationStrategy _strategy;

        private TopographicCamera _camera;
        private TopographicRenderer _renderer;
        private bool isPanning = false;
        private Point lastMousePosition;



        public FormSimplexUI()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _drawableLayers = new Dictionary<LayerNames, Layer>
            {
                [LayerNames.Axis] = new AxisLayer((Width, Height)),
                [LayerNames.Graph] = new GraphLayer((Width, Height), _function),
                [LayerNames.Simplex] = new SimplexLayer((Width, Height), _function),
            };
            ActivatePictureBox();
            _strategy = new MaxIterationsStrategy([10], _function.Calculate);
        }

        private void TextBoxFunction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                UpdateForm();
            }
        }

        private void UpdateForm()
        {
            UpdateFunction();

            UpdateUI();

            UpdateSimplexes();

            panelFunction.Invalidate();
            
            if (_function.GetVariablesCount == 2)
            {
                ActivatePictureBox();
                pictureBox.Visible = true;
                pictureBox.Enabled = true;
                statusStrip.Visible = true;
                statusStrip.Enabled = true;
                pictureBox.Invalidate();
            }
            else
            {
                pictureBox.Visible = false;
                pictureBox.Enabled = false;
                statusStrip.Visible = false;
                statusStrip.Enabled = false;
                pictureBox.Invalidate();
            }
        }

        public void UpdateFunction()
        {
            ((GraphLayer)_drawableLayers[LayerNames.Graph]).Function = new Function(textBoxFunction.Text); // maybe do smth with it
            _function = new Function(textBoxFunction.Text);
        }

        public void UpdateUI()
        {
            if (_function.IsCorrect)
            {
                listBoxIterations.Items.Clear();
                labelFunctionError.Visible = false;
            }
            else
            {
                if (textBoxFunction.Text == string.Empty)
                {
                    //pnt.X = i;
                    //pnt.Y = 0;
                    //PointF pointF = ConvertPanelToFunction(pnt);
                    //pnt = pointF;
                    //double[] multiDimentionalPoint = new double[Math.Max(_function.GetVariablesCount, 1)];
                    //Array.Fill(multiDimentionalPoint, 0);
                    //multiDimentionalPoint[0] = pnt.X;
                    //double result = _function.Calculate(multiDimentionalPoint);
                    //pnt.Y = (float)result;
                    //funcPointList.Add(ConvertFunctionToPanel(pnt));
                    listBoxIterations.Items.Clear();
                    labelFunctionError.Visible = false;
                }

                labelFunctionError.Visible = true;
            }

        }

        public void UpdateSimplexes()
        {
            if (!_function.IsCorrect)
            {
                return;
            }

            List<Simplex> simplexes = [.. _strategy.Iterate()];
            ((SimplexLayer)_drawableLayers[LayerNames.Simplex]).Simplexes.Clear();

            for (int i = 0; i < simplexes.Count; i++)
            {
                Simplex simplex = simplexes[i];
                simplex.SortVectors();
                ((SimplexLayer)_drawableLayers[LayerNames.Simplex]).Simplexes.Add(simplex.Vectors);
                _ = listBoxIterations.Items.Add($"Iteration {i + 1}: {simplex.GetBestInSorted}");
            }
        }

        #region Panel
        private void PanelFunction_Paint(object sender, PaintEventArgs e)
        {
            foreach (KeyValuePair<LayerNames, Layer> layer in _drawableLayers)
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

            if (e.Delta > 0 && Layer.Scale < MAX_SCALE)
            {
                Layer.Scale = Math.Min(Layer.Scale * 1.1f, MAX_SCALE);
            }
            else if (e.Delta < 0 && Layer.Scale > MIN_SCALE)
            {
                Layer.Scale = Math.Max(Layer.Scale / 1.1f, MIN_SCALE);
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
            if (!_movingScreen)
            {
                return;
            }

            var delta = new PointF
            {
                X = e.Location.X - _lastMouseLocation.X,
                Y = e.Location.Y - _lastMouseLocation.Y
            };

            _lastMouseLocation = e.Location;

            Layer.Center = new PointF(Layer.Center.X + delta.X, Layer.Center.Y + delta.Y);

            panelFunction.Invalidate();
        }

        private void FormSimplexUI_Resize(object sender, EventArgs e)
        {
            foreach (var layer in _drawableLayers)
            {
                layer.Value.PanelDimentions = (Width, Height);
            }

            panelFunction.Invalidate();
            pictureBox.Invalidate();
        }

        private void ButtonResetView_Click(object sender, EventArgs e)
        {
            //double[] startPoint = new double[Math.Max(_function.GetVariablesNumber(), 1)];
            //Array.Fill(startPoint, 10);

            Layer.ResetView();
            panelFunction.Invalidate();
        }
        #endregion

        #region pictureBox
        private void ActivatePictureBox()
        {
            _camera = new TopographicCamera();
            _renderer = new TopographicRenderer(_function,_camera,(pictureBox.Width, pictureBox.Height));
            pictureBox.MouseWheel += pictureBox_MouseWheel;
        }
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            _renderer.PictureBoxDimentions = (pictureBox.ClientSize.Width, pictureBox.ClientSize.Height);
            using (Bitmap bmp = _renderer.Render())
            {
                if(bmp.Width > 0 && bmp.Height > 0)
                {
                    e.Graphics.DrawImage(bmp, 0, 0);
                }
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                isPanning = true;
                lastMousePosition = e.Location;
                pictureBox.Cursor = Cursors.SizeAll;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            _camera.ScreenToWorld(e.X, e.Y, out double worldX, out double worldY);

            double z = _function.Calculate([worldX, worldY]);

            statusStrip.Text = "x: {worldX:F3}  y: {worldY:F3}  z: {z:F3}";

            if(isPanning)
            {
                double delta_x = e.X - lastMousePosition.X;
                double delta_y = e.Y - lastMousePosition.Y;
                _camera.OffsetX += delta_x;
                _camera.OffsetY += delta_y;
                lastMousePosition = e.Location;
                pictureBox.Invalidate();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                isPanning = false;
                pictureBox.Cursor = Cursors.Default;
            }
        }

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            double factor = e.Delta > 0 ? 1.1 : 0.9;
            double newScale = _camera.Scale * factor;
            newScale = Math.Max(MIN_TOPOGRAPHIC_SCALE, Math.Min(MAX_TOPOGRAPHIC_SCALE, newScale));

            if(Math.Abs(newScale - _camera.Scale) < 0.001)
            {
                return;
            }

            double worldX = (e.X - _camera.OffsetX) / _camera.Scale;
            double worldY = (_camera.OffsetY - e.Y) / _camera.Scale;

            _camera.Scale = newScale;
            _camera.OffsetX = e.X - worldX * _camera.Scale;
            _camera.OffsetY = e.Y - worldY + _camera.Scale;

            pictureBox.Invalidate();
        }
        #endregion
    }
}
