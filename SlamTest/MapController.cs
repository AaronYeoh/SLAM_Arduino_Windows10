using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlamTest.Annotations;

namespace SlamTest
{
    public class MapController
    {
        public RobotPose BotPose = new RobotPose();
        public string message;

        public void DataReceivedEventHandler(object sender, SerialReadEventArgs serialReadEventArgs)
        {
            var text = serialReadEventArgs.Message;
            var obj = JsonConvert.DeserializeObject<RobotSensorModel>(text);
            BotPose.XPosBot = obj.xPos;
            BotPose.YPosBot = obj.yPos;
            BotPose.ZAngleBot = obj.zAng;
        }
    }

    public class RobotPose : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _xPosRobot;
        private int _yPosRobot;
        private int _zAngleRobot;

        public int XPosBot
        {
            get { return _xPosRobot; }
            set
            {
                if (value == _xPosRobot) return;
                _xPosRobot = value;
                OnPropertyChanged();
            }
        }

        public int YPosBot
        {
            get { return _yPosRobot; }
            set
            {
                if (value == _yPosRobot) return;
                _yPosRobot = value;
                OnPropertyChanged();
            }
        }

        public int ZAngleBot
        {
            get { return _zAngleRobot; }
            set
            {
                if (value == _zAngleRobot) return;
                _zAngleRobot = value;
                OnPropertyChanged();
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
