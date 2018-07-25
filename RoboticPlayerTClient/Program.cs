using System;

class Program
{
    static void Main(string[] args)
    {
        ThalamusConnector thalamusCS;
        string character = "";
        int port = 7000;

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0] + " <CharacterName> <PortNumber>");
            return;
        }
        else
        {
            character = args[0];
            port = Int16.Parse(args[1]);

            thalamusCS = new ThalamusConnector("RoboticPlayer", character);
            UnityConnector unityCS = new UnityConnector(thalamusCS, port);
            thalamusCS.UnityConnector = unityCS;

            Console.ReadLine();
            thalamusCS.Dispose();
        }
        
    }
}
