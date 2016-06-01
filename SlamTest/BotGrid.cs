using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SlamTest
{
    public class BotGrid
    {
        private int _rows, _cols;
        private Canvas _mapCanvas;
        private int _rowHeight, _colWidth;

        public BotGrid(int rows, int cols, int rowHeight, int colWidth, Canvas mapCanvas)
        {
            _rows = rows;
            _cols = cols;
            _mapCanvas = mapCanvas;
            _rowHeight = rowHeight;
            _colWidth = colWidth;
        }

        public void DrawGrid()
        {
            int xSize = (int) _mapCanvas.Width;
            int ySize = (int) _mapCanvas.Height;

            //Lines that make up rows = numRows + 1
            for (int i = 0; i <= _rows; i++)
            {
                //Generate grid by drawing many lines
                DrawLine(0, i*_rowHeight, xSize, i*_rowHeight, _mapCanvas);
            }

            for (int i = 0; i <= _cols; i++)
            {
                DrawLine(i*_colWidth, 0, i*_colWidth, ySize, _mapCanvas);

            }
            for (int i = 0; i < _rows; i++)
            {
                DrawRectangle(xPos:0, yPos:i * _rowHeight * 2, size:_rowHeight*2, color:Colors.Goldenrod);
            }
            for (int i = 0; i < _cols; i++)
            {
                var xPosition = i*_colWidth*2;
                var yPosition = 4*_rowHeight;
                DrawRectangle(xPos: xPosition, yPos: yPosition, size: _colWidth * 2, color:Colors.Chartreuse);
            }
            CreateCells();
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

        public void CreateCells()
        {
            Cell[,] cells = new Cell[_rows,_cols];
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    cells[i, j] = new Cell(CellStatus.Unknown, j*_colWidth*2, i * _rowHeight * 2,_colWidth *2);
                }
            }
            DrawCells(cells);

        }
        public void DrawCells(Cell[,] cells)
        {
            foreach (var cell in cells)
            {
                switch (cell.status)
                {
                    case CellStatus.Obstacle:
                        DrawRectangle(cell.cellX, cell.cellY,cell.cellSize, Colors.Navy);
                        break;
                    case CellStatus.Unknown:
                        DrawRectangle(cell.cellX, cell.cellY, cell.cellSize, Colors.Lavender);
                        break;
                }
            }

        }

        public void DrawRectangle(int xPos, int yPos, int size, Color color)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(color);
            rect.Stroke = new SolidColorBrush(Colors.Brown);
            rect.Width = size-4;
            rect.Height = size-4;
            rect.Margin = new Thickness(2);
            rect.Stretch = Stretch.UniformToFill;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(rect, xPos);
            Canvas.SetTop(rect, yPos);
            _mapCanvas.Children.Add(rect);
        }
    }

    
}
