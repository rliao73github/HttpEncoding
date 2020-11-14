using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class GetSocketPort80Bin
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
        //-- int byteChunk = 25600;  can be really really slow
        int byteChunk = 256; //-- 2048;
        string request = "GET /echo/get/json HTTP/1.1" + 
            "\r\nHost: " + server +
            "\r\nAccept: application/json" +
            "\r\nConnection: close\r\n\r\n";

        Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
        Byte[] bytesReceived = new Byte[byteChunk];

        //-- Byte[] bytes4x = new Byte[byteChunk*4];
        string page = "";
        string pageEnd = "";
        string part1 = "";
        string part2 = "";

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
            int count = 0;
            do
            {
                count++;

                if (count > 100) break;

                bytes = s.Receive(bytesReceived, bytesReceived.Length, SocketFlags.None);
                if (bytes < byteChunk)
                {
                    part2 = Encoding.ASCII.GetString(bytesReceived, 0, byteChunk);

                    pageEnd = pageEnd + part2;

                }
                part1 = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                //-- fxied the bug, bytesReceived contains garbage data of last read
                Array.Clear(bytesReceived, 0, bytesReceived.Length);

                page = page + part1;
            }
            while (bytes > 0);
        }

        return page;
    }

    public static void TEST_Main(string[] args)
    {
        string host;
        int port = 80;

        if (args.Length == 0)
            // If no server name is passed as argument to this program,
            // use the current host name as the default.
            //-- host = Dns.GetHostName();
            host = "www.google.com"; //-- OK with port 80 
           //-- host = "ebilling.kitchener.ca"; //-- NOT OK with port 80
        else
            host = args[0];

        //-- host = "www.kitchener.ca";  OK 301 Moved permanently  
        //-- host = "app2.kitchener.ca";  OK 200 
        //-- host = "www.kitchener.ca";OK 301 
        //-- host = "www.google.com"; OK 200, slows down when byteChunk increases to 25600 
        host = "reqbin.com";
        string result = SocketSendReceive(host, port);
        Console.WriteLine(result);
        Console.ReadKey();
    }
}
