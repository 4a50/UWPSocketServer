using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace RaspiClient
{
  class Program
  {


    static void Main(string[] args)
    {
      //DemoClient.RunClient();
      //string[] tempargs = new string[] { "setip", "192.168.1.19:8090" };
      if (args.Length == 0) Console.WriteLine("No Arguments provided.  Require IPAddress:Port to start program");
      else if (args.Length >= 2 && args[0].ToUpper() == "SETIP" || args[0].ToUpper() == "-SETIP")
      {

        Regex regexIP = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
        Regex regexPort = new Regex(@"^(?:[0-9]{1,4})$");
        string[] connectionString = args[1].Split(':');
        if (regexIP.IsMatch(connectionString[0]) && regexPort.IsMatch(connectionString[1]))
        {
          Console.WriteLine("Running Client");
          Client client = new Client(connectionString[0], connectionString[1]);
          if (client.isConnected) client.RunClient();
          else { Console.WriteLine("Unable to Connect"); }
        }
        else
        {
          Console.WriteLine("Unable to Connect.  Invalid argument. (eg, 'setip 000.000.0.00'");
        }

      }
    }
    public static void SocketClose(Socket socket)
    {
      socket.Shutdown(SocketShutdown.Both);
      socket.Close();
      Console.WriteLine("Closed Socket");
    }
  }
}






