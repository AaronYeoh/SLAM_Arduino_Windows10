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
        public ObstaclePositions obstaclePositons = new ObstaclePositions();

        public void DataReceivedEventHandler(object sender, SerialReadEventArgs serialReadEventArgs)
        {
            var text = serialReadEventArgs.Message;
            try
            {
                var obj = JsonConvert.DeserializeObject<RobotSensorModel>(text);

                BotPose.XPosBot = obj.xPos;
                BotPose.YPosBot = obj.yPos;
                BotPose.ZAngleBot = obj.zAng;


                UpdateObstaclePositions(obj);
            }
            catch
            {
                //exterminate}
            }
        }

        private void UpdateObstaclePositions(RobotSensorModel obj)
        {
            var angle = MathH.DegToRad(BotPose.ZAngleBot);

            var frontObjectXPosition = BotPose.XPosBot + Math.Sin(angle) * obj.sonar;
            var frontObjectYPosition = BotPose.YPosBot + Math.Cos(angle) * obj.sonar;


            var leftObjectXPosition = BotPose.XPosBot + Math.Cos(angle)*obj.leftB;
            var leftObjectYPosition = BotPose.YPosBot - Math.Sin(angle)*obj.leftB;

            var rightObjectXPosition = BotPose.XPosBot - Math.Cos(angle)*obj.rightB;
            var rightObjectYPosition = BotPose.YPosBot + Math.Sin(angle)*obj.rightB;
            List<int[]> listOfObstacles = new List<int[]>
            {
                new int[] {(int) frontObjectXPosition, (int) frontObjectYPosition},
                new int[] {(int) leftObjectXPosition, (int) leftObjectYPosition},
                new int[] {(int) rightObjectXPosition, (int) rightObjectYPosition}
            };
            obstaclePositons.listOfObstacles = listOfObstacles;

        }
    }

    public class ObstaclePositions : INotifyPropertyChanged
    {
        public List<int[]> listOfObstacles
        {
            get { return _listOfObstacles; }
            set
            {
                if (value == _listOfObstacles) return;
                _listOfObstacles = value;
                OnPropertyChanged();
            }
        }

        private List<int[]> _listOfObstacles;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class MathH
    {
        public static double DegToRad(double angle)
        {
            return (Math.PI/180)*angle;
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
