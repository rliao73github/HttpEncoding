﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using RestSharp;
using System.Collections.Generic;



namespace HttpEncoding
{
    public class GetSocket
    {
        private static RestClient client = new RestClient("https://app2.kitchener.ca/");
        public static void Main(string[] args)
        {
            RestRequest request = new RestRequest("/esolutions/GetCokStreets/ki", Method.GET);
            IRestResponse<List<string>> response = client.Execute<List<string>>(request);
        //--Console.ReadKey();

        //var host = args[0];
        //var resource = args[1];
        //-- http://jsonplaceholder.typicode.com/posts/1

        //-- http://app2.kitchener.ca/services/cok_st_tax_calculator.aspx

            var host = "app2.kitchener.ca"; //-- "ebilling.kitchener.ca";
        var resource = "/services/cok_st_tax_calculator.aspx";
        var result = GetResource(host, resource);

            //-- Console.WriteLine(result);
            Console.ReadKey();
        }

        private static string GetResource(string host, string resource)
        {
            var hostEntry = Dns.GetHostEntry(host);
            var socket = CreateSocket(hostEntry);
            SendRequest(socket, host, resource);
            return GetResponse(socket);
        }

        private static Socket CreateSocket(IPHostEntry hostEntry)
        {
            const int httpPort = 80;
            //-- const int httpPort = 443;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            foreach (var address in hostEntry.AddressList)
            {
                var endPoint = new IPEndPoint(address, httpPort);
                var socket = new Socket(endPoint.AddressFamily,
                                  SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                if (socket.Connected)
                {
                    return socket;
                }
            }
            return null;
        }

        private static void SendRequest(Socket socket, string host, string resource)
        {
            string requestMessage = "GET /services/cok_st_tax_calculator.aspx HTTP/1.1" + "\r\n" +
"Host: app2.kitchener.ca" + "\r\n" +
"Connection: keep-alive" + "\r\n" +
//"Pragma: no-cache" + "\r\n" +
//"Cache-Control: no-cache" + "\r\n" +
//"Upgrade-Insecure-Requests: 1" + "\r\n" +
//"User-Agent: Mozilla/5.0(Windows NT 10.0; Win64; x64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36" + "\r\n" +
//"Accept: text/html,application/xhtml + xml,application/xml; q = 0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" + "\r\n"  +
//"Accept-Encoding: gzip, deflate" + "\r\n" +
"Accept-Language: en-US,en;q=0.9";

            //var requestMessage = String.Format(
            //    "GET {0} HTTP/1.1\r\n" +
            //    "Host: {1}\r\n" +
            //    "\r\n",
            //    resource, host
            //);

            var requestBytes = Encoding.ASCII.GetBytes(requestMessage);
            socket.Send(requestBytes);
        }

        private static string GetResponse(Socket socket)
        {
            int bytes = 0;
            byte[] buffer = new byte[256];
            var result = new StringBuilder();

            do
            {
                bytes = socket.Receive(buffer);
                result.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
            } while (bytes > 0);

            return result.ToString();
        }
    }
}
