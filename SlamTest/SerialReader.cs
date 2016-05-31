using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace SlamTest
{
    class SerialReader
    {
        private DataReader dataReaderObject;
        private CancellationTokenSource ReadCancellationTokenSource;
        private SerialDevice _serialPort;
        private string latestString;
        public delegate void CustomEventHandler(object sender, SerialReadEventArgs a);

        public event EventHandler<SerialReadEventArgs> RaiseDataReceivedEvent;

        protected virtual void OnDataReceived(SerialReadEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<SerialReadEventArgs> handler = RaiseDataReceivedEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        private bool status;
        /// <summary>
        /// - Create a DataReader object
        /// - Create an async task to read from the SerialDevice InputStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Listen(SerialDevice serialPort)
        {
            ReadCancellationTokenSource = new CancellationTokenSource();
            _serialPort = serialPort;
            try
            {
                if (serialPort != null)
                {
                    dataReaderObject = new DataReader(serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        status = await ReadAsync(ReadCancellationTokenSource.Token);
                        if (status == true)
                        {
                            status = false;
                            OnDataReceived(new SerialReadEventArgs(latestString));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "TaskCanceledException" || ex.GetType() == typeof(OperationCanceledException))
                {
                    CloseDevice();
                }
                else
                {
                    var problemTxt = ex.Message; // Does nothing for now
                }
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }
        StringBuilder stringBuilder = new StringBuilder();
        private async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1;
            
            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;

            var str = dataReaderObject.ReadString(bytesRead);

            if (str.Contains("\n"))
            {
                latestString = stringBuilder + str;
                stringBuilder.Length = 0;
                return true;
            }
            else
            {
                stringBuilder.Append(str);
            }


            return false;
        }



        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        public void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        private void CloseDevice()
        {
            if (_serialPort != null)
            {
                _serialPort.Dispose();
            }
            _serialPort = null;
        }
    }



    public class SerialReadEventArgs : EventArgs
    {
        public SerialReadEventArgs(string s)
        {
            msg = s;
        }
        private string msg;
        public string Message
        {
            get { return msg; }
        }
    }
}
