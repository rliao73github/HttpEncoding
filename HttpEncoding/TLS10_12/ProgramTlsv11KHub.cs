using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace HttpEncoding
{
    //-- OK to switch to TLSv11 0x0302
    //-- Delibetaly set the Security Protocol to TLSv10
    //--    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
    //-- {}{5}RESOLVED 505 extra respons issue. Using Connection: close
    //-- wierd thing is rcv 494, 12, 505 three packets
    //-- second wierd thing is after rcving 494, 12, then hanging; FIXED by adding <EOF> in req header
    public class SslTcpClientKHubTls11
    {
        private static Hashtable certificateErrors = new Hashtable();

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        public static void RunClient(string machineName, string serverName)
        {
            // Create a TCP/IP client socket.
            // machineName is the host running the server application.
            TcpClient client = new TcpClient(machineName, 443);
            int intC0 = client.Available;

            //--


            Console.WriteLine("SslStreatm Test - Client connected.");
            // Create an SSL stream that will close the client's stream.
            SslStream sslStream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );
            // The server name must match the name on the server certificate.
            try
            {
                //--                sslStream.AuthenticateAsClient(serverName);
                //-- System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                sslStream.AuthenticateAsClient(serverName, null,
                    (SslProtocols)ServicePointManager.SecurityProtocol, true);

                var secPro1 = (SslProtocols)ServicePointManager.SecurityProtocol;

            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }

            int intC1 = client.Available;

            //-- Signal the end of the message using the "<EOF>".
            //-- byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
            string requestMessage = "POST /REST/system/authenticatePeopleSoft HTTP/1.1\r\n" +
            "Host: khub.kitchener.ca\r\n" +
            "Connection: close\r\n" +    //-- Connection: close is IMPORTANT! 2020-07-19
            "Content-Length: 45\r\n" +      //-- Content-Length does MATTER 2020-07-19
            "Pragma: no-cache\r\n" +
            "Cache-Control: no-cache\r\n" +
            "Accept: application/json\r\n" +    //-- used to be */*
            "X-Requested-With: XMLHttpRequest\r\n" +
            "Content-Type: application/x-www-form-urlencoded; charset=UTF-8\r\n" +
            "\r\n" +  //-- extra \r\n is IMPORTANT!

            "firstName=Rong&lastName=LIAO&employeeId=13456\r\n" +
            "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            //"\r\n<EOF>";  //-- no need to have <EOF>, use more \r\n   RESOLVED rcv 494, 12, 505

            //            string requestMessage = "GET / HTTP/1.1" +
            //"\r\nHost: khub.kitchener.ca" +
            //"\r\nConnection: Close\r\n\r\n";

            var secPro2 = (SslProtocols)ServicePointManager.SecurityProtocol;

            byte[] requestBytes = Encoding.ASCII.GetBytes(requestMessage);

            sslStream.Write(requestBytes);
            sslStream.Flush();

            string serverMessage = ReadMessage(sslStream, client);
            Console.WriteLine("SslStreatm Test - Server says: \r\n {0} \r\n", serverMessage);

            var secPro3 = (SslProtocols)ServicePointManager.SecurityProtocol;

            client.Close();
            Console.WriteLine("SslStreatm Test - Client closed.");
        }
        static string ReadMessage(SslStream sslStream, TcpClient tc)
        {
            //-- added by Os 2020-07-25 
            int i1 = tc.Available;

            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048]; //-- new byte[2048];
            StringBuilder messageData = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                //Decoder decoder = Encoding.UTF8.GetDecoder();
                //char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                //decoder.GetChars(buffer, 0, bytes, chars, 0);

                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                sb.Clear();
                sb.Append(chars); 
                messageData.Append(chars);

                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes > 0);

            return messageData.ToString();
        }
        private static void DisplayUsage()
        {
            Console.WriteLine("To start the client specify:");
            Console.WriteLine("clientSync machineName [serverName]");
            Environment.Exit(1);
        }

        static void ReadCallback(IAsyncResult ar)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            int byteCount = -1;
            bool completed = false;

            Byte[] buffer = new Byte[256];
            StringBuilder readData = new StringBuilder();

            SslStream stream = (SslStream)ar.AsyncState;
            try
            {
                Console.WriteLine("Reading data from the server.");
                byteCount = stream.EndRead(ar);
                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, byteCount)];
                decoder.GetChars(buffer, 0, byteCount, chars, 0);
                readData.Append(chars);
                // Check for EOF or an empty message.
                if (readData.ToString().IndexOf("<EOF>") == -1 && byteCount != 0)
                {
                    // We are not finished reading.
                    // Asynchronously read more message data from  the server.
                    stream.BeginRead(buffer, 0, buffer.Length,
                        new AsyncCallback(ReadCallback),
                        stream);
                }
                else
                {
                    Console.WriteLine("Message from the server: {0}", readData.ToString());
                }
            }
            catch (Exception readException)
            {
                 Exception e = readException;
                completed = true;
                return;
            }
            completed = true;
        }

        public static void Main_TLS11(string[] args)
        {
            string serverCertificateName = null;
            string machineName = null;
            if (args == null || args.Length < 1)
            {
                DisplayUsage();
            }
            // User can specify the machine name and server name.
            // Server name must match the name on the server's certificate.
            machineName = args[0];
            if (args.Length < 2)
            {
                serverCertificateName = machineName;
            }
            else
            {
                serverCertificateName = args[1];
            }
            machineName = "khub.kitchener.ca";
            serverCertificateName = "khub.kitchener.ca";

            //--            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            //TcpClient client = new TcpClient(decodedUri.DnsSafeHost, 443);
            //SslStream sslStream = new SslStream(client.GetStream());

            //// use this overload to ensure SslStream has the same scope of enabled protocol as HttpWebRequest
            //sslStream.AuthenticateAsClient(decodedUri.Host, null,
            //    (SslProtocols)ServicePointManager.SecurityProtocol, true);

            // Check sslStream.SslProtocol here

            //client.Close();
            //sslStream.Close();

            RunClient(machineName, serverCertificateName);

            Console.ReadKey();
        }
    }
}
