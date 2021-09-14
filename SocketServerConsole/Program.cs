using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServerConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      StartServer();
    }
    public static void StartServer()
    {
      // Get Host IP Address that is used to establish a connection  
      // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
      // If a host has multiple addresses, you will get a list of addresses  
      List<Socket> socketList = new List<Socket>();
      IPAddress ipAddress = IPAddress.Parse("192.168.1.19");
      IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8090);


      try
      {

        // Create a Socket that will use Tcp protocol      
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        // A Socket must be associated with an endpoint using the Bind method  
        listener.Bind(localEndPoint);
        // Specify how many requests a Socket can listen before it gives Server busy response.  
        // We will listen 10 requests at a time  
        listener.Listen(10);

        Console.WriteLine("Waiting for a connection...");
        Socket handler = listener.Accept();
        socketList.Add(handler);
        handler.Send(SendString($"I see you!"));


        // Incoming data from the client.    
        string data = null;
        byte[] bytes = null;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        long counter = stopWatch.ElapsedMilliseconds;
        bool connected = true;
        while (connected == true)
        {
          if (stopWatch.ElapsedMilliseconds > counter)
          {
            Console.WriteLine("tick");
            counter = stopWatch.ElapsedMilliseconds + 5000;
            foreach (Socket s in socketList)
            {
              Console.WriteLine("Status of the Connections:");
              Console.WriteLine($"Connection: {s.Connected}");
            }
          }
          bytes = new byte[1024];
          int bytesRec = handler.Receive(bytes);
          data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
          if (data.IndexOf("<EOF>") > -1)
          {
            if (data == "SBF")
            {
              handler.Send(SendString("Closing Server"));
              handler.Shutdown(SocketShutdown.Both);
              handler.Close();
              connected = false;
            }
            Console.WriteLine("Text received : {0}", data);
            data = "";
          }
        }




      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }

      Console.WriteLine("\n Press any key to continue...");
      Console.ReadKey();


    }
    public static byte[] SendString(string msg)
    {
      return Encoding.ASCII.GetBytes(msg);
    }
  }
}
