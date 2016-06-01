using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamTest
{
    public enum CellStatus
    {
        Unknown = 0,
        Clear = 1,
        Obstacle = 2,
        Covered = 3
    }
    public class Cell
    {
        // Properties of the cell to determine state and location
        private CellStatus _status;
        private short lowerStatePreviouslyDetected; // A flag to indicate whether 
        public short cellX;
        public short cellY;
        public short cellSize;
        public short cellPosX;
        public short cellPosY;
        private static short threshold = 2;


        private Cell() { }

        // Explicit value constructor for creating a cell 
        public Cell(CellStatus x, int cX, int cY, int cXPos, int cYPos, int size)
        {
            _status = x;
            cellX = (short)cX;
            cellY = (short)cY;
            cellSize = (short)size;

            //Cell's index in array
            cellPosX = (short)cXPos;
            cellPosY = (short)cYPos;
        }

        public CellStatus status
        {
            get { return _status; }
            set
            {
                // Unknown < Clear < Obstacle < Covered
                if (_status < value)
                {
                    _status = value;
                    BotGrid.Instance?.DrawSingleCell(this);
                }
                else
                {
                    if (lowerStatePreviouslyDetected > threshold)
                    {
                        _status = value;
                        lowerStatePreviouslyDetected = 0;
                        BotGrid.Instance?.DrawSingleCell(this);
                    }
                    lowerStatePreviouslyDetected++;
                }
            }
        }


    }
}
