using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Telehash.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter 1 for sender, 2 for listener: ");
            ConsoleKeyInfo Key = Console.ReadKey();

            if (Key.KeyChar == '1')
            {
                Console.WriteLine();
                Sender();
            }
            else if (Key.KeyChar == '2')
            {
                Console.WriteLine();
                Listener();
            }
            else
            {
                Console.WriteLine("You did not hit the correct key. Exiting...");
            }
        }

        private static void Sender()
        {
            Console.WriteLine("Starting up Switch...");

            Switch ThisSwitch = Switch.Current;

            Console.Write("Destination IP Address: ");
            String DestinationIpAddress = Console.ReadLine();

            Console.Write("Destination Port: ");
            String DestinationPort = Console.ReadLine();

            Console.Write("Text to transmit:");
            String Text = Console.ReadLine();

            ThisSwitch.SendTelex(
                Text, 
                new IPEndPoint(
                    IPAddress.Parse(DestinationIpAddress), 
                    Convert.ToInt32(DestinationPort)));

            Console.WriteLine("Transmitted " + Text + " to " + DestinationIpAddress);
            Console.WriteLine("Hit return when done.");
            Console.ReadLine();

            ThisSwitch.StopListening();
        }

        private static void Listener()
        {
            Console.Write("Port to listen on : ");
            string port = Console.ReadLine();
            Console.WriteLine("Starting up Switch and listening on port " + port + "...");

            Switch ThisSwitch = Switch.Current;
            ThisSwitch.StartListening(Convert.ToInt32(port));
            ThisSwitch.TelexReceived += ThisSwitchTelexReceived;

            Console.WriteLine("Port opened. Waiting for a telex to appear...");
            Console.ReadLine();
        }

        private static void ThisSwitchTelexReceived(TelexReceivedEventArgs eventArgs)
        {
            Console.WriteLine("Got some text!!! The sender says: " + eventArgs.TelexMessage.ToString());
            Console.WriteLine("Press enter when finished.");
            Console.ReadLine();
        }
    }
}
