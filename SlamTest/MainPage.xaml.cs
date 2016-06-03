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
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
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
        private BotGrid botGrid;
        private Windows.Storage.StorageFile file;
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


            botGrid = new BotGrid(rows, cols, rowHeight, colWidth, MapCanvas, scale, mapController.BotPose, mapController.obstaclePositons);
            botGrid.DrawGrid();
        }

        



        private void SerialList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartStopButton.IsEnabled = true;
            selectedSerialDevice = tempInfo[SerialList.SelectedIndex];
            //Prevent this from firing too early and getting null exception
            if (!isStopped)
            {
                StopSerialRead();
            }
        }

        SerialReader serialReader = new SerialReader();
        private async void StartStopSerialButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await StartStopSerialRead();
            }
            catch { StopSerialRead();}
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
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Start reading Serial Device";
            ToolTipService.SetToolTip(StartStopButton, toolTip);
            StartStopButton.Label = "Start reading Serial Device";

            serialReader.CancelReadTask();
            serialReader.RaiseDataReceivedEvent -= DataReceivedEventHandler;
            serialReader.RaiseDataReceivedEvent -= mapController.DataReceivedEventHandler;
        }

        private async Task StartSerialRead()
        {
            isStopped = false;
            StartStopButton.Icon = new SymbolIcon(Symbol.Stop);
            StartStopButton.Label = "Stop reading Serial Device";
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Stop reading Serial Device";
            ToolTipService.SetToolTip(StartStopButton, toolTip);
            try
            {
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
                    ApplicationView applicationView = ApplicationView.GetForCurrentView();
                    applicationView.Title = "Failed to read from device";
                }
            }
            catch (Exception e)
            {
                //The serial device doesn't work.
                RefreshSerialDevices();

                ApplicationView applicationView = ApplicationView.GetForCurrentView();
                applicationView.Title = e.Message;
            }
        }

        private void DataReceivedEventHandler(object sender, SerialReadEventArgs serialReadEventArgs)
        {
            if (SaveSerialToggleButton.IsChecked.GetValueOrDefault())
            {
                SaveToFile(serialReadEventArgs.Message);
            }
            SerialReadTextBlock.Text = serialReadEventArgs.Message;
        }

        private async void SaveToFile(string str)
        {
            if (file != null)
            {
                int NumberOfRetries = 5;
                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        await Windows.Storage.FileIO.AppendTextAsync(file, str);
                        break;
                    }
                    catch
                    {
                        //if (i == NumberOfRetries)
                            //file = null; // make user choose another file
                            Task.Delay(i+2).Wait();
                    }
                }
            }

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

        private void ClearMapButton_OnClick(object sender, RoutedEventArgs e)
        {
            botGrid.RedrawCells();
        }

        private async void BluetoothButton_OnClick(object sender, RoutedEventArgs e)
        {
            bool result = await Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
            if (!result)
            {
                //App is running on phone, open general settings
                bool result2 = await Launcher.LaunchUriAsync(new Uri("ms-settings:"));
            }
        }

        private async void SaveSerialToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            await GetFileReference();
        }

        private async Task GetFileReference()
        {
            if (SaveSerialToggleButton.IsChecked.GetValueOrDefault())
            {
                try
                {
                    file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync("Default");
                    await file.OpenAsync(FileAccessMode.ReadWrite);
                }
                catch
                {
                    
                }
                if (file == null)
                {
                    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation =
                        Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add("Plain Text", new List<string>() {".txt"});
                    // Default file name if the user does not type one in or select a file to replace
                    savePicker.SuggestedFileName = "705SerialData";
                    file = await savePicker.PickSaveFileAsync();
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("Default", file);
                }
            }
        }
    }
}
