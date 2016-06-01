using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamTest
{
    public class Cell
    {
        // Properties of the cell to determine state, size, and location
        bool isAlive;
        float cellWidth;
        float cellHeight;
        float cellX;
        float cellY;

        // Explicit value constructor for creating a cell when clicking on the grid
        public Cell(bool x, float cW, float cH, float cX, float cY)
        {
            isAlive = x;
            cellWidth = cW;
            cellHeight = cH;
            cellX = cX;
            cellY = cY;
        }

        // Properties
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public float CellHeight
        {
            get { return cellHeight; }
            set { cellHeight = value; }
        }

        public float CellWidth
        {
            get { return cellWidth; }
            set { cellWidth = value; }
        }

        public float CellX
        {
            get { return cellX; }
            set { cellX = value; }
        }

        public float CellY
        {
            get { return cellY; }
            set { cellY = value; }
        }

    }
}
