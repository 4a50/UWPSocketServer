using System;
using System.Threading;

namespace ProvingGround
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      ConsoleKeyInfo cki;
      while (true)
      {
        if (Console.KeyAvailable)
        {
          cki = Console.ReadKey(true);
          Console.WriteLine($"cki: {cki.Key}");
          Thread.Sleep(500);
        }
        Console.WriteLine("ticky");
      }
    }
  }
}
