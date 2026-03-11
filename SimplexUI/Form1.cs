using Nelder_Mead_method;
using System.Linq.Expressions;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        myFunc expr;


        public Form1()
        {
            InitializeComponent();
            setStartParametres();
            funcColor = Color.Red;
            panelFunc.MouseWheel += PanelFunc_MouseWheel;
            backBrush = new SolidBrush(Color.White);
            expr = new myFunc(textBoxFunc.Text);

        }

        #region XY methods
        private void setStartParametres()
        {
            centerXY = new PointF((panelFunc.Right - panelFunc.Left) / 2, (panelFunc.Bottom - panelFunc.Top) / 2);
            scaler = 40;
            labelFuncError.Visible = false;
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
                setStartParametres();
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
            if (expr != null && expr.isCorrect)
            {
                for (float i = 0; i < panelFunc.Width; i++)
                {
                    pnt.X = i;
                    pnt.Y = 0;
                    pnt = PanelToFuncXY(pnt);
                    pnt.Y = (float)expr.Calculate(pnt.X);
                    funcPointList.Add(FuncToPanelXY(pnt));
                }
                for (int i = 1; i < funcPointList.Count(); i++)
                {
                    /*if (funcPointList[i - 1].Y <= funcPointList[i].Y && function is tg)
                    {
                        PointF tgPoint = new PointF(funcPointList[i - 1].X, 0);
                        g.DrawLine(p, funcPointList[i - 1], tgPoint);
                        tgPoint = new PointF(funcPointList[i].X, panelFunc.Height);
                        g.DrawLine(p, tgPoint, funcPointList[i]);
                        continue;
                    }*/
                    if (Math.Abs(funcPointList[i - 1].Y - funcPointList[i].Y) < panelFunc.Height) g.DrawLine(p, funcPointList[i - 1], funcPointList[i]);
                }
            }
        }
        #endregion

        #region Buttons

        private void buttonColorFunc_Click(object sender, EventArgs e)
        {
            ColorDialog colorChanger = new ColorDialog();
            colorChanger.FullOpen = true;
            colorChanger.Color = funcColor;
            if (colorChanger.ShowDialog() == DialogResult.Cancel) return;
            funcColor = colorChanger.Color;
            panelFunc.Invalidate();
        }

        private void buttonStartScale_Click(object sender, EventArgs e)
        {
            centerXY = new PointF((panelFunc.Right - panelFunc.Left) / 2 + panelFunc.Left, (panelFunc.Top - panelFunc.Bottom) / 2 + panelFunc.Bottom);
            scaler = 40;
            panelFunc.Invalidate();
        }

        private void buttonFunc_Click(object sender, EventArgs e)
        {
            expr = new myFunc(textBoxFunc.Text);
            if (!expr.isCorrect) labelFuncError.Visible = true;
            else labelFuncError.Visible = false;
            panelFunc.Invalidate();
        }
        #endregion

        #region panelFunc
        private void PanelFunc_Paint(object sender, PaintEventArgs e)
        {
            if (panelFunc.BackgroundImage == null) e.Graphics.FillRectangle(backBrush, panelFunc.ClientRectangle);
            XYLinesInit(e.Graphics);
            DrawFunc(e.Graphics);
        }

        private void PanelFunc_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && scaler < 300)
                scaler *= 1.1f;
            else if (e.Delta < 0 && scaler > 5)
                scaler /= 1.1f;
            panelFunc.Invalidate();
            //if (scaler != 40) label1.Text = ((float)scaler / 40).ToString();
        }

        private void panelFunc_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMovingScreen = true;
                lastXY = e.Location;
            }
        }

        private void panelFunc_MouseUp(object sender, MouseEventArgs e)
        {
            isMovingScreen = false;
        }

        private void panelFunc_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMovingScreen)
            {
                PointF currentXY = e.Location;
                PointF delta = new PointF();
                delta.X = currentXY.X - lastXY.X;
                delta.Y = currentXY.Y - lastXY.Y;
                lastXY = currentXY;
                centerXY.X += delta.X;
                centerXY.Y += delta.Y;
                panelFunc.Invalidate();
            }
        }

        private void panelFunc_MouseClick(object sender, MouseEventArgs e)
        {
            PointF position = e.Location;
            panelFunc.Invalidate();
        }
        #endregion

        #region Other
        private void textBoxFunc_TextChanged(object sender, EventArgs e)
        {
            if (textBoxFunc.Text == "") labelFuncError.Visible = false;
        }
        #endregion
    }
}
