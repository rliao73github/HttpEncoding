using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HttpEncoding
{
    //-- 206 Partial Content, to GET  via 80
    //-- {}{5}RESOLVED 505 extra respons issue. Using Connection: close
    //-- wierd thing is rcv 494, 12, 505 three packets
    //-- second wierd thing is after rcving 494, 12, then hanging; FIXED by adding <EOF> in req header

    public class PartialContent80
    {
        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        // This method requests the home page content for the specified server.
        private static string SocketSendReceive(string server, int port)
        {
            string request = "GET /kplabs-logo.png HTTP/1.1" + "\r\n" +
                "Host: " + server + "\r\n" +
                "Range: bytes=0-16\r\n" +
                "Connection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[1024];
            string page = "";

            // Create a socket connection with the specified server and port.
            using (Socket s = ConnectSocket(server, port))
            {

                if (s == null)
                    return ("Connection failed");

                // Send request to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the server home page content.
                int bytes = 0;
                page = "Default HTML page on " + server + ":\r\n";

                // The following will block until the page is transmitted.
                do
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
            }

            return page;
        }

        public static void Main(string[] args)
        {
            string host = "dexter.kplabs.in";
            int port = 80;

            Console.WriteLine("Tcp streatm Test - Begin.");

            //if (args.Length == 0)
            //    host = "dexter.kplabs.in"; //-- OK with port 80
            //else
            //    host = args[0];

            string result = SocketSendReceive(host, port);
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}