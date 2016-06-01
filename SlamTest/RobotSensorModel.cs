using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamTest
{
    class RobotSensorModel
    {
            public int xPos { get; set; }
            public int yPos { get; set; }
            public int zAng { get; set; }
            public double sonar { get; set; }
            public double leftB { get; set; }
            public double rightB { get; set; }
            public string message { get; set; }

    }
}
