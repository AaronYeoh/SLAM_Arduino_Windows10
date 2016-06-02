using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamTest
{
    class RobotSensorModel
    {
            public float xPos { get; set; }
            public float yPos { get; set; }
            public float zAng { get; set; }
            public double sonar { get; set; }
            public double leftB { get; set; }
            public double rightB { get; set; }
            public string message { get; set; }
            public bool Enabled { get; set; }

    }
}
