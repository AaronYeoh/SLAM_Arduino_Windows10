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

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshSerialDevices();
        }



        private void SerialList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSerialButton.IsEnabled = true;
            selectedSerialDevice = tempInfo[SerialList.SelectedIndex];
            StartSerialButton.Content = selectedSerialDevice.Name;
        }

        SerialReader serialReader = new SerialReader();
        private async void StartSerialButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopSerialButton.IsEnabled = true;

            var serialDevice = await SerialDevice.FromIdAsync(selectedSerialDevice.Id);

            if (serialDevice != null)
            {
                serialDevice.BaudRate = 115200;
                //SerialReadTextBlock.Text = serialDevice.PortName;
                serialDevice.IsDataTerminalReadyEnabled = true;
                //SerialReader serialReader = new SerialReader();
                serialReader.Listen(serialDevice);
                serialReader.RaiseDataReceivedEvent += SerialReaderOnRaiseDataReceivedEvent;
            }
            else
            {
                //The serial device doesn't work.
                RefreshSerialDevices();
            }
        }

        private void SerialReaderOnRaiseDataReceivedEvent(object sender, SerialReadEventArgs serialReadEventArgs)
        {
            SerialReadTextBlock.Text = serialReadEventArgs.Message;
        }

        private void StopSerialButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopSerialButton.IsEnabled = false;
            serialReader.CancelReadTask();
            serialReader.RaiseDataReceivedEvent -= SerialReaderOnRaiseDataReceivedEvent;
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
            foreach (var item in tempInfo)
            {
                SerialList.Items.Add(item.Name);
            }
            //Prevent nulls
            SerialList.SelectionChanged += SerialList_OnSelectionChanged;

        }
    }
}
