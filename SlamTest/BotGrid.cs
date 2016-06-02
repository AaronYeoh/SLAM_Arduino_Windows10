using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private RobotPose _botPose;
        private ObstaclePositions _obstaclePositions;
        private static BotGrid _instance = null;
        public static BotGrid Instance { get { return _instance; } }
        public BotGrid(int rows, int cols, int rowHeight, int colWidth, Canvas mapCanvas, int scale, RobotPose botPose, ObstaclePositions obstaclePositions)
        {
            _rows = rows;
            _cols = cols;
            _mapCanvas = mapCanvas;
            _rowHeight = rowHeight;
            _colWidth = colWidth;
            _scale = scale;
            _botPose = botPose;
            _instance = this;
            _obstaclePositions = obstaclePositions;
            _obstaclePositions.PropertyChanged += ObstaclePositionsOnPropertyChanged;
            _botPose.PropertyChanged += BotPoseOnPropertyChanged;
        }

        private void ObstaclePositionsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var obsPos = sender as ObstaclePositions;
            AddNewObstacleReadings(obsPos.listOfObstacles);
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
                    cells[i, j] = new Cell(CellStatus.Unknown,  i * _rowHeight * 2,j * _colWidth * 2, i, j ,_colWidth *2);
                }
            }
            DrawCells();

        }
        public void DrawCells()
        {
            foreach (var cell in cells)
            {
                DrawSingleCell(cell);
            }

        }

        public void RedrawCells()
        {
            _botPose.PropertyChanged -= BotPoseOnPropertyChanged;
            var rects = _mapCanvas.Children.OfType<Rectangle>().ToList();
            foreach (var rect in rects)
            {
                _mapCanvas.Children.Remove(rect);
            }
            CreateCells();
            DrawCells();
            _botPose.PropertyChanged += BotPoseOnPropertyChanged;
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

            Bresenham.Line(_botPose.XPosBot/5, _botPose.YPosBot/5,tag[0],tag[1], new Bresenham.PlotFunction(SetCell));
            //Bresenham.Line(10, 20, tag[0], tag[1], new Bresenham.PlotFunction(SetCell));
            cells[tag[0],tag[1]].status = CellStatus.Obstacle;
        }

        public void AddNewObstacleReadings(List<int[]> list)
        {
            foreach (var obstacle in list)
            {
                try
                {
                    AddNewObstacleReading(obstacle[0]/5, obstacle[1]/5);
                }
                catch { }
            }
        }
        private void AddNewObstacleReading(int x, int y)
        {
            Bresenham.Line(_botPose.XPosBot / 5, _botPose.YPosBot / 5, x, y, new Bresenham.PlotFunction(SetCell));
            //Bresenham.Line(10, 20, tag[0], tag[1], new Bresenham.PlotFunction(SetCell));
            cells[x, y].status = CellStatus.Obstacle;
        }

        private void BotPoseOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_botPose.Enabled)
            {
                cells[_botPose.XPosBot/5, _botPose.YPosBot/5].status = CellStatus.Covered;
            }
        }
    }

    
}
