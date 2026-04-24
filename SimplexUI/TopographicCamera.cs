using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexUI
{
    public class TopographicCamera
    {
        public double Scale { get; set; } = 100;
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;

        public double MinScale { get; set; } = 20.0;
        public double MaxScale { get; set; } = 500.0;

        public TopographicCamera(double initialScale = 100.0)
        {
            Scale = initialScale;
            OffsetX = 0;
            OffsetY = 0;
        }

        public void Zoom(double factor, double cursorX, double cursorY)
        {
            double newScale = Scale * factor;
            newScale = Math.Max(MinScale, Math.Min(MaxScale, newScale));

            if(Math.Abs(newScale - Scale) < 0.001)
            {
                return;
            }

            double worldX = (cursorX - OffsetX) / Scale;
            double worldY = (OffsetY - cursorY) / Scale;

            Scale = newScale;
            OffsetX = cursorX - worldX * Scale;
            OffsetY = cursorY + worldX * Scale;
        }

        public void Pannoraming(double deltaX, double deltaY)
        {
            OffsetX += deltaX;
            OffsetY += deltaY;
        }

        public void ScreenToWorld(int screenX, int screenY, out double worldX, out double worldY)
        {
            worldX = (screenX - OffsetX) / Scale;
            worldY = (OffsetY - screenY) / Scale;
        }

        public void WorldToScreen(out int screenX, out int screenY, double worldX, double worldY)
        {
            screenX = (int)(worldX * Scale + OffsetX);
            screenY = (int)(OffsetY - worldY * Scale);
        }
    }
}
