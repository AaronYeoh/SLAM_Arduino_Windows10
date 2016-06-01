using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamTest
{
    public enum CellStatus
    {
        Unknown,
        Covered,
        Obstacle,
        Clear
    }
    public class Cell
    {
        // Properties of the cell to determine state and location
        public CellStatus status;
        public int cellX;
        public int cellY;
        public int cellSize;

        private Cell() { }
        public Cell(int cX, int cY, int size)
        {
            status = CellStatus.Unknown;
            cellX = cX;
            cellY = cY;
            cellSize = size;
        }
        // Explicit value constructor for creating a cell 
        public Cell(CellStatus x, int cX, int cY, int size)
        {
            status = x;
            cellX = cX;
            cellY = cY;
            cellSize = size;
        }


    }
}
