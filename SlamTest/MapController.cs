using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Newtonsoft.Json;
using SlamTest.Annotations;

namespace SlamTest
{
    public class MapController
    {
        private string oldMessage="";
        public RobotPose BotPose = new RobotPose();
        public string message;
        public ObstaclePositions obstaclePositons = new ObstaclePositions();

        public void DataReceivedEventHandler(object sender, SerialReadEventArgs serialReadEventArgs)
        {
            var text = serialReadEventArgs.Message.Trim();
            if (text.StartsWith("{") && text.EndsWith("}")) //Check if is a json-like string. Avoid exceptions.
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<RobotSensorModel>(text);

                    BotPose.Enabled = obj.Enabled;
                    BotPose.XPosBot = (int) obj.xPos;
                    BotPose.YPosBot = (int) obj.yPos;
                    BotPose.ZAngleBot = (int) obj.zAng;

                    if (obj.message != oldMessage)
                    {
                        ApplicationView applicationView = ApplicationView.GetForCurrentView();
                        applicationView.Title = obj.message;
                        oldMessage = obj.message;
                    }

                    if (obj.Enabled)
                    {
                        UpdateObstaclePositions(obj);
                    }
                }
                catch
                {
                    //exterminate}
                }
            }
        }

        private void UpdateObstaclePositions(RobotSensorModel obj)
        {
            List<int[]> listOfObstacles = new List<int[]>();
            var angle = MathH.DegToRad(BotPose.ZAngleBot);

            if (obj.sonar < 198 && obj.sonar > 0.0)
            {
                var frontObjectXPosition = BotPose.XPosBot + Math.Sin(angle)*obj.sonar;
                var frontObjectYPosition = BotPose.YPosBot + Math.Cos(angle)*obj.sonar;
                listOfObstacles.Add(new int[] {(int) frontObjectXPosition, (int) frontObjectYPosition});
            }
            if (obj.leftB < 80.0 && obj.leftB > 0.0)
            {
                var leftObjectXPosition = BotPose.XPosBot + Math.Cos(angle)*obj.leftB;
                var leftObjectYPosition = BotPose.YPosBot - Math.Sin(angle)*obj.leftB;
                listOfObstacles.Add(new int[] {(int) leftObjectXPosition, (int) leftObjectYPosition});
            }
            if (obj.rightB < 80.0 && obj.rightB > 0.0)
            {
                var rightObjectXPosition = BotPose.XPosBot - Math.Cos(angle)*obj.rightB;
                var rightObjectYPosition = BotPose.YPosBot + Math.Sin(angle)*obj.rightB;
                listOfObstacles.Add(new int[] {(int) rightObjectXPosition, (int) rightObjectYPosition});
            }
            
                
                
                
            
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

        public bool Enabled { get; set; }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
