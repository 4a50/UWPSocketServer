using System;
using System.IO;
using System.Text;
namespace InputRead
{


  public class StatusTracker
  {
    string Name { get; set; }
    char ElemType { get; set; }
    public bool SwOne { get; set; }
    public bool LtOne { get; set; }

    public StatusTracker()
    {
    }
  }


  public class NoProgram
  {
    public static void NoMain()
    {
      int[] a = { 1, 2, 3, 4, 5 };
      var panelOne = new StatusTracker();
      var panelTwo = new StatusTracker();


      //Verify Object Made
      Console.WriteLine(panelOne.SwOne.ToString());
      string str = "123456789";
      Console.WriteLine("str: " + str.Length.ToString());
      //Change str to a Byte Array
      byte[] bytes = Encoding.UTF8.GetBytes(str);
      Console.WriteLine("strBytes: " + bytes.Length.ToString() + "\t" + bytes.ToString());
      //Change Byte Array Back into a string
      string convString = Encoding.UTF8.GetString(bytes);
      Console.WriteLine("strBack: " + convString);

      //Convert the panelOne Object into a JSON string
      string jsonConvert = Newtonsoft.Json.JsonConvert.SerializeObject(panelOne);
      Console.WriteLine("JSON: " + jsonConvert);
      //Convert the JSON string back into a StatusTracker Object
      var deserializeJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<StatusTracker>(jsonConvert);
      Console.WriteLine("JSON2OBJ: " + deserializeJSON.SwOne.ToString());




      //Converting to a JSON string
      //string moduleOneJson = Newtonsoft.Json.JsonConvert.SerializeObject(module1);
      //Console.WriteLine(moduleOneJson);
      //  EXAMPLE Config File Entry
      //
      //  FloodLightFwd=S,1,0
      //  FloodLightAft=S,1,0
      //  Velocity=M,34.2,1
      // Name of Element=Type, Value, Disabled

      StreamWriter file = new StreamWriter("client.cfg");



      //Creating a sample transmission to a file.  Initial Objectbuild.
      //StreamWriter missionFile = new StreamWriter("missionFile.txt");
      //missionFile.WriteLine(moduleOneJson);
      //missionFile.Close();

      //////////////////////////////

      //This is a test of the encoding and transmission process.
      //Instead of Transmitting, Just Encoding and decoding will be used.

      //Server Sends Initial Object to the Server.  Server Recieved JSON string

      //Read the config file and instantiate an object with the appropriate
      //objects.



      //Populate the properties as required.

      //Convert the object to a JSON string

      //Convert the string to a byte[]

      //<<<This is where the transmission would occur>>>

      //Convert the byte[] to a string

      //Convert the JSON String to a object

      //Display all the properties of an object.



    }

    //void createConfig(List<ElementDesign> obj, StreamWriter file)
    //{
    //	string val;
    //	file.WriteLine("module=1");
    //	foreach (var elem in obj)
    //	{
    //		if (elem.ElementType == 'S' || elem.ElementType == 'L') { val = elem.Value.ToString(); }
    //		else if (elem.ElementType == 'A') { val = elem.FValue.ToString(); }
    //		else { val = "N"; }
    //		Console.WriteLine("{0}={1},{2},{3}", elem.Name, elem.ElementType, val,
    //		elem.IsDisabled);
    //		file.WriteLine("{0}={1},{2},{3}", elem.Name, elem.ElementType, val,
    //		elem.IsDisabled);

    //	}
    //}
  }

}