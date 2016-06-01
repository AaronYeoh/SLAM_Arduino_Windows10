using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SlamTest
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SerialDevice myport;
        private DeviceInformationCollection tempInfo;
        private DeviceInformation selectedSerialDevice;
        private bool isStopped = true;
        private MapController mapController;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshSerialDevices();
            //int rows = 24, cols = 40;
            int rows = 50, cols = 50;
            int scale = 5; // 5cm Real World per pixel
            MapCanvas.Width = cols*50;
            MapCanvas.Height = rows * 50;
            int colWidth = (int)MapCanvas.Width / (2*cols);
            int rowHeight = colWidth;

            mapController = new MapController();

            RobotShape.DataContext = mapController.BotPose;
            

            BotGrid botGrid = new BotGrid(rows, cols, rowHeight, colWidth, MapCanvas, scale, mapController.BotPose);
            botGrid.DrawGrid();
            
        }

        



        private void SerialList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartStopButton.IsEnabled = true;
            selectedSerialDevice = tempInfo[SerialList.SelectedIndex];
            StopSerialRead();
        }

        SerialReader serialReader = new SerialReader();
        private async void StartStopSerialButton_OnClick(object sender, RoutedEventArgs e)
        {
            await StartStopSerialRead();
        }

        private async Task StartStopSerialRead()
        {
            if (isStopped)
            {
                await StartSerialRead();
            }
            else
            {
                StopSerialRead();
            }
        }

        private void StopSerialRead()
        {
            isStopped = true;
            StartStopButton.Icon = new SymbolIcon(Symbol.Play);
            serialReader.CancelReadTask();
            serialReader.RaiseDataReceivedEvent -= DataReceivedEventHandler;
            serialReader.RaiseDataReceivedEvent -= mapController.DataReceivedEventHandler;
        }

        private async Task StartSerialRead()
        {
            isStopped = false;
            StartStopButton.Icon = new SymbolIcon(Symbol.Stop);
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Stop reading Serial Device";
            ToolTipService.SetToolTip(StartStopButton, toolTip);

            var serialDevice = await SerialDevice.FromIdAsync(selectedSerialDevice.Id);

            if (serialDevice != null)
            {
                serialDevice.BaudRate = 115200;
                serialDevice.IsDataTerminalReadyEnabled = true;
                serialReader.Listen(serialDevice);
                serialReader.RaiseDataReceivedEvent += DataReceivedEventHandler;
                serialReader.RaiseDataReceivedEvent += mapController.DataReceivedEventHandler;
            }
            else
            {
                //The serial device doesn't work.
                RefreshSerialDevices();
            }
        }

        private void DataReceivedEventHandler(object sender, SerialReadEventArgs serialReadEventArgs)
        {
            SerialReadTextBlock.Text = serialReadEventArgs.Message;
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshSerialDevices();
        }

        private async void RefreshSerialDevices()
        {
            //Prevent null exceptions
            SerialList.SelectionChanged -= SerialList_OnSelectionChanged;
            string _serialSelector = SerialDevice.GetDeviceSelector();
            //SerialInfoTextBlock.Text = SerialDevice.GetDeviceSelectorFromUsbVidPid(2341, 8041);
            tempInfo = await DeviceInformation.FindAllAsync(_serialSelector);

            SerialList.DataContext = tempInfo;
            SerialList.Items?.Clear();
            SerialList.SelectionChanged += SerialList_OnSelectionChanged;
            foreach (var item in tempInfo)
            {
                SerialList.Items.Add(item.Name);
                //Fast track selection
                if (item.Name.Contains("Arduino") || item.Name.Contains("BTG"))
                {
                    SerialList.SelectedItem = item.Name;
                }
            }
            if (tempInfo.Count == 0)
            {
                SerialList.PlaceholderText = "No serial devices available";
            }
        }
    }
}
