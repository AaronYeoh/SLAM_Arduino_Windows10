using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SlamTest
{
    public static class BotGrid
    {
       public static void DrawGrid(int rows, int cols, int rowHeight, int colWidth, Canvas mapCanvas)
        {
            int xSize = (int)mapCanvas.Width;
            int ySize = (int)mapCanvas.Height;

            //Lines that make up rows = numRows + 1
            for (int i = 0; i <= rows; i++)
            {
                //Generate grid by drawing many lines
                DrawLine(0, i * rowHeight, xSize, i * rowHeight, mapCanvas);
            }

            for (int i = 0; i <= cols; i++)
            {
                DrawLine(i * colWidth, 0, i * colWidth, ySize, mapCanvas);
            }

        }

        public static void DrawLine(int x1, int y1, int x2, int y2, Canvas mapCanvas)
        {
            Line lin = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetTop(lin, y1);
            Canvas.SetLeft(lin, x1);
            mapCanvas.Children.Add(lin);
        }
    }

}
