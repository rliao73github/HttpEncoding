using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class GetSocketPort80Tax
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

            //-- can NOT connect to ebilling, or khub
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
        int byteChunk = 2560;

        //string request = "GET / HTTP/1.1\r\nHost: " + server +
        //    "\r\nConnection: Close\r\n\r\n";
        string request = "POST /modules/tax/Partial/Calculator/Calculate.aspx HTTP/1.1" +
"\r\nHost: www.kitchener.ca" +
"\r\nConnection: Close" +
"\r\nX-Requested-With: XMLHttpRequest" +
"\r\nContent-Type: application/x-www-form-urlencoded; charset=UTF-8" +
"\r\n" +
"\r\nStreetNumber=79&StreetName=REDTAIL+ST&StreetUnit=&SearchStreetAddressTermsOfUse=on" +
        "\r\n\r\n";
        //byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);
       //-- byte[] requestBytes = Encoding.UTF8.GetBytes(request);

        Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
        Byte[] bytesReceived = new Byte[byteChunk];
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

                if (count == 2) break;

                bytes = s.Receive(bytesReceived, bytesReceived.Length, SocketFlags.None);
                if (bytes < byteChunk)
                {
                    part2 = Encoding.ASCII.GetString(bytesReceived, 0, byteChunk);

                    pageEnd = pageEnd + part2;

                }
                part1 = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
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
            //-- host = "www.google.com";  OK with port 80 
            host = "ebilling.kitchener.ca"; //-- NOT OK with port 80
        else
            host = args[0];

        //-- host = "www.kitchener.ca";  OK 301 Moved permanently  
        //-- host = "app2.kitchener.ca";  OK 200 
        //-- host = "www.kitchener.ca";OK 301 
        //-- host = "www.google.com"; OK 200, slows down when byteChunk increases to 25600 
        host = "www.kitchener.ca";
        string result = SocketSendReceive(host, port);
        Console.WriteLine(result);
        Console.ReadKey();
    }
}
