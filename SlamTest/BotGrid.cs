using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SlamTest
{
    public class BotGrid
    {
        private int _rows, _cols;
        private Canvas _mapCanvas;
        private int _rowHeight, _colWidth;
        private int _scale;
        private Cell[,] cells;
        private static BotGrid _instance = null;
        public static BotGrid Instance { get { return _instance; } }
        public BotGrid(int rows, int cols, int rowHeight, int colWidth, Canvas mapCanvas, int scale)
        {
            _rows = rows;
            _cols = cols;
            _mapCanvas = mapCanvas;
            _rowHeight = rowHeight;
            _colWidth = colWidth;
            _scale = scale;
            _instance = this;
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
            //for (int i = 0; i < _rows; i++)
            //{
            //    DrawRectangle(xPos:0, yPos:i * _rowHeight * 2, size:_rowHeight*2, color:Colors.Goldenrod);
            //}
            //for (int i = 0; i < _cols; i++)
            //{
            //    var xPosition = i*_colWidth*2;
            //    var yPosition = 4*_rowHeight;
            //    DrawRectangle(xPos: xPosition, yPos: yPosition, size: _colWidth * 2, color:Colors.Chartreuse);
            //}
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
            cells = new Cell[_rows,_cols];
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    cells[i, j] = new Cell(CellStatus.Unknown, j*_colWidth*2, i * _rowHeight * 2, i, j ,_colWidth *2);
                }
            }
            var cel = cells[0, 0];
            cel.status = CellStatus.Obstacle;
            DrawCells();

        }
        public void DrawCells()
        {
            foreach (var cell in cells)
            {
                DrawSingleCell(cell);
            }

        }

        public void DrawSingleCell(Cell cell)
        {
            switch (cell.status)
            {
                case CellStatus.Obstacle:
                    DrawRectangle(cell.cellX, cell.cellY, cell.cellSize, Colors.Navy, cell);
                    break;
                case CellStatus.Unknown:
                    DrawRectangle(cell.cellX, cell.cellY, cell.cellSize, Colors.DarkGray, cell);
                    break;
                case CellStatus.Clear:
                    DrawRectangle(cell.cellX, cell.cellY, cell.cellSize, Colors.Lavender, cell);
                    break;
                case CellStatus.Covered:
                    DrawRectangle(cell.cellX, cell.cellY, cell.cellSize, Colors.Black, cell);
                    break;
            }
        }

        public bool SetCell(int x, int y)
        {
            cells[x,y].status = CellStatus.Clear;
            return true;
        }

        public void DrawRectangle(int xPos, int yPos, int size, Color color, Cell cell)
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
            rect.Tag = new int[2] {cell.cellPosX, cell.cellPosY};
            Canvas.SetLeft(rect, xPos);
            Canvas.SetTop(rect, yPos);
            rect.Tapped += RectOnTapped;
            _mapCanvas.Children.Add(rect);
        }

        private void RectOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            var rect = sender as Rectangle;
            var tag = rect?.Tag as int[];

            Bresenham.Line(0,0,tag[0],tag[1], new Bresenham.PlotFunction(SetCell));
            cells[tag[0],tag[1]].status = CellStatus.Obstacle;
            //rect.Fill = new SolidColorBrush(Colors.Navy);
            //Canvas.SetZIndex(rect,1);
            //var rects = _mapCanvas.Children.OfType<Rectangle>().ToList();
            //foreach (var rectangle in rects)
           // {
            //    _mapCanvas.Children.Remove(rectangle);
            //}


            //DrawCells();
        }
    }

    
}
