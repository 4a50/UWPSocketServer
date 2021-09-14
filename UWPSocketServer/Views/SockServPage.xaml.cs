using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UWPSocketServer.Models;
using UWPSocketServer.ViewModels;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPSocketServer.Views
{
    public sealed partial class SockServPage : Page
    {
        StreamSocketListener StreamSocketListener { get; set; }
        private bool StopServer { get; set; }
        private string MessageFromServer { get; set; }        
        public SockServViewModel ViewModel { get; } = new SockServViewModel();

        static string PortNumber = "8090";
        private List<SocketTaskIOModel> TaskList { get; set; }       
        public SockServPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TaskList = new List<SocketTaskIOModel>();
            StartServer();

        }
        private async void SetBackgroundSocketServer()
        {
            //TimeTrigger minuteTrigger = new TimeTrigger(15, false);
            //var builder = new BackgroundTaskBuilder();
            //builder.Name = "SocketServerBackground";
            //builder.TaskEntryPoint = "BackgroundTasks.SocketServerBackground";
            //builder.SetTrigger(minuteTrigger);
            //BackgroundTaskRegistration reg = builder.Register();

            var socketTaskBuilder = new BackgroundTaskBuilder();
            socketTaskBuilder.Name = "TheREALSocket";
            socketTaskBuilder.TaskEntryPoint = "BackgroundTasks.RealSocket";
            var trig = new SocketActivityTrigger();
            socketTaskBuilder.SetTrigger(trig);
            BackgroundTaskRegistration sockReg = socketTaskBuilder.Register();

            StreamSocketListener _tcpListener = new StreamSocketListener();

            // Note that EnableTransferOwnership() should be called before bind,
            // so that tcpip keeps required state for the socket to enable connected
            // standby action. Background task Id is taken as a parameter to tie wake pattern
            // to a specific background task.  
            _tcpListener.EnableTransferOwnership(sockReg.TaskId, SocketActivityConnectedStandbyAction.Wake);
            _tcpListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
            await _tcpListener.BindServiceNameAsync("my-service-name");


        }
        public async void StartServer()
        {
            try
            {
                StreamSocketListener = new StreamSocketListener();
                Debug.WriteLine("streamSocketStarted");
                // The ConnectionReceived event is raised when connections are received.
                StreamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                IReadOnlyList<HostName> hosts = NetworkInformation.GetHostNames();
                int idx = 0;
                foreach (HostName h in hosts)
                {
                    UpdateConsoleListBox($"[{idx++}] Host Display Name: {h.DisplayName}");
                }

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await StreamSocketListener.BindEndpointAsync(hosts[4], PortNumber);

                UpdateConsoleListBox("server is listening...");
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                UpdateConsoleListBox(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
            //}
            //else
            //{
            //    UpdateConsoleListBox($"Current Port of the StreamSocketListener: {App.streamSocketListener.Information.LocalPort}");
            //    UpdateConsoleListBox($"Bypassing Listener Setup");
            //}

        }

        private void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StopServer = false;
            TaskList.Add(new SocketTaskIOModel
            {
                ID = "RaspThree",
                Read = new Task(() => inMonitor(args)),
                Write = new Task(() => outMonitor(args))
            }) ;
            TaskList[TaskList.Count - 1].Read.Start();
            TaskList[TaskList.Count - 1].Write.Start();
            }
        private async void inMonitor(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead());
            bool stopServer = false;
            string request;
            while (!stopServer)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    stopServer = StopServer;                    
                });

                if (!streamReader.EndOfStream)
                {
                    request = await streamReader.ReadLineAsync();
                    Debug.WriteLine("rcvFromClient");
                    UpdateClientListBox($"Client: {request}");
                    UpdateConsoleListBox(string.Format("server received the request: \"{0}\"", request));
                }
            }
            Debug.WriteLine("Stopping the InMonitor");
            streamReader.Close();
           
        }
        private async void outMonitor(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var streamWriter = new StreamWriter(args.Socket.OutputStream.AsStreamForWrite());
            bool stopServer = false;
            bool resetOutString = false;
            string sendToClient = "";
            while (!stopServer) {

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    stopServer = StopServer;
                    sendToClient = MessageFromServer;
                });

                if (resetOutString)
                {
                Debug.WriteLine($"stopServer: {stopServer} sendToClient: {sendToClient}");
                    ResetOutString();
                    resetOutString = false;
                    Debug.WriteLine("resetOutString");
                }
                if (sendToClient != "")
                {
                    Debug.WriteLine($"sendToClient: {sendToClient}");
                    await streamWriter.WriteLineAsync(sendToClient);
                    await streamWriter.FlushAsync();
                    resetOutString = true;
                }
            }
            await streamWriter.WriteLineAsync("JKF");
            await streamWriter.FlushAsync();
            streamWriter.Close();
            Debug.WriteLine("Stopping the OutMonitor");            
        }

                

        private async void UpdateConsoleListBox(string msg)
        {
            Debug.WriteLine(msg);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                consoleListBox.Items.Add(msg);
            });
        }
        private async void UpdateClientListBox(string msg)
        {
            
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                clientMessage_TextBox.Items.Add(msg);
                if (consoleListBox.Items.Count > 0) consoleListBox.SelectedIndex = consoleListBox.Items.Count - 1;
            });


        }
        private void OnClick_ClearConsole(object sender, RoutedEventArgs e)
        {
            consoleListBox.Items.Clear();
        }
        private void OnClick_SendToClient(object sender, RoutedEventArgs e)
        {

            MessageFromServer = serverMessage_TextBox.Text;
            UpdateConsoleListBox("Sending: "+ serverMessage_TextBox.Text);

        }
        private void OnClick_ShutdownServer(object sender, RoutedEventArgs e)
        {
            UpdateConsoleListBox("Closing all Streams");
            StopServer = true;
            Thread.Sleep(200);
            UpdateConsoleListBox($"Total Client Threads Tracking: {TaskList.Count}");
            foreach (SocketTaskIOModel t in TaskList)
            {
                UpdateConsoleListBox($"SocketStreams Status: Read - {t.Read.Status} Write - {t.Write.Status}");
                t.Read.Dispose();
                t.Write.Dispose();
                UpdateConsoleListBox("Read/Write Disposed");
            }
            TaskList.Clear();
            UpdateConsoleListBox("Closing Socket Connection");
            StreamSocketListener.CancelIOAsync();
            StreamSocketListener.Dispose();
        }
        private void OnClick_ClearClientWindow(object sender, RoutedEventArgs e)
        {
            clientMessage_TextBox.Items.Clear();
        }
        private void ResetOutString()
        {
            MessageFromServer = "";

        }
    }

}
