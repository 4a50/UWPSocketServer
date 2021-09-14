using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaspiClient
{
  public class Client
  {

    private TcpClient clientSocket = new TcpClient();
    private NetworkStream serverStream;
    private byte[] outStream { get; set; }   
    public IPAddress ipAddress { get; set; }
    public int port { get; set; }
    public bool isConnected { get; set; }
    public Client(string ipAdd, string prt)


    {
      ipAddress = IPAddress.Parse(ipAdd);
      port = int.Parse(prt);      
      Console.WriteLine("Starting Client");
      Connect();

    }

    private bool Connect()
    {
      int counter = 0;
      Debug.WriteLine("Attempting to Connect to {0}:{1}", ipAddress.ToString(), port.ToString());
      Console.WriteLine("Attempting to Connect to {0}:{1}", ipAddress.ToString(), port.ToString());
      isConnected = false;
      while (counter++ <= 10 && !isConnected)
      {
        try
        {
          clientSocket.Connect(ipAddress, port);
          Debug.WriteLine("Client Socket Program - Server Connected.");
          Console.WriteLine("Client Socket Program - Server Connected.");
          isConnected = true;
        }

        catch
        {
          Debug.WriteLine("Unable to Connect.  Try " + counter.ToString());
          Console.WriteLine("Unable to Connect.  Try " + counter.ToString());
        }
      }
      return isConnected;
    }
    public void RunClient()
    {      
      serverStream = clientSocket.GetStream();
      outStream = Encoding.UTF8.GetBytes("Message From Client: ");
      serverStream.Write(outStream, 0, outStream.Length);
      serverStream.Flush();
      byte[] data = new byte[256];
      string recievedString = string.Empty;
      Send("Raspberry Pi Checking in");
      Stopwatch stw = new Stopwatch();
      long marker = stw.ElapsedMilliseconds;
      Console.WriteLine($"Marker: {marker}");
      int interval = 3000;
      stw.Start();
      ConsoleKeyInfo cki;
      bool stopClient = false;
      serverStream.Flush();
      //StartKeyInputThread();
      while (!stopClient)
      {
        if (stw.ElapsedMilliseconds >= marker)
        {
          marker = stw.ElapsedMilliseconds + interval;
          Console.WriteLine($"{ interval / 1000} seconds elapsed.  Marker: {marker} stopClient: {stopClient}");
        }
        if (Console.KeyAvailable)
        {
          cki = Console.ReadKey(true);
          if (cki.Key == ConsoleKey.M) EnterStringToSend();
          else if (cki.Key == ConsoleKey.Q) stopClient = true;
        }
        //Check INCOMING DATA        
        if (serverStream.DataAvailable)
        {
          int bytes = serverStream.Read(data, 0, data.Length);
          recievedString = Encoding.ASCII.GetString(data, 0, bytes);
          string editedString = recievedString.Trim();
          Console.WriteLine($"Rcv: -->{editedString}<--  String Length: {editedString.Length}");
          if (editedString.ToUpper() == "JKF") stopClient = true;

          serverStream.Flush();
        }

      }

      Console.WriteLine("Closing Socket Connection");
      clientSocket.Close();
    }
    public void EnterStringToSend()
    {
      Console.WriteLine("Enter Message to send");
      string msg = Console.ReadLine();
      Send(msg);
    }

    public void Send(string str)
    {
      Console.WriteLine("Byte Length: " + str.Length.ToString());
      Console.WriteLine("Sending: " + str);
      Console.WriteLine();
      str += Environment.NewLine;
      serverStream.Write(Encoding.UTF8.GetBytes(str), 0, str.Length);
    }
  }
}
